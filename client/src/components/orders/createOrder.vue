<template>
<div>
    <md-progress-spinner v-if="loading" class="md-accent" :md-stroke="3" md-mode="indeterminate"></md-progress-spinner>

    <md-empty-state v-else-if="!user.is_authenticated" v-show="!user.is_authenticated"
        md-icon="lock"
        md-label="Необходима авторизация"
        md-description="Для продолжения Вам необходимо войти в свой аккаунт. Если Вы ещё не зарегистрированы, то можете это сделать на странице регистрации.">
      <md-button class="md-primary md-raised" @click="$router.push('/sign_up')">Зарегистрироваться</md-button>
    </md-empty-state>

    <md-card v-else class="order-card">
        <md-card-header>
            <md-card-header-text>
                <div class="md-title">Оформление заявки на ремонт</div>
            </md-card-header-text>
        </md-card-header>

        <md-card-content>
            <form novalidate class="order-form">
                
                <md-field :class="{'md-invalid': errors.has('category')}">
                    <md-select v-model="order.category" name="category" placeholder="Модель оборудования" required>
                        <md-option v-for="c in categories" :key="c.id" :value="c.id">{{c.description}}</md-option>
                    </md-select>
                </md-field>

                <md-field :class="{'md-invalid': errors.has('address')}">
                    <label>Адрес</label>
                    <md-input required
                        maxlength="75"
                        name="address"
                        v-model="order.address"
                        data-vv-as="адрес"
                        data-vv-name="address"
                        v-validate="'required|min:5|max:75'"
                        ></md-input>
                    <span class="md-error">{{errors.first('address')}}</span>
                </md-field>

                <md-datepicker v-model="order.visit_date" md-immediately>
                    <label>Дата проведения дианостики</label>
                </md-datepicker>
            </form>

            <md-card-actions>
                <md-button @click="$router.push('/orders')">Отмена</md-button>
                <md-button class="md-primary md-raised" @click="createOrder">Ок</md-button>
            </md-card-actions>
        </md-card-content>
    </md-card>

    <md-dialog-alert
        md-content="Заявка успешно оставлена. Вы будете перенаправлены к странице заявки."
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
import {EventBus} from '@/EventBus'
import auth from '@/auth'

export default {
    name: 'create-order',
    data() {
        return {
            order: {
                address: '',
                category: null,
                visit_date: null
            },
            user: auth.user,
            snackbar_message: '',
            order_id: -1,
            show_snackbar: false,
            show_dialog: false,
            loading: false,
            categories: []
        }
    },
    async created() {
        EventBus.$on('user-sign-out', this.signOutListener);

        await this.loadCategories();
    },
    destroyed() {
        EventBus.$off('user-sign-out', this.signOutListener);
    },
    methods: {
        signOutListener() {
            console.log('kke3');
            this.$router.push('/');
        },

        onDialogClosed() {
            this.$router.push(`/orders/${this.order_id}`);
        },

        async loadCategories() {
            try {
                this.loading = true;
                this.categories = await api.getCategories();
            } catch (error) {
                const status = error.response.status;

                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }
            } finally {
                this.loading = false;
            }
        },

        async createOrder() {
            if (!await this.$validator.validate() || this.order.category == null || this.order.visit_date == null) {
                this.snackbar_message = 'Введите данные корректно';
                this.show_snackbar = true;
                return;
            }
            
            try {
                EventBus.$emit('global-loading-start');
                const result = await api.createOrder(this.order);
                this.order_id = result.id;

                this.show_dialog = true;
            } catch (error) {
                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 401 || status === 403) {
                    this.snackbar_message = 'Ошибка авторизации';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 400) {
                    this.snackbar_message = 'Введите данные корректно';
                    this.show_snackbar = true;
                    return;
                }
            } finally {
                EventBus.$emit('global-loading-finish');
            }
        },
    }
}
</script>

<style scoped>
.order-card {
    width: 75%;
    margin: 0 auto;
}

.order-form {
    width: 80%;
    margin: 0 auto;
}
</style>
