import Vue from 'vue'
import Router from 'vue-router'
import HomePage from '@/components/homePage'
import SignUpPage from '@/components/users/signUp'
import Orders from '@/components/orders/orders'
import CreateOrder from '@/components/orders/createOrder'
import OrderDetail from '@/components/orders/orderDetail'
import CreateReview from '@/components/reviews/createReview'

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
    },
    {
      path: '/orders',
      name: 'orders',
      component: Orders
    },
    {
      path: '/orders/create',
      name: 'create_order',
      component: CreateOrder
    },
    {
      path: '/orders/:id',
      name: 'orderDetail',
      component: OrderDetail
    },
    {
      path: '/orders/:id/review',
      name: 'create_review',
      component: CreateReview
    }
  ]
})
