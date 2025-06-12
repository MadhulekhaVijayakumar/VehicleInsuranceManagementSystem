// src/services/AdminInsuranceService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const getAllInsurances = async (page = 1, pageSize = 7, token) => {
  const response = await axios.get(`${baseUrl}/Insurance/admin/all`, {
    headers: {
      Authorization: `Bearer ${token}`
    },
    params: { page, pageSize }
  });
  return response.data;
};

export default {
  getAllInsurances
};
