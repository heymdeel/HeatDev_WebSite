<template>
<div>
    <md-card class="password-card">

        <md-card-header>
            <div class="md-title">Смена пароля</div>
        </md-card-header>
        
        <md-card-content>
            <form novalidate @submit.stop.prevent="submit" class="user-form">
                <md-field md-has-password :class="{'md-invalid': errors.has('old_password')}">
                    <label>Старый пароль</label>
                    <md-input required
                        type="password" 
                        name="old_password"
                        v-model="input.old_password"
                        data-vv-as="пароль"
                        data-vv-name="old_password"
                        v-validate="'required|alpha_dash|min:5|max:20'"
                        @keyup.enter.native="changePassword">
                    </md-input>
                    <span class="md-error">{{errors.first('old_password')}}</span>
                </md-field>

                <md-field md-has-password :class="{'md-invalid': errors.has('new_password')}">
                    <label>Новый пароль</label>
                    <md-input required
                        type="password" 
                        name="new_password"
                        v-model="input.new_password"
                        data-vv-as="новый пароль"
                        data-vv-name="new_password"
                        v-validate="'required|alpha_dash|min:5|max:20'"
                        @keyup.enter.native="changePassword">
                        </md-input>
                        <span class="md-error">{{errors.first('new_password')}}</span>
                </md-field>

                <md-field md-has-password :class="{'md-invalid': errors.has('password_confirm')}">
                    <label>Подтверждение пароля</label>
                    <md-input required
                        type="password"
                        name="password_confirm"
                        v-model="password_confirm"
                        data-vv-as="пароль"
                        data-vv-name="password_confirm"
                        v-validate="'confirmed:new_password'"
                        @keyup.enter.native="changePassword">
                    </md-input>
                    <span class="md-error">{{errors.first('password_confirm')}}</span>
                </md-field>
            </form>
        </md-card-content>

        <md-card-actions>
            <md-button class="md-primary" @click="leavePage">Отмена</md-button>
            <md-button class="md-primary md-raised" @click="changePassword">Сохранить</md-button>
        </md-card-actions>        
    </md-card>        

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
    name: 'change-password',
    props: {
        'user': Object
    },
    data() {
        return {
            input: {
                old_password: '',
                new_password: ''
            },
            password_confirm: '',
            snackbar_message: '',
            show_snackbar: false
        }
    },
    methods: {
        leavePage() {
            this.$emit('page-leave');
        },

        async changePassword() {
            if (!await this.$validator.validate()) {
                this.snackbar_message = 'Введите данные корректно';
                this.show_snackbar = true;
                return;
            }

            try {    
                EventBus.$emit('global-loading-start');
                await auth.changePassword(this.user.id, this.input);

                this.$emit('password-changed');
                this.leavePage();
            } catch (error) {
                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 401 || status === 403) {
                    this.snackbar_message = 'У вас недостаточно прав для совершения данного действия';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 404) {
                    this.snackbar_message = 'Ресурс не найден. Перезагрузите страницу';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 400) {
                    this.snackbar_message = 'Введите данные корректно!';
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
.user-form {
    width: 70%;
    margin: 0 auto;
    margin-bottom: 20px;    
}

.password-card {
    margin: 0 auto;
    width: 70%;
}
</style>
