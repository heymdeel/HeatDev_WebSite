<template>
<div>
    
    <md-button @click="$router.push('/sign_up')">Регистрация</md-button>
    <md-button class="md-raised md-accent" @click="openSignInDialog()">Войти</md-button>

    <md-dialog :md-active.sync="show_dialog" md-theme="default" @md-closed="onDialogClose">
        <md-dialog-title>Авторизация</md-dialog-title>

        <md-dialog-content>
            <form novalidate @submit.stop.prevent="submit">
                
                <md-field :class="{'md-input-invalid': errors.has('login')}">
                    <label>Логин</label>
                    <md-input required
                        name="login"
                        v-model="login"
                        data-vv-as="логин"
                        data-vv-name="login"
                        v-validate="'required'"
                        @keyup.enter.native="signIn"
                        ></md-input>
                </md-field>
                <span class="error">{{errors.first('login')}}</span>

                <br>

                <md-field md-has-password :class="{'md-input-invalid': errors.has('password')}">
                    <label>Пароль</label>
                    <md-input required 
                        type="password" 
                        name="password" 
                        v-model="password" 
                        data-vv-as="пароль"
                        data-vv-name="password"
                        v-validate="'required'" 
                        @keyup.enter.native="signIn"></md-input>
                </md-field>
                <span class="error">{{errors.first('password')}}</span>

            </form>
        </md-dialog-content>

        <md-dialog-actions>
            <md-button class="md-primary" @click="closeSignInDialog">Отмена</md-button>
            <md-button class="md-primary md-raised" @click="signIn">Войти</md-button>
        </md-dialog-actions>
    </md-dialog>

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
    name: 'auth-comp',
    data() {
        return {
            login: '',
            password: '',
            snackbar_message: '',
            show_dialog: false,
            show_snackbar: false
        }
    },
    methods: {
        closeSignInDialog() {
            this.show_dialog = false;
        },

        onDialogClose() {
            this.login = '';
            this.password = '';
            this.$validator.reset();
        },

        openSignInDialog() {
            this.show_dialog = true;
        },

        async signIn() {
            if (!await this.$validator.validate()) {
                this.snackbar_message = 'Введите все данные корректно';
                this.show_snackbar = true;
                return;
            }

            const credentials = {
                    login: this.login,
                    password: this.password
                };
            try {
                EventBus.$emit('global-loading-start');
                await auth.signIn(credentials);
                
                //EventBus.$emit('user-login');
                this.show_dialog = false;
            } catch(error) {
                console.log(error);

                const status = error.response.status;

                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 400) {
                    this.snackbar_message = 'Введите данные корректно';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 404) {
                    this.snackbar_message = 'Неверный логин/пароль';
                    this.show_snackbar = true;
                    return;
                }
            } finally {
                EventBus.$emit('global-loading-finish');
            }
        }
    }
}
</script>

<style scoped>
.error {
    color: red
}
</style>
