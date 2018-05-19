<template>
  <div id="app">
    <spinner v-if="global_loading"></spinner>
    <head-bar></head-bar>
    <router-view/>
  </div>
</template>

<script>
import HeadBar from '@/components/toolbar/headBar'
import Spinner from '@/components/spinner'

import {EventBus} from '@/EventBus'

export default {
  name: 'App',
  components: {
    'head-bar': HeadBar,
    'spinner': Spinner
  },
  data() {
    return {
      global_loading: false
    }
  },
  created() {
    EventBus.$on('global-loading-start', this.loadingStartListener);
    EventBus.$on('global-loading-finish', this.loadingFinishListener);
  },
  destroyed() {
    EventBus.$off('global-loading-start', this.loadingStartListener);
    EventBus.$off('global-loading-finish', this.loadingFinishListener);
  },
  methods: {
    loadingStartListener() {
      this.global_loading = true;
    },

    loadingFinishListener() {
      this.global_loading = false;
    }
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 0px;
  
}

</style>
