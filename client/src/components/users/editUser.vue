<template>
<div>
    <md-card class="edit-profile-card">
        <md-card-header>
            <div class="md-title">Изменение данных профиля</div>
        </md-card-header>
        
        <md-card-content>
            <form novalidate @submit.stop.prevent="submit" class="user-form">
                <md-field :class="{'md-invalid': errors.has('name')}">
                    <label>Имя</label>
                    <md-input 
                        required 
                        maxlength="15"
                        name="name"
                        data-vv-as="имя"
                        data-vv-name="name"
                        v-model="input.name" 
                        v-validate="'required|alpha_spaces|min:3|max:15'"
                        @keyup.enter.native="editProfile">
                    </md-input>
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
                        @keyup.enter.native="editProfile">
                    </md-input>
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
                        @keyup.enter.native="editProfile">
                    </md-input>
                    <span class="md-error">{{errors.first('phone')}}</span>
                </md-field>

                <md-field md-clearable>
                    <label>Аватар</label>
                    <md-file
                        id="image" 
                        type="file" 
                        accept="image/*"
                        @input="onFileChange">
                    </md-file>
                </md-field>
            </form>
        </md-card-content>
        
        <md-card-actions>
            <md-button class="md-primary" @click="leavePage">Отмена</md-button>
            <md-button class="md-primary md-raised" @click="editProfile">Сохранить</md-button>
        </md-card-actions>
    </md-card>

    <md-snackbar md-theme="default" md-position="center" :md-duration="4000" :md-active.sync="show_snackbar" md-persistent>
        <span>{{snackbar_message}}</span>
        <md-button class="md-accent" @click="show_snackbar = false">Закрыть</md-button>
    </md-snackbar>
</div>  
</template>

<script>
import api from '@/API'
import {EventBus} from '@/EventBus'

export default {
    name: 'edit-user',
    props: {
        'user': Object
    },
    data() {
        return {
            input: {
                name: '',
                surname: '',
                phone: '',
                avatar: ''
            },
            snackbar_message: '',
            show_snackbar: false,
            file: null
        }
    },
    created() {
        this.input.name = this.user.profile.name;
        this.input.surname = this.user.profile.surname;
        this.input.phone = this.user.profile.phone;
        this.input.avatar = this.user.profile.avatar;
    },
    methods: {
        onFileChange(e) {
            this.file = document.getElementById('image').files[0];
        },

        leavePage() {
            this.$emit('page-leave');
        },

        async editProfile() {
            if (!await this.$validator.validate()) {
                this.snackbar_message = 'Введите данные корректно';
                this.show_snackbar = true;
                return;
            }

            if (this.file) {
                this.input.avatar = await api.uploadImage(this.file);
            }

            try {
                EventBus.$emit('global-loading-start');
                const response = await api.updateUserProfile(this.user.id, this.input);

                this.$emit('profile-edited', this.input);
                this.leavePage();      
            } catch (error) {
                const status = error.response.status;
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

.edit-profile-card {
    margin: 0 auto;
    width: 70%;
}
</style>
