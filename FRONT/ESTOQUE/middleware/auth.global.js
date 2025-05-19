export default defineNuxtRouteMiddleware((to, from) => {
  // Executa no client-side, então use process.client para acessar localStorage
  if (process.client) {
    const autorizado = localStorage.getItem('loginAutorizado') === 'true';
    if (!autorizado && to.path !== '/login') {
      return navigateTo('/login');
    }
  }
});
