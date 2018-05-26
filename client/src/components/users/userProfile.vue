<template>
<div>
    <md-empty-state v-if="!user.is_authenticated" v-show="!user.is_authenticated"
        md-icon="lock"
        md-label="Необходима авторизация"
        md-description="Для продолжения Вам необходимо войти в свой аккаунт. Если Вы ещё не зарегистрированы, то можете это сделать на странице регистрации.">
      <md-button class="md-primary md-raised" @click="$router.push('/sign_up')">Зарегистрироваться</md-button>
    </md-empty-state>

    <div v-else>
        <md-progress-spinner v-if="loading" class="md-accent" :md-stroke="3" md-mode="indeterminate"></md-progress-spinner>

        <md-card v-else-if="!(edit_page || change_password)" class="profile-card">
        
            <md-card-header>
                <md-card-header-text>
                    <div class="md-title">{{user.profile.name}} 
                        {{user.profile.surname}}
                        <md-menu md-size="small" md-direction="bottom-start">
                            <md-menu-content>
                                <md-menu-item @click="edit_page = true">
                                    <span>Изменить данные</span>
                                    <md-icon style="margin-right: 0">edit</md-icon>
                                </md-menu-item>
                                <md-menu-item @click="change_password = true">
                                    Смена пароля
                                    <md-icon style="margin-right: 0">edit</md-icon>
                                </md-menu-item>
                            </md-menu-content>
                                
                            <md-button class="md-icon-button" md-menu-trigger>
                                <md-icon>more</md-icon>
                            </md-button>
                        </md-menu>
                    </div>
                    <div class="md-subhead">{{userRoles}}</div>
                    <div class="md-subhead">Телефон: {{user.profile.phone}}</div>
                </md-card-header-text>
            </md-card-header>
                      
            <md-card-media md-ratio="16:9">
                <img :src="user.profile.avatar">
            </md-card-media>
        </md-card>

        <edit-user v-else-if="edit_page"
            :user="user"
            v-on:page-leave="edit_page = false"
            v-on:profile-edited="profileEditedListener">
        </edit-user>

        <change-password v-else
            :user="user"
            v-on:page-leave="change_password = false"
            v-on:password-changed="passwordChangedListener">
        </change-password>
    </div>
    
    <md-dialog-alert
        md-content="Вы успешно сменили пароль."
        md-confirm-text="Ок"
        :md-active.sync="show_dialog">
    </md-dialog-alert>
</div>
</template>

<script>
import EditUser from './editUser'
import ChangePassword from './changePassword'

import api from '@/API'
import auth from '@/auth'
import {EventBus} from '@/EventBus'

export default {
    name: 'profile',
    components: {
        'edit-user': EditUser,
        'change-password': ChangePassword
    },
    data() {
        return {
            user: auth.user,
            loading: false,
            edit_page: false,
            show_dialog: false,
            change_password: false
        }
    },
    metaInfo() {
        return {
            title: this.user.name
        }
    },
    async created() {
        EventBus.$on('user-sign-out', this.signOutProfileListening);
    },
    destroyed() {
        EventBus.$off('user-sign-out', this.signOutProfileListening);
    },
    computed: {
        userRoles() {
            if (!this.user.roles)
                return '';

            const roleNames = {
                'admin': 'администратор',
                'worker': 'сотрудник',
                'client': 'клиент',
                'user': 'пользователь'
            };

            return this.user.roles.map(r => roleNames[r]).join(', ');
        }
    },
    methods: {
        signOutProfileListening() {
            this.$router.push('/');
        },

        profileEditedListener(profile) {
            this.user.profile = profile;
        },

        passwordChangedListener() {
            this.show_dialog = true;
        }
    }
}
</script>

<<style scoped>
.profile-card {
    margin: 0 auto;
    width: 70%;
}
</style>

