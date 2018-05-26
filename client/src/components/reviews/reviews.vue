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

        <div v-else-if="reviews" class="reviews-container">
            <md-card
                md-with-hover 
                class="reviews-card"
                v-for="review in reviews" 
                :key="review.id">

                <md-card-header style="{margin-left: 0;}">
                    <md-avatar>
                        <img :src="review.client_profile.avatar" alt="client">
                    </md-avatar>

                    <md-card-header-text>
                        <div class="md-title">{{review.client_profile.name + ' ' + review.client_profile.surname}}</div>
                    </md-card-header-text>
                </md-card-header>

                <md-card-content class="card-content">
                    <div> {{review.text}} </div>

                    <star-rating 
                        :show-rating="false" 
                        read-only 
                        v-model="review.rating" 
                        :star-size="40"
                        :border-width="1" 
                        class="stars">
                    </star-rating>
                </md-card-content>
            </md-card>
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
import {EventBus} from '@/EventBus'

import StarRating from 'vue-star-rating'

export default {
    name: 'reviews',
    components: {
        'star-rating': StarRating
    },
    data() {
        return {
            reviews: [],
            error: null,
            loading: false,
            snackbar_message: '',
            show_snackbar: false
        }
    },
    async created() {
        await this.loadReviews();
    },
    methods: {
        async loadReviews() {
            this.error = null;
            EventBus.$emit('global-loading-start');
            try {
                this.reviews = await api.getAllReviews();
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

                if (status === 404) {
                    this.error = {
                        title: "Отзывы не найдены",
                        description: "На данный момент отзывы отсутствуют. Вы можете оставить свой отзыв, если Вы пользовались услугами нашего сервиса, на странице заявки.",
                        link: "/",
                        link_text: "На главную",
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

<style>
.reviews-container {
    display: flex;
    flex-flow: row wrap;
    justify-content: space-between;
    align-items: center;
}

.reviews-card {
    flex-basis: 400px;
    margin: 0 auto;
    margin-top: 20px;
    margin-bottom: 20px;
    padding: 10px;   
}

.stars {
    margin-top: 15px;
}

.card-content {
    display: flex;
    flex-flow: column wrap;
    align-items: center;
}
</style>
