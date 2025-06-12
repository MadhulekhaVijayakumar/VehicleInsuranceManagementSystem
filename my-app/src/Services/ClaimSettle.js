// src/services/ClaimService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';



// Helper to get token from localStorage
const getAuthHeader = () => {
  const token = localStorage.getItem('token'); // Assuming you store the JWT as 'token'
  return {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };
};

const getApprovedClaims = async () => {
  const response = await axios.get(`${baseUrl}/InsuranceClaim/approved`, getAuthHeader());
  return response.data;
};

const settleApprovedClaims = async () => {
  const response = await axios.put(`${baseUrl}/InsuranceClaim/settle-approved`, null, getAuthHeader());
  return response.data;
};

export default {
  getApprovedClaims,
  settleApprovedClaims,
};
