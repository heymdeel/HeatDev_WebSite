import axios from 'axios'
import decode from 'jwt-decode'
import {EventBus} from '@/EventBus'

export default {
    user: {
        is_authenticated: false,
        access_token: '',
        refresh_token: '',
        profile: {
          name: '',
          surname: '',
          phone: '',
          avatar: '',
        },
        roles: [],
        id: 0
      },

      async signIn(credentials) {
        try {
          const token_response = await axios.post('/api/auth/sign_in', credentials);          
          this.readUserData(token_response);
          axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;

          const profile_response = await axios.get(`/api/users/${this.user.id}/profile`);
          this.user.profile = profile_response.data;

          this.storeUserData();
        } catch (error) {
            throw error;
        }
      },

      async signOut() {
        try {
          await axios.delete(`/api/auth/token/${this.user.refresh_token}`);

          this.clearAuth();
        } catch (error) {
          const status = error.response.status;
            if (status === 400 || status === 404) {
              this.clearAuth();
              return;
            }

            throw error;
        }
      },

      async signUp(user) {
        try {
          const token_response = await axios.post('/api/auth/sign_up', user);
          this.readUserData(token_response);
          axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;

          const profile_response = await axios.get(`/api/users/${this.user.id}/profile`);
          this.user.profile = profile_response.data;

          this.storeUserData();
        } catch (error) {
          throw error;
        }
      },

      readUserData(token_response) {
        this.user.access_token = token_response.data.access_token;
        this.user.refresh_token = token_response.data.refresh_token;
        this.user.roles = token_response.data.roles;
        this.user.id = token_response.data.user_id;
      },

      storeUserData() {
        this.user.is_authenticated = true;
        localStorage.setItem('user', JSON.stringify(this.user));       
      },

      clearAuth() {
        axios.defaults.headers.common["Authorization"] = '';
        this.user.is_authenticated = false;
        localStorage.removeItem('user');
      },

      async refreshTokens() {
        try {
          const response = await axios.post(`/api/auth/token/${this.user.refresh_token}`);
          this.readUserData(response);

        } catch (error) {
          throw error;
        }
      },

      async checkTokens() {
        if (!this.tokenisValid(this.user.refresh_token)) {
          this.clearAuth();
          EventBus.$emit('user-sign-out')
          return false;
        }

        if (!this.tokenisValid(this.user.access_token)) {
          try {
            await this.refreshTokens();

            // update auth data only if we got new tokens from server
            axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;
            this.storeUserData();
          } catch (error) {
            console.log(error);
            
            const status = error.response.status;
            if (status === 400 || status === 404) {
              this.clearAuth();
              EventBus.$emit('user-sign-out')
              return false;
            }
          }
        }

        return true;
      },

      async init() {
        const user = localStorage.getItem('user');

        if (!user) {
          this.user.is_authenticated = false;
          return;
        }
        
        this.user = JSON.parse(user);
          
        if (!this.tokenisValid(this.user.refresh_token)) {
          this.clearAuth();
          return;
        }

        if (!this.tokenisValid(this.user.access_token)) {
          try {
            await this.refreshTokens();
          } catch (error) {
            console.log(error);
            
            const status = error.response.status;
            if (status === 400 || status === 404) {
              this.clearAuth();
              return;
            }
          }
        }
        
        // auth data is still empty. no matter if we have new tokens or old tokens
        axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;
        this.storeUserData();
      },

      tokenisValid(tokenString) {
        const token = decode(tokenString);
        var current_time = Date.now() / 1000;
        
        if (token.exp < current_time)
          return false;
        else 
          return true;
      },

      userIsWorker() {
        return this.user.is_authenticated && this.user.roles.some(r => r == 'worker');
      }
}