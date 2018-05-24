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
        <div v-else class="orders-container">
            <md-card 
            class="order-card"
            v-for="order in sortedOrders" 
            :key="order.id">

                <md-card-header style="{margin-left: 0;}">
                    <md-avatar>
                        <img :src="order.client.avatar" alt="client">
                    </md-avatar> 

                    <md-card-header-text>
                        <div class="md-title">{{categoryName(order.category)}}</div>
                        <div class="md-subhead">
                        Заказчик: 
                        <span class="link">
                            {{order.client.name + ' ' + order.client.surname}}
                        </span>
                        </div>    
                    </md-card-header-text>
                </md-card-header>

                <md-divider></md-divider>

                <md-card-content>
                    <div class="flex-list">
                        <div class="md-subheading left-item">
                            Дата оформления: 
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

                        <div class="md-body-2 right-item" :style="statusColour(order.status)">
                            {{statusName(order.status)}}
                        </div>
                    </div>

                    <div class="flex-list">
                        <div class="md-subheading left-item">
                            Адрес: 
                        </div>

                        <div class="md-body-2 right-item">
                            {{order.address}}
                        </div>
                    </div>

                    <div class="flex-list">
                        <div class="md-subheading left-item">
                            Контакнтый номер: 
                        </div>

                        <div class="md-body-2 right-item">
                            <md-icon>phone</md-icon>
                            {{order.client.phone}}
                        </div>
                    </div>
                </md-card-content>

                <md-card-actions>
                    <md-button class="md-primary md-raised" @click="goToOrder(order.id)">Перейти к заявке</md-button>
                </md-card-actions>
            </md-card>
        </div>
    </div>
</div>
</template>

<script>
import auth from '@/auth'
import api from '@/API'
import {EventBus} from '@/EventBus'

export default {
    name: 'worker-orders',
    data() {
        return {
            user: auth.user,
            orders: [],
            categories: [],
            error: null,
            loading: false,
            snackbar_message: '',
            show_snackbar: false
        }
    },
    async created() {
        EventBus.$on('user-sign-in', this.loadOrderDetails);

        if (!this.user.is_authenticated) {
            this.showAuthError();
            return;
        };

        await this.loadOrders();
    },
    destroyed() {
        EventBus.$off('user-sign-in', this.loadOrderDetails);
    },
    computed: {
        sortedOrders() {
            const comparator = (a, b) => {
                if (a.status < b.status) {
                    return -1;
                };

                if (a.status > b.status) {
                    return 1;
                };

                return 0;
            };

            return this.orders.sort(comparator);
        }
    },
    methods: {
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

        statusColour(status) {
            const colours = {
                0: 'black',
                1: 'black',
                2: 'blue',
                3: 'blue',
                4: 'green',
                5: 'red',
                6: 'red'
            };

            return { color: colours[status]};
        },

        statusName(status) {
            const names = {
                0: 'Ожидание подтверждения',
                1: 'Ожидание диагностики',
                2: 'Диагностика',
                3: 'Выполняются ремонтные работы',
                4: 'Завершен',
                5: 'Отменен',
                6: 'Отменен'
            };

            return names[status];
        },

        categoryName(id) {
            return this.categories.find(c => c.id == id).description;
        },

        goToOrder(id) {
            this.$router.push(`orders/${id}`);
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

        async loadOrders() {
            try {
                this.error = null;
                this.loading = true;
                this.orders = await api.getAllOrders();
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
                        title: "Заявки отсутствуют.",
                        description: "На данный момент заявки остутсвуют. Вы можете зайти на данную страницу позже.",
                        link: "/",
                        link_text: "На главную",
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
.orders-container {
    display: flex;
    flex-flow: row wrap;
    justify-content: space-between;
    align-items: center;
}

.order-card {
    flex-basis: 400px;
    margin: 0 auto;
    margin-top: 20px;
    margin-bottom: 20px;
    padding: 10px;   
}

.link {
  color: #820000 !important;
  font-weight: bold;
}

.flex-list {
    display: flex;
    flex-flow: row wrap;
    margin: 0 auto;
    width: 80%;
    justify-content: space-around;
    align-items: center;
    margin-top: 15px;
    margin-bottom: 15px;
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
