import axios from 'axios'
import decode from 'jwt-decode'
export default {
    user: {
        authenticated: false,
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

      async signIn(creds) {
        try {
          const token_response = await axios.post('/api/auth/sign_in', creds);          
          this.parseTokenResponse(token_response);
          this.saveAuth();

          const profile_response = await axios.get(`/api/users/${this.user.id}/profile`);
          this.user.profile = profile_response.data;
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

          this.parseTokenResponse(token_response);
          this.saveAuth();

          const profile_response = await axios.get(`/api/users/${this.user.id}/profile`);
          this.user.profile = profile_response.data;
        } catch (error) {
          throw error;
        }
      },

      parseTokenResponse(token_response) {
        this.user.access_token = token_response.data.access_token;
        this.user.refresh_token = token_response.data.refresh_token;
        this.user.roles = token_response.data.roles;
        this.user.id = token_response.data.user_id;
      },

      saveAuth() {
        axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;
        this.user.authenticated = true;
        localStorage.setItem('user', JSON.stringify(this.user));       
      },

      clearAuth() {
        axios.defaults.headers.common["Authorization"] = '';
        this.user.authenticated = false;
        localStorage.removeItem('user');
      },

      async refreshTokens(token) {
        try {
          const response = await axios.post(`/api/auth/token/${token}`);
          this.parseTokenResponse(response);

        } catch (error) {
          throw error;
        }
      },

      async checkAuth() {
        const user = localStorage.getItem('user');

        if (!user) {
          this.user.authenticated = false;
          return;
        }
        
        this.user = JSON.parse(user);
          
        if (!this.tokenisValid(this.user.refresh_token)) {
          this.clearAuth();
          return;
        }

        if (!this.tokenisValid(this.user.access_token)) {
          try {
            await this.refreshTokens(this.user.refresh_token);
          } catch (error) {
            console.log(error);
            
            const status = error.response.status;
            if (status === 400 || status === 404) {
              this.clearAuth();
              return;
            }
          }
        }

        this.saveAuth();
      },

      tokenisValid(tokenString) {
        const token = decode(tokenString);
        var current_time = Date.now() / 1000;
        if (token.exp < current_time)
          return false;
        else 
          return true;
      }
}