<style scoped lang="scss">
  .main {
    height: 100%;
    select {
      outline: none;
      -webkit-appearance: none;
      -moz-appearance: none;
      appearance: none;
      background: url("data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='grey'><polygon points='0,0 16,0 8,16'/></svg>") no-repeat right 0.7em top 50%, 0 0;
      background-size: 0.65em auto, 100%;
      border: 1px solid #ccc;
      padding: 0.5em;
      width: 100%;
      font-size: 1em;
      border-radius: 0.25em;
      margin-bottom: 10px;
      // increase spacing of option
      option {
        padding: 10px;
      }
    }
  }
  .container {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-column-gap: 10px;
    height: 100%;
  }
  iframe {
    width: 100%;
    height: 100%;
    border: none;
  }
</style>

<template>
  <div class="main">
    <select @change="onLoadExample($event.target.value)">
      <option v-for="(text, name) in Examples" :value="text">{{name}}</option>
    </select>
    <div class="container">
      <div id="editor" />
      <iframe :src="iframeSrc"/>
    </div>
  </div>
</template>

<script setup>
import * as monaco from 'monaco-editor'
import { onMounted, ref } from 'vue'
import { getXmlCompletionProvider, getXmlHoverProvider } from './completion-provider'
import { debounce } from 'lodash'
import Examples from './examples'

let editor = null
const iframeSrc = ref(``)
const xml = ref('')

const debounceFunc = debounce(async () => {
  const editorVal = editor.getValue()
  if (editorVal !== xml.value) {
    xml.value = editorVal
    await refreshPreview()
  }
}, 1000)

function onLoadExample(xml2) {
  xml.value = xml2
  editor.setValue(xml.value)
  refreshPreview()
}

async function refreshPreview() {
  const val = xml.value
  monaco.editor.removeAllMarkers("owner")
  const response = await fetch(`api/pdf`, {
    method: "POST",
    headers: {
      "Content-Type": "application/xml"
    },
    body: val
  })
  if (response.status === 400) {
    const body = await response.json()
    monaco.editor.setModelMarkers(editor.getModel(), "owner", [{
      severity: monaco.MarkerSeverity.Error,
      startLineNumber: body.lineNumber,
      endLineNumber: body.lineNumber,
      startColumn: body.columnNumber,
      endColumn: body.columnNumber,
      message: body.message
    }])
  }
  if (response.status === 200) {
    const textBody = await response.text()
    iframeSrc.value = `api/pdf?id=${textBody}`
  }
}

onMounted(async () => {
  editor = monaco.editor.create(document.getElementById('editor'), {
    theme: 'vs-dark', // dark theme
    language: 'xml',
    automaticLayout: true,
    tabSize: 2,
    suggestOnTriggerCharacters: true,
  })
  editor.setValue(Examples.Blank)
  editor.onDidChangeModelContent(debounceFunc)
  xml.value = Examples.Blank
  await refreshPreview()
})

// register a completion item provider for xml language
monaco.languages.registerCompletionItemProvider('xml', getXmlCompletionProvider(monaco));
monaco.languages.registerHoverProvider('xml', getXmlHoverProvider(monaco));

</script>
