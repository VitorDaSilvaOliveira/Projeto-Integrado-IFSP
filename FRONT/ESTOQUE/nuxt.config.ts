// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },

  modules: [
    '@nuxt/content',
    '@nuxt/eslint',
    '@nuxt/icon',
    '@nuxt/image',
    '@nuxt/scripts',
    '@nuxt/test-utils',
    '@nuxt/ui'
  ],

  css: [
    'bootstrap/dist/css/bootstrap.min.css',
    '~/assets/main.css',
  ],

  nitro: {
    devProxy: {
      '/api': {
        target: 'https://apiprojifsp.azurewebsites.net',
        changeOrigin: true,
        prependPath: false
      }
    }
  }
})
