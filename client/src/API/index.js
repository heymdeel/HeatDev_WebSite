import axios from 'axios'

export default {
    async uploadImage(image) {
        let data = new FormData();
        data.append('image', image);

        try {
            const response = await axios.post('api/images', data);
            return response.data;
        } catch (error) {
            throw error;
        }
    }
}