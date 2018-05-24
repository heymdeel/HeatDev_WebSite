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
            await auth.checkTokens();
            
            const response = await axios.post('/api/orders/', order);
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    async getOrderDetails(id) {
        await auth.checkTokens();

        try {
            const response = await axios.get(`/api/orders/${id}`);
            return response.data;    
        } catch (error) {
            throw error;
        }
    },

    async changeOrderStatus(orderId, status) {
        await auth.checkTokens();

        try {
            const response = await axios.put(`/api/orders/${orderId}/status/${status}`);
        } catch (error) {
            throw error;
        }        
    }
}