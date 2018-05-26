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
            const response = await axios.put(`/api/orders/${orderId}/status`, status, {
                headers: { 'Content-Type': 'application/json' }
              });
        } catch (error) {
            throw error;
        }        
    },

    async changePrice(orderId, price) {
        await auth.checkTokens();

        try {
            const response = await axios.put(`/api/orders/${orderId}/price`, price, {
                headers: { 'Content-type': 'application/json' }
            });
        } catch (error) {
            throw error;
        }
    },
    
    async getAllOrders() {
        await auth.checkTokens();

        try {
            const response = await axios.get('/api/orders');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    async sendReview(review) {
        await auth.checkTokens();

        try {
            await axios.post(`/api/reviews`, review);
        } catch (error) {
            throw error;
        }
    },

    async getClientOrders() {
        await auth.checkTokens();

        try {
            const response = await axios.get('/api/orders/my');
            return response.data;
        } catch (error) {
            throw error;
        }
    }
}