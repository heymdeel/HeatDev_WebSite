<template>
<div>
    <md-empty-state v-if="error" v-show="error"
        :md-icon="error.icon"
        :md-label="error.title"
        :md-description="error.description">
        <md-button class="md-primary md-raised" @click="$router.push(error.link)">{{error.link_text}}</md-button>
    </md-empty-state>
    <div v-else>
        <md-card class="review-card">
            <md-card-header>
                <div class="md-title">Оцените выполнение заказа</div>
            </md-card-header>

            <md-card-content>

                <div class="rating-form">
                    <md-field :class="{'md-invalid': errors.has('text')}">
                        <label>Комментарий</label>
                        <md-textarea
                            required
                            maxlength="1500"
                            v-model="review.text"
                            data-vv-as="комментарий"
                            data-vv-name="text"
                            v-validate="'required|min:10|max:1500'">

                        </md-textarea>
                        <md-icon>description</md-icon>
                        <span class="md-error">{{errors.first('text')}}</span>
                    </md-field>

                    <star-rating :show-rating="false" v-model="review.rating" :border-width="1" class="stars"></star-rating>
                </div>

                <md-card-actions>
                    <md-button class="md-raised md-primary" @click="sendReview">Отправить</md-button>
                </md-card-actions>
                
            </md-card-content>
        </md-card>
    </div>

    <md-dialog-alert
        md-content="Отзыв успешно оставлен. Вы будете перенаправлены к главной странице."
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
import auth from '@/auth'
import api from '@/API'
import {EventBus} from '@/EventBus'

import StarRating from 'vue-star-rating'

export default {
    name: 'create-review',
    components: {
        'star-rating': StarRating
    },
    data() {
        return {
            user: auth.user,
            review: {
                order_id: this.$route.params.id,
                rating: 0,
                text: '',
            },
            error: null,
            show_snackbar: false,
            snackbar_message: '',
            show_dialog: false
        }
    },

    created() {
        EventBus.$on('user-sign-out', this.signOutReviewListener);
        if (!this.user.is_authenticated) {
            this.showAuthError();
            return;
        }
    },
    destroyed() {
        EventBus.$off('user-sign-out', this.signOutReviewListener);
    },
    methods: {
        signOutReviewListener() {
            this.$roter.push('/');
        },

        onDialogClosed() {
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

        async sendReview() {
            if (!await this.$validator.validate() || this.review.rating === 0) {
                this.snackbar_message = 'Заполните все данные';
                this.show_snackbar = true;
                return;
            }

            try {
                this.error = null;
                EventBus.$emit('global-loading-start');
                await api.sendReview(this.review);
                this.show_dialog = true;
            } catch (error) {
                const status = error.response.status;
                
                if (status === 502 || status === 504) {
                    this.snackbar_message = 'Ошибка доступа к серверу';
                    this.show_snackbar = true;
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
                EventBus.$emit('global-loading-finish');
            }
        }
    }
}
</script>

<style scoped>
.review-card {
    margin: 0 auto;
    width: 70%;
}

.rating-form {
    display: flex;
    flex-flow: column wrap;
    margin: 0 auto;
    width: 80%;
}

.stars {
    margin: 0 auto;
    margin-top: 15px;
    margin-bottom: 15px;
}
</style>
