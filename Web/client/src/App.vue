<style scoped lang="scss">
  .main {
    height: 100%;
  }
  .container {
    select {
      height: 1rem;
    }
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
import Examples from "./examples"

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
