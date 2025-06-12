// src/services/ClaimStatusService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';


const getMyClaims = async (token) => {
  try {
    const response = await axios.get(`${baseUrl}/InsuranceClaim/Client/MyClaims`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || "Failed to fetch claims";
  }
};

export default {
  getMyClaims
};
