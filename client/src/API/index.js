import axios from 'axios'
import auth from '@/auth'

export default {
    // anonymous requests
    //========================================================================================
    async uploadImage(image) {
        let data = new FormData();
        data.append('image', image);

        try {
            const response = await axios.post('api/images', data);
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    async getCategories() {
        try {
            const result = await axios.get('/api/orders/categories');
            
            return result.data;
        } catch (error) {
            throw error;
        }
    },

    // authorized requests
    //========================================================================================
    async createOrder(order) {
        try {
            if (!auth.checkTokens()) {
                return;
            }
            
            const response = await axios.post('/api/orders/', order);
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    async getOrderDetails(id) {
        if (!auth.checkTokens()) {
            return;
        }

        try {
            const response = await axios.get(`/api/orders/${id}`);
            return response.data;    
        } catch (error) {
            throw error;
        }
    }
}