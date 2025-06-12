// src/services/InsuranceService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const getAuthHeaders = () => ({
  headers: {
    Authorization: `Bearer ${localStorage.getItem('token')}`
  }
});

export const getPaidProposals = async () => {
  const response = await axios.get(`${baseUrl}/Insurance/Admin/GetPaidProposal`, getAuthHeaders());
  return response.data;
};

export const generateInsurance = async (proposalId) => {
  const response = await axios.post(`${baseUrl}/Insurance/Admin/GenerateInsurance/${proposalId}`, {}, getAuthHeaders());
  return response.data;
};
