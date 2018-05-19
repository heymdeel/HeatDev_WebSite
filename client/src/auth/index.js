import axios from 'axios'

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
          this.user.access_token = token_response.data.access_token;
          this.user.refresh_token = token_response.data.refresh_token;
          this.user.roles = token_response.data.roles;
          this.user.id = token_response.data.user_id;

          axios.defaults.headers.common['Authorization'] = 'Bearer ' + this.user.access_token;
          this.user.authenticated = true;
          
          const profile_response = await axios.get(`/api/users/${this.user.id}/profile`);
          this.user.profile = profile_response.data;

          localStorage.setItem('user', JSON.stringify(this.user));       
        } catch (error) {
            throw error;
        }
      },

      async signOut() {
        try {
          await axios.delete(`/api/auth/token/${this.user.refresh_token}`);

          localStorage.removeItem('user');
          this.user.authenticated = false;
          axios.defaults.headers.common["Authorization"] = '';
        } catch (error) {
          throw error;
        }
      }
}