import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';


const token = localStorage.getItem("token");
const getAllClients = async (pageNumber = 1, pageSize = 10) => {
  try {
    const response = await axios.get(`${baseUrl}/Client/AllClient`, {
      params: { pageNumber, pageSize },
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("There was an error fetching clients:", error);
    throw error; // or return an error message for UI
  }
};

export default {
  getAllClients
};
