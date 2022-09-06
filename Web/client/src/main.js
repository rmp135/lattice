import { createApp } from 'vue'
import App from './App.vue'
import editorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker'

self.MonacoEnvironment = {
  getWorker(_, label) {
    return new editorWorker()
  }
}

createApp(App)
  .mount('#app')
