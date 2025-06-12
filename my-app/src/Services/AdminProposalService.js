import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    Authorization: `Bearer ${token}`,
  };
};

const getSubmittedProposals = async () => {
  const response = await axios.get(`${baseUrl}/Proposal/Admin/GetSubmitted`, {
    headers: getAuthHeaders(),
  });
  return response.data;
};

const getProposalDetails = async (proposalId) => {
  const response = await axios.get(`${baseUrl}/Proposal/GetProposalDetails/${proposalId}`, {
    headers: getAuthHeaders(),
  });
  return response.data;
};

const downloadDocument = async (proposalId, fileType) => {
  const response = await axios.get(`${baseUrl}/Proposal/Admin/Download-document`, {
    headers: getAuthHeaders(),
    params: { proposalId, fileType },
    responseType: 'blob', // important for file downloads
  });
  return response;
};

const verifyProposal = async (proposalId, approve) => {
  const response = await axios.put(
    `${baseUrl}/Proposal/Admin/Verify/${proposalId}?approve=${approve}`,
    {},
    { headers: getAuthHeaders() }
  );
  return response.data;
};


export default {
  getSubmittedProposals,
  getProposalDetails,
  downloadDocument,
  verifyProposal,
};
