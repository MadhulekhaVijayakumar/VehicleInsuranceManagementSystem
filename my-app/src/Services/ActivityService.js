import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';



export const getRecentActivities = async () => {
    const response = await axios.get(`${baseUrl}/Activity/activities/recent`);
    return response.data;
};
