import Vue from 'vue'
import Router from 'vue-router'
import HomePage from '@/components/homePage'
import SignUpPage from '@/components/users/signUp'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage
    },
    {
      path: '/sign_up',
      name: 'sign_up',
      component: SignUpPage
    }
  ]
})
