<script>
export default {
  data () {
    return {
      htmlAdjusted: '',
      loginAutorizado: false
    }
  },

  async mounted () {
 
    this.loginAutorizado = localStorage.getItem('loginAutorizado') === 'true'

    if (this.loginAutorizado) {
      const res = await fetch('/api/DataDictionary/Element/Index') // proxy no nuxt.config.ts
      const htmlFromApi = await res.text()

      this.htmlAdjusted = htmlFromApi
        .replaceAll('/_content/JJMasterData.Web/css/', '/data-dictionary/css/')
        .replaceAll('/_content/JJMasterData.Web/js/', '/data-dictionary/js/')
        .replaceAll('/_content/JJMasterData.Web/images/', '/data-dictionary/images/')
    }

  }

}
</script>


<template>
  <div v-if="loginAutorizado">
    <h1 style="font-size: 7rem;">DICIONÁRIO DE DADOS</h1>
    <iframe 
    src="https://apiprojifsp.azurewebsites.net/MasterData/Form/Render/produto" 
    style="width: 100%; height: 80vh; border: none;"
    ></iframe>
  </div>
</template>

<style scoped>
.dictionary-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 1rem;
  border: 1px solid #ddd;
  overflow-x: auto;
}
</style>
