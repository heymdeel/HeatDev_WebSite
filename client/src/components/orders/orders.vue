<template>
<div>
    <div v-if="user.is_authenticated">
        <worker-orders v-if="isWorker"></worker-orders>
        <md-empty-state v-else
            md-icon="assignment"
            md-label="Оформите заявку на ремонт"
            md-description="Заполните форму заявки, и мы поможем Вам отремонтировать Вашу технику.">
        <md-button class="md-primary md-raised" @click="$router.push('/orders/create')">Оставить заявку</md-button>
        </md-empty-state>
    </div>

    <md-empty-state v-else v-show="!user.is_authenticated"
        md-icon="lock"
        md-label="Необходима авторизация"
        md-description="Для продолжения Вам необходимо войти в свой аккаунт. Если Вы ещё не зарегистрированы, то можете это сделать на странице регистрации.">
      <md-button class="md-primary md-raised" @click="$router.push('/sign_up')">Зарегистрироваться</md-button>
    </md-empty-state>
</div>
</template>

<script>
import WorkerOrders from '@/components/orders/workerOrders'

import auth from '@/auth'

export default {
    name: 'orders',
    components: {
        'worker-orders': WorkerOrders
    },
    data() {
        return {
            user: auth.user
        }
    },
    computed: {
        isWorker() {
            return auth.userIsWorker();
        }
    }
}
</script>

<style scoped>

</style>
