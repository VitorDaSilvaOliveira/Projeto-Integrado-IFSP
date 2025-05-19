<script>
import { useRouter } from 'vue-router';
import { formatarData, formatarDataEUA } from '~/composables/visualization';
import axios from 'axios';
import { IconesEstoque } from '#components';

export default {
  data () {
    return {
      // Variáveis estáticas: ----------------------------\
      token: useToken(),
      urlProd: useUrlProd(),
      urlProdProducao: useUrlProdProducao(),
      urlProdSistema: useUrlProdSistema(),
      urlProdControladoria: useUrlProdControladoria(),
      urlCaminhoParametros: '/Controladoria/parametros',
      endpoint: '/',
      hoje: formatarData(new Date()),
      hojeEUA: formatarDataEUA(new Date()),
      senhaDelete: 'tp190980*',
      senhaDeleteInputadaUsuario: '',
      anoAtual: new Date().getFullYear(),
      // //////////////////////////////////////////////////

      infoUsuariosWEB: [],
      staticInfoUsuariosWEB: [],

      router: useRouter(),
      listaUsuariosMockados: [
        {
          nomeUsuario: 'ADMIN',
          senhaUsuario: 'adm',
          role: 'Administrador'
        },
        {
          nomeUsuario: 'FINANCEIRO',
          senhaUsuario: 'fin',
          role: 'Financeiro'
        },
        {
          nomeUsuario: 'PRODUÇÃO',
          senhaUsuario: 'prod',
          role: 'Produção'
        },
        {
          nomeUsuario: 'RECURSOS HUMANOS',
          senhaUsuario: 'rh',
          role: 'Recursos Humanos'
        },
        {
          nomeUsuario: 'FISCAL',
          senhaUsuario: 'fisc',
          role: 'Fiscal'
        },
        {
          nomeUsuario: 'LOGÍSTICA',
          senhaUsuario: 'log',
          role: 'Logística'
        },
      ],
      selectedUsuario: localStorage.getItem('sigla'),
      selectedSenha: '',
      senhaLogin: '',
      wrongUserOrPassword: false,
      widthTela: window.innerWidth
    }
  },

  watch: {
  },

  methods: {
    organizeInfoUsuariosWEBArray (info) {
      return new Promise((resolve) => {
          this.infoUsuariosWEB = info;
          this.staticInfoUsuariosWEB = this.infoUsuariosWEB;

          resolve();
      });
    },
    async fetchInfoUsuariosWEB () {
      try {
        let res = await axios.get(`${this.urlProd}/sistema/usuarios/ativo-WEB`,
          { headers: { Authorization: `Bearer ${this.token}` } }
        );

        await this.organizeInfoUsuariosWEBArray(res.data);

      } catch (error) { console.log(error); }
    },
    
    saveUserLocalStorage (usuario) {

      localStorage.setItem('id_usuario', usuario.nCdUsuario);
      localStorage.setItem('nome', usuario.nomeUsuario);
      localStorage.setItem('nomeAbreviado', usuario.nomeAbreviado);
      localStorage.setItem('origem', usuario.origem);
      localStorage.setItem('userRole', usuario.perfil);
      localStorage.setItem('sigla', usuario.siglaUsuario);
      localStorage.setItem('hierarquia', usuario.hierarquia);
      localStorage.setItem('email', usuario.email);
    },
    getUserInfo () {
      for (let user of this.infoUsuariosWEB) {
        if (user.siglaUsuario === this.selectedUsuario) {
          this.selectedSenha = user.senha;
        }
      }
    },
    async login() {
      const url = `${this.urlProd}/login/${this.selectedUsuario}/${this.senhaLogin}`;
      try {
        const response = await axios.post(url);

        if (response.status === 200 && response.data) {
          const token = response.data.token; // Token retornado pelo backend
          const decodedToken = JSON.parse(atob(token.split('.')[1])); // Decodifica o payload do token JWT
          const expirationTime = decodedToken.exp * 1000; // Converte o tempo de expiração em milissegundos

          localStorage.setItem('authToken', token);
          localStorage.setItem('tokenExpiration', expirationTime.toString());
          this.saveUserLocalStorage(response.data.usuario)
          this.token = token; // Atualiza o token no estado local

          this.$router.push('/');
        } else {
          console.error('Erro ao autenticar. Verifique suas credenciais.');
        }
      } catch (error) {
        this.wrongUserOrPassword = true;
      }
    },
    isTokenValid() {
      const token = localStorage.getItem('token');
      const tokenExpiration = localStorage.getItem('tokenExpiration');
      const currentTime = new Date().getTime();

      if (token && tokenExpiration && currentTime < parseInt(tokenExpiration)) {

        console.log('O token ainda é válido')
        return true; // O token ainda é válido
      }

      // Remove token expirado
      localStorage.removeItem('token');
      localStorage.removeItem('tokenExpiration');
      console.log('Token expirou ou não existe')
      return false; // Token expirou ou não existe
    },


    // Métodos de refinamento: -----------------------------------------------------------------\
    // focusUsuarioInput () {
    //   document.getElementById('usuario-input').focus();
    // },
    // Lida com o clique do botão ENTER. (HGPK = Handle Global Press Key)
    HGPKEnterLogin (event) {
      const e = event;
      // Confere se o botão apertado foi o 'ENTER'.
      const ENTER = e.key === 'Enter';

      // Recupera botões e elementos da página.
      let loginButton = document.getElementById('login-button');

      if (ENTER) {
        e.preventDefault();
        loginButton.click();
      }
    },
    // Adiciona o escutador de eventos.
    addHGPKEnterLogin () {
      window.addEventListener('keydown', this.HGPKEnterLogin);
    },
    // Remove o escutador de eventos.
    removeHGPKEnterLogin () {
      window.removeEventListener('keydown', this.HGPKEnterLogin);
    },
    /////////////////////////////////////////////////////////////////////////////////////////////
  },

  async mounted () {
    await this.fetchInfoUsuariosWEB();

    this.addHGPKEnterLogin();
  },

  unmounted () {
    this.removeHGPKEnterLogin();
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
              <input type="password" class="form-control BOR-grey-1 BORRAD-8" id="usuario-input" v-model="senhaLogin">
            </div>
            
            <!-- Senha -->
            <div class="mb-3">
              <label for="senha-input" class="form-label FFAMILY-sans-serif FSIZE-14px COLOR-azul-preto">Senha</label>
              <input type="password" class="form-control BOR-grey-1 BORRAD-8" id="senha-input" v-model="senhaLogin">
            </div>

            <!-- Botão ENTRAR (login) -->
            <div class="D-flex JC-space-between WIDTH-100" style="margin-top: 20px;">
              <a class="FSIZE-13px">Esqueceu sua senha?</a>

              <NuxtLink to="/" @click="showProducaoAreas()" style="width: 25%;">
                <button
                  style="width: 100%;"
                  id="login-button"
                  type="button"
                  class="btn BGC-azul-2 COLOR-white WIDTH-25 BORRAD-5 BGC-H-azul-1"
                  @click="login()"
                >Entrar</button>
              </NuxtLink>

            </div>

            <div class="FSIZE-13px COLOR-red" v-show="wrongUserOrPassword">Credenciais inválidas. Tente novamente.</div>

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