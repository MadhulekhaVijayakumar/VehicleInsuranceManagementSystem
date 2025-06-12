import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

// Helper to get token from localStorage
const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };
};

const getAllPendingClaims = () => {
  return axios.get(`${baseUrl}/InsuranceClaim/pending`, getAuthHeaders());
};

const getClaimDetails = (claimId) => {
  return axios.get(`${baseUrl}/InsuranceClaim/review/${claimId}`, getAuthHeaders());
};

const downloadDocument = (claimId, fileType) => {
  return axios.get(`${baseUrl}/InsuranceClaim/download/${claimId}/${fileType}`, {
    ...getAuthHeaders(),
    responseType: 'blob',
  });
};

const updateClaimStatus = (claimId, newStatus) => {
    return axios.put(
      `${baseUrl}/InsuranceClaim/Admin/UpdateStatus/${claimId}?newStatus=${newStatus}`,
      null,
      getAuthHeaders()
    );
  };
export default {
  getAllPendingClaims,
  getClaimDetails,
  downloadDocument,
  updateClaimStatus,
};
