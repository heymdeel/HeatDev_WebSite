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
                <router-link tag="md-button" to="/orders" >Заказы</router-link>
                <router-link tag="md-button" to="/reviews" >Отзывы</router-link>
            </nav>

            <user-box></user-box>
        </div>
    </md-toolbar>              
</header>    
</template>

<script>
import UserBox from './userBox'

export default {
    name: 'head-bar',
    components: {
        'user-box': UserBox
    },
    data() {
        return {
            login: ''
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
