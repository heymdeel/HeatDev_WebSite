<template>
<div>
    <md-empty-state v-if="error" v-show="error"
        :md-icon="error.icon"
        :md-label="error.title"
        :md-description="error.description">
        <md-button class="md-primary md-raised" @click="$router.push(error.link)">{{error.link_text}}</md-button>
    </md-empty-state>
    <div v-else>
        <md-progress-spinner v-if="loading" class="md-accent" :md-stroke="3" md-mode="indeterminate"></md-progress-spinner>
        <div v-else>
            {{order.address}} {{order.client.phone}}  {{order.client.name}}
        </div>
    </div>

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
    name: 'order-detail',
    data() {
        return {
            user: auth.user,
            order: null,
            error: null,
            loading: false,
            show_snackbar: false,
            snackbar_message: ''
        }
    },
    async created() {
        if (!this.user.is_authenticated) {
            this.showAuthError();
            return;
        };

        await this.loadOrderDetails();

        EventBus.$on('user-sign-in', this.loadOrderDetails);
        EventBus.$on('user-sign-out', this.signOutListener);
    },
    destroyed() {
        EventBus.$off('user-sign-in', this.loadOrderDetails);
        EventBus.$on('user-sign-out', this.signOutListener);
    },
    methods: {
        signOutListener() {
            this.$router.push('/');
        },

        showAuthError() {
            this.error = {
                title: "Необходима авторизация",
                description: "Для продолжения Вам необходимо войти в свой аккаунт. Если Вы ещё не зарегистрированы, то можете это сделать на странице регистрации.",
                link: "/sign_up",
                link_text: "Зарегистрироваться",
                icon: "lock"
            };
        },

        async loadOrderDetails() {
            try {
                this.loading = true;
                this.order = await api.getOrderDetails(this.$route.params.id);
                this.error = null;
            } catch (error) {
                const status = error.response.status;

                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
                    this.error = {
                        title: "Ошибка доступа к серверу",
                        description: "Возникла ошибка при подключении к серверу. Вы можете обновить страницу или вернуться на главную.",
                        link: "/",
                        link_text: "На главную",
                        icon: "error"
                    };
                    return;
                }

                if (status === 401 || status === 403) {
                    this.snackbar_message = 'Ошибка авторизации';
                    this.show_snackbar = true;
                    this.showAuthError();

                    return;
                }

                if (status === 404) {
                    this.error = {
                        title: "Информация о заявке не найдена",
                        description: "Информация о данной заявке не была найдена. Возможно, вы ошиблись страницей, или заявка была удалена.",
                        link: "/orders",
                        link_text: "К заявкам",
                        icon: "search"
                    };
                    return;
                }
            } finally {
                this.loading = false;
            }
        }
    }
}
</script>

<style scoped>

</style>
