// services/ProposalVerificationService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    Authorization: `Bearer ${token}`,
  };
};

export const verifyProposal = async (proposalId, approve) => {
  const response = await axios.put(
    `${baseUrl}/Proposal/Admin/Verify/${proposalId}?approve=${approve}`,
    {},
    { headers: getAuthHeaders() }
  );
  return response.data;
};
