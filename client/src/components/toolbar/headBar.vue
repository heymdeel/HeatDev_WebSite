<template>
<header>
    <md-toolbar class="md-dense md-primary" md-theme="toolbar2">
        
        <div class="logo">
            <router-link to="/" class="home-link">
                <img src="../../assets/logo.png">
                <h1 class="md-title">HeatDevices</h1>
            </router-link>    
        </div>

        <nav>
            <router-link tag="md-button" to="/" >Главная</router-link>
            <router-link tag="md-button" to="/courses" >Заказы</router-link>
            <router-link tag="md-button" to="/contacts" >Отзывы</router-link>
        </nav>

        <user-box class="user-form"></user-box>
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
    display: flex;
    flex-flow: row wrap;
    justify-content: space-around;
    margin-bottom: 30px;
}

.logo {
    margin-left: 15%;
    flex-basis: 20%;
}

.home-link {
    display: flex;
    flex-flow: row wrap;
    justify-content: flex-start;
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
    
    align-items: center;
}

.user-form {
    margin: 0 auto;
    margin-right: 15%;
}
</style>
