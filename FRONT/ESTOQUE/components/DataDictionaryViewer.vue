<template>
  <div class="data-dictionary-container" v-html="htmlContent"></div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const htmlContent = ref('')

onMounted(async () => {
  const res = await fetch('https://apiprojifsp.azurewebsites.net/DataDictionary/Element/Index')
  const htmlText = await res.text()

  const parser = new DOMParser()
  const doc = parser.parseFromString(htmlText, 'text/html')

  const bodyHTML = doc.body.innerHTML

  htmlContent.value = bodyHTML
    .replaceAll('/_content/JJMasterData.Web/css/', '/data-dictionary/css/')
    .replaceAll('/_content/JJMasterData.Web/js/', '/data-dictionary/js/')
    .replaceAll('/_content/JJMasterData.Web/images/', '/data-dictionary/images/')
})
</script>

<style scoped>
.data-dictionary-container {
  max-width: 900px;  /* limita largura */
  margin: 0 auto;
  padding: 1rem;
  border: 1px solid #ccc;
  background: white;
  overflow: auto; /* scroll se precisar */
  height: 600px;   /* ajuste a altura que quiser */
}
</style>
