import Schema from './schema'

// Much of the below came from https://github.com/nbelyh/editsvgcode.

function getLastOpenedTag(text): { tagName: string, isAttributeSearch: boolean } {
  // get all tags inside of the content
  const tags = text.match(/<\/*(?=\S*)([a-zA-Z-]+)/g);
  if (!tags) {
    return undefined;
  }
  // we need to know which tags are closed
  const closingTags = [];
  for (let i = tags.length - 1; i >= 0; i--) {
    if (tags[i].indexOf('</') === 0) {
      closingTags.push(tags[i].substring('</'.length));
    } else {
      // get the last position of the tag
      const tagPosition = text.lastIndexOf(tags[i]);
      const tag = tags[i].substring('<'.length);
      const closingBracketIdx = text.indexOf('/>', tagPosition);
      // if the tag wasn't closed
      if (closingBracketIdx === -1) {
        // if there are no closing tags or the current tag wasn't closed
        if (!closingTags.length || closingTags[closingTags.length - 1] !== tag) {
          // we found our tag, but let's get the information if we are looking for
          // a child element or an attribute
          text = text.substring(tagPosition);
          return {
            tagName: tag,
            isAttributeSearch: text.indexOf('<') > text.indexOf('>')
          };
        }
        // remove the last closed tag
        closingTags.splice(closingTags.length - 1, 1);
      }
      // remove the last checked tag and continue processing the rest of the content
      text = text.substring(0, tagPosition);
    }
  }
}

function isItemAvailable(itemName, maxOccurs, items) {
  // the default for 'maxOccurs' is 1
  maxOccurs = maxOccurs || '1';
  // the element can appear infinite times, so it is available
  if (maxOccurs && maxOccurs === 'unbounded') {
    return true;
  }
  // count how many times the element appeared
  let count = 0;
  for (let i = 0; i < items.length; i++) {
    if (items[i] === itemName) {
      count++;
    }
  }
  // if it didn't appear yet, or it can appear again, then it
  // is available, otherwise is not
  return count === 0 || parseInt(maxOccurs) > count;
}

function findAttributes(elements) {
  const attrs = [];
  for (let element of elements) {
    // skip level if it is a 'complexType' tag
    if (element.tagName === 'complexType') {
      const child = findAttributes(element.children);
      if (child) {
        return child;
      }
    }
      // we need only those XSD elements that have a
    // tag 'attribute'
    else if (element.tagName === 'attribute') {
      attrs.push(element);
    }
  }
  return attrs;
}

function getAvailableAttribute(monaco, lastOpenedTag, usedChildTags) {
  const availableItems = [];

  const info = Schema[lastOpenedTag.tagName];

  // if there are no attributes, then there are no
  // suggestions available
  if (!info || !info.attributes) {
    return [];
  }
  for (let attribute of info.attributes) {
    // get all attributes for the element
    // accept it in a suggestion list only if it is available
    if (isItemAvailable(attribute.name, attribute.maxOccurs, usedChildTags)) {
      // mark it as a 'property', and get the documentation
      availableItems.push({
        suggestions: [],
        label: attribute.name,
        insertText: `${attribute.name}="$\{1\}"`,
        kind: monaco.languages.CompletionItemKind.Property,
        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
        detail: attribute.detail,
        documentation: {
          value: attribute.description || "",
          isTrusted: true
        }
      });
    }
  }
  // return the elements we found
  return availableItems;
}

function getAvailableElements(monaco, lastOpenedTag, isOpenElement) {
  const availableItems = [];

  const info = Schema[lastOpenedTag.tagName];

  // if there are no such elements, then there are no suggestions
  if (!info || !info.elements) {
    return [];
  }
  for (let element of info.elements) {
    const elementInfo = Schema[element];
    let insertText = `${isOpenElement ? '' : '<'}${element}`
    if (elementInfo.autoClose) {
      insertText += `$\{0\} />`
    } else {
      insertText += `>$\{0\}</${element}${isOpenElement ? '' : '>'}`
    }

    availableItems.push({
      label: element,
      insertText,
      kind: monaco.languages.CompletionItemKind.Class,
      detail: elementInfo.detail,
      insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      documentation: {
        value: elementInfo.description || "",
        isTrusted: true
      }
    });
  }
  // return the suggestions we found
  return availableItems;
}

export function getXmlCompletionProvider(monaco) {
  return {
    triggerCharacters: ['<', ' '],
    provideCompletionItems: function (model, position, completionContext) {
      let i;
      // get editor content before the pointer
      let textUntilPosition: string = model.getValueInRange({
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: position.lineNumber,
        endColumn: position.column
      })
      let textFromPosition: string = model.getValueInRange({
        startLineNumber: position.lineNumber,
        startColumn: position.column,
        endLineNumber: Number.MAX_VALUE,
        endColumn: Number.MAX_VALUE
      })
      // if we want suggestions, inside of which tag are we?
      const lastOpenedTag = getLastOpenedTag(textUntilPosition);
      if (!lastOpenedTag) return { suggestions: [] }
      let usedItems = [];
      if (lastOpenedTag.isAttributeSearch) {
        // If we're looking for attributes, find the last opening and next closing tags and find all used attributes.
        // Not the most accurate but good enough.
        const opening = textUntilPosition.lastIndexOf("<")
        const closing = textFromPosition.indexOf(">")
        
        const tagText = `${textUntilPosition}${textFromPosition}`.slice(opening, textUntilPosition.length + closing);
        // The already used attributes don't need to be precise, they can include junk as well.
        const matches = tagText.matchAll(/(.+?)[ =]/g)
        usedItems = Array.from(matches).map(m => m[1].trim())
        
        // If there are an odd number of open quotes, disable autocorrect.
        // Not great, but it'll do.
        const openQuotes = textUntilPosition.slice(opening, textUntilPosition.length).split('').filter(l => l === '"')
        if (openQuotes.length > 0 && openQuotes.length % 2 != 0) {
          return {
            suggestions: []
          }
        }

        return {
          suggestions: getAvailableAttribute(monaco, lastOpenedTag, usedItems)
        }
      }

      if (completionContext.triggerCharacter === ' ') return []

      // get the elements/attributes that are already mentioned in the element we're in
      const isOpenElement = textUntilPosition[textUntilPosition.length - 1] === "<"

      return {
        suggestions: getAvailableElements(monaco, lastOpenedTag, isOpenElement)
      };
    }
  };
}

export function getXmlHoverProvider(monaco) {
  return {
    provideHover: function (model, position, token) {
      let wordInfo = model.getWordAtPosition(position);
      if (!wordInfo)
        return;

      let line = model.getLineContent(position.lineNumber);
      if (line.substring(wordInfo.startColumn - 2, 1) === '<') {
        let info = Schema[wordInfo.word];
        if (info) {
          return {
            contents: [
              {value: `**${wordInfo.word}**`},
              {value: info.description}
            ],
            range: new monaco.Range(position.lineNumber, wordInfo.startColumn, position.lineNumber, wordInfo.endColumn),
          }
        }
      } else {
        let textUntilPosition = model.getValueInRange({
          startLineNumber: 1,
          startColumn: 1,
          endLineNumber: position.lineNumber,
          endColumn: position.column
        })
        const lastOpenedTag = getLastOpenedTag(textUntilPosition);
        let info = Schema[lastOpenedTag.tagName];
        if (info && info.attributes) {
          for (let i = 0; i < info.attributes.length; i++) {
            // get all attributes for the element
            const attribute = info.attributes[i];
            if (attribute.name === wordInfo.word) {
              return {
                contents: [
                  {value: `**${wordInfo.word}**`},
                  {value: attribute.description}
                ],
                range: new monaco.Range(position.lineNumber, wordInfo.startColumn, position.lineNumber, wordInfo.endColumn),
              }
            }
          }
        }
      }
    }
  }
}
