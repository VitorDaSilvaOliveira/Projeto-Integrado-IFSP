// middleware/auth.global.js
export default defineNuxtRouteMiddleware((to, from) => {
  if (process.client) {
    const autorizado = localStorage.getItem('loginAutorizado') === 'true'
    if (!autorizado && to.path !== '/login') {
      return navigateTo('/login')
    }
  }
})
