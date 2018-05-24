<template>
<header>
    <md-toolbar class="md-dense md-primary" md-theme="toolbar2">
        <div class="container">
            <div>
                <router-link to="/" class="home-link">
                    <img src="../../assets/logo.png">
                    <h1 class="md-title">HeatDevices</h1>
                </router-link>    
            </div>

            <nav>
                <router-link tag="md-button" to="/" >Главная</router-link>

                <md-badge v-if="active_orders_num" class="md-primary md-square" md-position="top" :md-content="active_orders_num">
                    <router-link tag="md-button" to="/orders" >Заказы</router-link>
                </md-badge>

                <router-link v-else tag="md-button" to="/orders" >Заявки</router-link>
                
                <router-link tag="md-button" to="/reviews" >Отзывы</router-link>
            </nav>

            <user-box></user-box>
        </div>
    </md-toolbar>              
</header>    
</template>

<script>
import UserBox from './userBox'

import api from '@/API'
import auth from '@/auth'
import {EventBus} from '@/EventBus'

export default {
    name: 'head-bar',
    components: {
        'user-box': UserBox
    },
    data() {
        return {
            user: auth.user,
            active_orders_num: 0
        }
    },
    created() {
        EventBus.$on('refresh-page', this.checkOrders);
        EventBus.$on('user-sign-in', this.checkOrders);
        EventBus.$on('user-sign-out', this.headBarSignOutListener);
    },
    destroyed() {
        EventBus.$off('refresh-page', this.checkOrders);
        EventBus.$off('user-sign-in', this.checkOrders);
        EventBus.$off('user-sign-out', this.headBarSignOutListener);
    },
    methods: {
        headBarSignOutListener() {
            this.active_orders_num = 0;
        },

        async checkOrders() {
            if (!auth.userIsWorker()) {
                return;
            }

            try {
                const orders = await api.getAllOrders();
                this.active_orders_num = orders.filter(o => o.status == 0).length;
            } catch (error) {
                console.log(error);
            }
        }
    }
}
</script>

<style scoped>
header {    
    margin-bottom: 30px;
}

.container {
    display: flex;
    flex-flow: row wrap;
    justify-content: space-between;
    align-items: center;
    margin: 0 auto;
    width: 70%;
}

.home-link {
    display: flex;
    flex-flow: row wrap;
    align-items: center;
}

img {
    max-height: 50px;
}

nav > * {
    margin: auto;
}

nav {
    display: flex;
    flex-flow: row wrap;
}
</style>
