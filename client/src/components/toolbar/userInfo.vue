<template>
<div>
    <md-menu md-size="medium" md-align-trigger>
        <md-button md-menu-trigger>
            <span>{{user.profile.name}}</span>
            <md-avatar>
                <img :src="user.profile.avatar">
            </md-avatar>
            <md-icon>expand_more</md-icon>
        </md-button>

        <md-menu-content>
            <md-menu-item @click="$router.push('/profile')">
                <span>Мой профиль</span>
            </md-menu-item>
            <md-divider></md-divider>
            <md-menu-item @click="signOut" >
                <span>Выход</span>
                <md-icon md-theme="default" class="md-accent">exit_to_app</md-icon>
            </md-menu-item>
        </md-menu-content>
    </md-menu>

    <md-snackbar md-theme="default" md-position="center" :md-duration="4000" :md-active.sync="show_snackbar" md-persistent>
        <span>{{snackbar_message}}</span>
        <md-button class="md-accent" @click="show_snackbar = false">Закрыть</md-button>
    </md-snackbar>
</div>
</template>

<script>
import auth from '@/auth'
import {EventBus} from '@/EventBus'

export default {
    name: 'user-info',
    data() {
        return {
            show_snackbar: false,
            snackbar_message: '',
            user: auth.user
        }
    },
    methods: {
        async signOut() {
            try {
                EventBus.$emit('global-loading-start');
                await auth.signOut();    
            } catch (error) {
                console.log(error);

                const status = error.response.status;

                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }
            } finally {
                EventBus.$emit('global-loading-finish');
            }
            
            //EventBus.$emit('user-logout');
        }
    }
}
</script>

<style scoped>
</style>
