// services/claimService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const getAuthHeaders = () => {
    const token = localStorage.getItem('token');
    return {
      Authorization: `Bearer ${token}`,
    };
  };

export const getAllClaimsForAdmin = async (pageNumber = 1, pageSize = 7) => {
  try {
    const response = await axios.get(`${baseUrl}/InsuranceClaim/Admin/AllClaim`, {
      headers: getAuthHeaders(),
      params: { pageNumber, pageSize },
    });
    return response.data;
  } catch (error) {
    throw error.response?.data?.message || "Failed to fetch claims";
  }
};
