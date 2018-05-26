<template>
<div>
    <md-card>
        <md-card-header>
            <md-card-header-text>
                <div class="md-title">Регистрация</div>
            </md-card-header-text>
        </md-card-header>
        
        <md-card-content class="container">
            
            <div class="left">
                <form novalidate class="sign-up-form">

                    <md-field :class="{'md-invalid': errors.has('login')}">
                        <label>Логин</label>
                        <md-input required
                            maxlength="15"
                            name="login"
                            v-model="input.login"
                            data-vv-as="логин"
                            data-vv-name="login"
                            v-validate="'required|alpha_num|min:3|max:15'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('login')}}</span>
                    </md-field>
                    
                    <md-field md-has-password :class="{'md-invalid': errors.has('password')}">
                        <label>Пароль</label>
                        <md-input required
                            type="password" 
                            name="password"
                            v-model="input.password"
                            data-vv-as="пароль"
                            data-vv-name="password"
                            v-validate="'required|alpha_dash|min:5|max:20'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('password')}}</span>
                    </md-field>

                    <md-field md-has-password :class="{'md-invalid': errors.has('password_confirm')}">
                        <label>Подтверждение пароля</label>
                        <md-input required
                            type="password"
                            name="password_confirm"
                            v-model="password_confirm"
                            data-vv-as="пароль"
                            data-vv-name="password_confirm"
                            v-validate="'confirmed:password'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('password_confirm')}}</span>
                    </md-field>

                    <md-field :class="{'md-invalid': errors.has('name')}">
                        <label>Имя</label>
                        <md-input required
                            maxlength="15"
                            name="name"
                            v-model="input.name"
                            data-vv-as="имя"
                            data-vv-name="name"
                            v-validate="'required|alpha_spaces|min:3|max:15'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('name')}}</span>
                    </md-field>

                    <md-field :class="{'md-invalid': errors.has('surname')}">
                        <label>Фамилия</label>
                        <md-input required
                            maxlength="15"
                            name="surname"
                            v-model="input.surname"
                            data-vv-as="фамилия"
                            data-vv-name="surname"
                            v-validate="'required|alpha_spaces|min:3|max:15'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('surname')}}</span>
                    </md-field>

                    <md-field :class="{'md-invalid': errors.has('phone')}">
                        <md-icon>phone</md-icon>
                        <label>Номер телефона</label>
                        <md-input required
                            maxlength="11"
                            name="phone"
                            v-model="input.phone"
                            data-vv-as="номер телефона"
                            data-vv-name="phone"
                            v-validate="'regex:^[7-8][0-9]{10}$'"
                            @keyup.enter.native="signUp"
                            ></md-input>
                            <span class="md-error">{{errors.first('phone')}}</span>
                    </md-field>

                    <md-field md-clearable>
                        <md-icon>insert_photo</md-icon>
                        <label>Аватар</label>
                        <md-file
                            id="avatar"
                            type="file" 
                            accept="image/*"
                            @input="onFileChange"></md-file>
                    </md-field>
                    
                </form>
            </div>
            
            <md-card-media md-medium md-ratio="4:3" class="right">
                <img :src="image_preview" alt="Avatar">
            </md-card-media>
        </md-card-content>
        
        <md-card-actions>
            <md-button @click="$router.push('/')">Отмена</md-button>
            <md-button class="md-raised md-primary" @click="signUp">Регистрация</md-button>
        </md-card-actions>
    </md-card>

    <md-dialog-alert
        md-content="Вы были успешно зарегистрированы."
        md-confirm-text="Ок"
        @md-closed="onDialogClosed"
        :md-active.sync="show_dialog">
    </md-dialog-alert>

    <md-snackbar md-theme="default" md-position="center" :md-duration="4000" :md-active.sync="show_snackbar" md-persistent>
        <span>{{snackbar_message}}</span>
        <md-button class="md-accent" @click="show_snackbar = false">Закрыть</md-button>
    </md-snackbar>
</div>
</template>

<script>

import api from '@/API'
import auth from '@/auth'
import {EventBus} from '@/EventBus'

export default {
    name: 'sign-up',
    data() {
        return {
            input: {
                login: '',
                password: '',
                name: '',
                surname: '',
                phone: '',
                avatar: ''
            },
            snackbar_message: '',
            show_snackbar: false,
            show_dialog: false,
            password_confirm: '',
            file: null,
            image_preview: 'https://www.drupal.org/files/issues/default-avatar.png'
        }
    },
    created() {
        EventBus.$on('user-sign-in', this.userSignInListener);
    },
    destroyed() {
        EventBus.$off('user-sign-in', this.userSignInListener);
    },
    methods: {
        userSignInListener() {
            this.$router.push('/');
        },

        onDialogClosed() {
            this.$router.push('/');
        },

        onFileChange(e) {
            this.file = document.getElementById('avatar').files[0];

            let image = new Image();
            let reader = new FileReader();

            reader.onload = (e) => {
                this.image_preview = e.target.result;
            };

            reader.readAsDataURL(this.file);
        },

        async signUp() {
            if (!await this.$validator.validate()) {
                this.snackbar_message = 'Введите данные корректно';
                this.show_snackbar = true;
                return;
            }

            EventBus.$emit('global-loading-start');
            try {
                if (this.file !== null) {
                    this.input.avatar = await api.uploadImage(this.file);
                }
                
                await auth.signUp(this.input);
                this.show_dialog = true;
            } catch (error) {
                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }

                if (error.response.status === 400) {
                    this.snackbar_message = 'Введите данные корректно';
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
.container {
    display: flex;
    flex-flow: row wrap;
}

.left {
    flex-basis: 50%;
}

.right {
    flex-basis: 50%;
}

.sign-up-form {
    width: 80%;
    margin: 0 auto;
}
</style>
