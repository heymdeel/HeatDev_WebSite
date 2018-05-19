<template>
<div>
    <auth-comp v-if="!user.authenticated"></auth-comp>
    <user-info v-else></user-info>
</div>
</template>

<script>
import AuthComp from './authComp'
import UserInfo from './userInfo'

import auth from '@/auth'

export default {
    name: 'user-box',
    components: {
        'user-info': UserInfo,
        'auth-comp': AuthComp
    },
    data() {
        return {
            user: auth.user
        }
    }
}
</script>

<style scoped>
</style>
