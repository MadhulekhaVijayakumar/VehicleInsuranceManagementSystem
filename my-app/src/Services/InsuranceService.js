// src/services/InsuranceService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';



const getClientPolicies = async (token) => {
  const response = await axios.get(`${baseUrl}/Insurance/Client/ViewPolicy`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return response.data;
};
const downloadPolicyPdf = async (policyNumber, token) => {
    const response = await fetch(`${baseUrl}/Insurance/Client/Download-policy/${policyNumber}`, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  
    if (!response.ok) {
      throw new Error('Download failed.');
    }
  
    return await response.blob();
  };
  

export default {
  getClientPolicies,
  downloadPolicyPdf
};
