<script>
import { useRouter } from 'vue-router';
import { formatarData, formatarDataEUA } from '~/composables/visualization';
import axios from 'axios';
import { IconesEstoque } from '#components';

export default {
  data () {
    return {

    }
  },

  watch: {
  },

  methods: {
  },

  async mounted () {
  },

  unmounted () {
  }

}
</script>

<template>

  <div class="D-flex JC-center ALITEM-center HEIGHT-100vh">

      <!-- Caixa da Esquerda => Formulário LOGIN | Redes sociais -->
      <div id="left-box" class="caixa-esquerda D-flex FD-column ALITEM-center HEIGHT-100vh">

        <!-- Logo -->
        <div style="margin-top: 10%;">
          <IconesEstoque corProp="black" alturaProp="10" larguraProp="10" />
        </div>

        <!-- Formulário LOGIN -->
        <div class="formulario-login D-flex JC-center ALITEM-center PADDING-B20">
          <form class="D-flex FD-column JC-flex-start HEIGHT-60 WIDTH-100">

            <!-- Usuario -->
            <div class="mb-3">
              <label for="usuario-input" class="form-label FFAMILY-sans-serif FSIZE-14px COLOR-azul-preto">Usuário</label>
              <input type="password" class="form-control BOR-grey-1 BORRAD-8" id="usuario-input">
            </div>
            
            <!-- Senha -->
            <div class="mb-3">
              <label for="senha-input" class="form-label FFAMILY-sans-serif FSIZE-14px COLOR-azul-preto">Senha</label>
              <input type="password" class="form-control BOR-grey-1 BORRAD-8" id="senha-input">
            </div>

            <!-- Botão ENTRAR (login) -->
            <div class="D-flex JC-space-between WIDTH-100" style="margin-top: 20px;">
              <a class="FSIZE-13px">Esqueceu sua senha?</a>

              <NuxtLink to="/" style="width: 25%;">
                <button
                  style="width: 100%;"
                  id="login-button"
                  type="button"
                  class="btn BGC-azul-2 COLOR-white WIDTH-25 BORRAD-5 BGC-H-azul-1"
                >Entrar</button>
              </NuxtLink>

            </div>

            <!-- <div class="FSIZE-13px COLOR-red">Credenciais inválidas. Tente novamente.</div> -->

          </form>
        </div>
        
      </div>

      <!-- Caixa da Direita com a imagem -->
      <div id="right-box" class="caixa-direita D-flex ALITEM-center JC-center HEIGHT-100vh">
        <!-- <img src="~/assets/images/imagem-login.jpg" class="HEIGHT-100 WIDTH-100 imagem-invertida"> -->
        <img src="~/assets/images/prateleiras-galpao.jpg" class="HEIGHT-100 WIDTH-100" style="opacity: 0.8;">
      </div>
      
  </div>

</template>

<style>
.imagem-invertida {
  -webkit-transform: scaleX(-1);
  transform: scaleX(-1);
}

.caixa-esquerda { width: 35%; height: 100vh; }

.caixa-direita { width: 65%; height: 100vh; }

.logo-login { height: 20%; width: 100%; }

.imagem-logo-login { width: 35%; height: 30%; }

.formulario-login { width: 62.5%; height: 70%; }

.redes-sociais { width: 50%; height: 10%; }

@media only screen and (max-width: 768px) { /* Celulares|Tablets */
  .caixa-esquerda { width: 100%; height: 100vh; }

  .caixa-direita { width: 0%; height: 100vh; }

  .logo-login { width: 100%; height: 30%; }

  .imagem-logo-login { width: 80%; height: 50%; }

  .formulario-login { width: 80%; height: 80%; }

  .redes-sociais { width: 80%; height: 20%; }
}

@media only screen and (min-width : 769px) and (max-width: 1100px) { /* Computadores: Telas menores */
  .caixa-esquerda { width: 45%; }

  .caixa-direita { width: 55%; }

  .logo-login { width: 100%; height: 30%; }

  .imagem-logo-login { width: 50%; height: 40%; }

  .formulario-login { width: 70%; height: 75%; }

  .redes-sociais { width: 80%; height: 20%; }
}
</style>