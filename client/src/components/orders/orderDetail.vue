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
        <md-card v-else v-show="order" class="order-details-card">
            <md-card-header>
                <div class="md-title">Информация о заявке</div>
            </md-card-header>

            <md-card-content>
                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Заказчик: 
                    </div>

                    <div class="md-body-2 right-item">
                        {{order.client.name}} {{order.client.surname}}
                        <br>
                        <md-icon>phone</md-icon>{{order.client.phone}}
                        <br>
                        {{order.address}}
                    </div>
                </div>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Модель оборудования: 
                    </div>

                    <div class="md-body-2 right-item">
                        {{categoryName(order.category)}}
                    </div>
                </div>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Дата проведения диагностики: 
                    </div>

                    <div class="md-body-2 right-item">
                        <md-icon>date_range</md-icon>
                        {{formatDate(order.visit_time)}}
                    </div>
                </div>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Дата оформления заявки: 
                    </div>

                    <div class="md-body-2 right-item">
                        <md-icon>date_range</md-icon>
                        {{formatDate(order.beginning_time)}}
                    </div>
                </div>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Статус: 
                    </div>

                    <div class="md-body-2 right-item" :style="statusColour">
                        {{statusName}}
                    </div>
                </div>

                <md-divider></md-divider>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Стоимость диагностики: 
                    </div>

                    <div class="md-body-2 right-item">
                        {{order.diagnostic_price}}
                    </div>
                </div>

                <div class="flex-list">
                    <div class="md-subheading left-item">
                        Стоимость работ: 
                    </div>

                    <div class="md-body-2 right-item">
                        {{order.price}}
                    </div>
                </div>
            </md-card-content>

            <md-card-actions>
                <md-button class="md-raised md-accent" v-show="canDecline" @click="declineHandler">Отмена</md-button>
                <md-button class="md-raised md-primary" v-show="canAccept" @click="acceptHandler">{{acceptText}}</md-button>
                <md-button class="md-raised md-primary" v-show="canChangePrice" @click="priceHandler">стоимость работ</md-button>
            </md-card-actions>
        </md-card>
    </div>

    <md-dialog-confirm
        :md-active.sync="show_decline_dialog"
        md-title="Вы действительно хотите отменить заявку?"
        md-content="Данные действие необратимо."
        md-confirm-text="Да"
        md-cancel-text="Нет"
        @md-confirm="changeStatus" />

    <md-dialog-confirm
        :md-active.sync="show_accept_dialog"
        md-title="Вы действительно хотите продолжить?"
        md-confirm-text="Да"
        md-cancel-text="Нет"
        @md-confirm="changeStatus" />

    <md-dialog-prompt
        :md-active.sync="show_price_dialog"
        v-model="order.price"
        type="number"
        md-title="Введите стоимость работ."
        md-confirm-text="Ок" 
        md-cancel-text="Отмена"
        @md-confirm="changePrice"/>

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
            categories: [],
            error: null,
            loading: false,
            show_snackbar: false,
            snackbar_message: '',
            show_decline_dialog: false,
            show_accept_dialog: false,
            show_price_dialog: false,
            new_status: 0
        }
    },
    async created() {
        EventBus.$on('user-sign-in', this.loadOrderDetails);
        EventBus.$on('user-sign-out', this.signOutListener);

        if (!this.user.is_authenticated) {
            this.showAuthError();
            return;
        };

        await this.loadOrderDetails();
    },
    destroyed() {
        EventBus.$off('user-sign-in', this.loadOrderDetails);
        EventBus.$off('user-sign-out', this.signOutListener);
    },
    computed: {
        acceptText() {
            const statuses = {
                0: 'Подтвердить заявку',
                1: 'Начать диагностику',
                2: 'Начать выполнение работ',
                3: 'Завершить заказ'
            };

            return statuses[this.order.status];
        },

        canChangePrice() {
            return auth.userIsWorker() && [2, 3].includes(this.order.status);
        },

        canAccept() {
            return auth.userIsWorker() && [0, 1, 2, 3].includes(this.order.status);
        },

        canDecline() {
            if (auth.userIsClient() && [0, 1].includes(this.order.status)) {
                return true;
            }

            if (auth.userIsWorker() && [0, 1, 2, 3].includes(this.order.status)) {
                return true;
            }

            return false;
        },

        statusColour() {
            const colours = {
                0: 'black',
                1: 'black',
                2: 'blue',
                3: 'blue',
                4: 'green',
                5: 'red',
                6: 'red'
            };

            return { color: colours[this.order.status]};
        },

        statusName() {
            const names = {
                0: 'Ожидание подтверждения',
                1: 'Ожидание диагностики',
                2: 'Диагностика',
                3: 'Выполняются ремонтные работы',
                4: 'Завершен',
                5: 'Отменен',
                6: 'Отменен'
            };

            return names[this.order.status];
        }
    },
    methods: {
        signOutListener() {
            console.log('kke2');
            this.$router.push('/');
        },

        acceptHandler() {
            if (this.order.status in [0, 1, 2, 3]) {
                this.new_status = this.order.status + 1;
            };

            this.show_accept_dialog = true;
        },

        declineHandler() {
            if (auth.userIsWorker()) {
                this.new_status = 6;
            };

            if (auth.userIsClient()) {
                this.new_status = 5;
            }

            this.show_decline_dialog = true;
        },

        priceHandler() {
            this.show_price_dialog = true;
        },

        formatDate(date) {
            const now = new Date();
            const new_date = new Date(date);

            const options = {
                month: 'long',
                day: 'numeric',
                year: 'numeric'
            };
            
            return new_date.toLocaleString("ru", options);
        },

        categoryName(id) {
            return this.categories.find(c => c.id == id).description;
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

        handleStatus(status) {
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

                if (status === 404) {
                    this.snackbar_message = 'Заявка не найдена';
                    this.show_snackbar = true;
                    return;
                }

                if (status === 400) {
                    this.snackbar_message = 'Неверный статус заявки';
                    this.show_snackbar = true;
                    return;
                }
        },

        async changePrice() {
            try {
                EventBus.$emit('global-loading-start');
                await api.changePrice(this.order.id, this.order.price);
            } catch (error) {
                const status = error.response.status;
                this.handleStatus(status);
            } finally {
                EventBus.$emit('global-loading-finish');
            }
        },

        async changeStatus() {
            try {
                EventBus.$emit('global-loading-start');
                await api.changeOrderStatus(this.order.id, this.new_status);
                this.order.status = this.new_status;
            } catch (error) {
                const status = error.response.status;
                this.handleStatus(status);
            } finally {
                EventBus.$emit('global-loading-finish');
            }
        },

        async loadOrderDetails() {
            try {
                this.error = null;
                this.loading = true;
                this.order = await api.getOrderDetails(this.$route.params.id);
                this.categories = await api.getCategories();
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
.order-details-card {
    width: 60%;
    margin: 0 auto;
}

.flex-list {
    display: flex;
    flex-flow: row wrap;
    justify-content: space-between;
    align-items: center;
    margin: 0 auto;
    width: 70%;
    margin-top: 10px;
    margin-bottom: 10px;
}

.left-item {
    flex-basis: 50%;
    text-align: left;
}

.right-item {
    flex-basis: 50%;
    text-align: right;
}
</style>
