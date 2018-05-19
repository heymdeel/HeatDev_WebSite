import Vue from 'vue'
import App from './App'
import router from './router'

import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'
import './themes.scss'

import VeeValidate, {Validator} from 'vee-validate'
import messagesRU from 'vee-validate/dist/locale/ru';

Validator.localize('ru', messagesRU);

const config = {
    locale: 'ru',
    dictionary: {
      ru: { messages: messagesRU }
    }
};

Vue.use(VueMaterial)
Vue.use(VeeValidate, config)

Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
