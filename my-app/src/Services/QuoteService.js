// src/services/InsuranceService.js
import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

const API_URL = baseUrl + "/Insurance/Admin/Generatequote"; 

const generateInsuranceQuote = async (proposalId) => {
  const token = localStorage.getItem("token");
  const response = await axios.post(
    `${API_URL}/${proposalId}`,
    {},
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
  return response.data;
};
const getApprovedProposals = async () => {
  const token = localStorage.getItem("token");
  const response = await axios.get(`${baseUrl}/Proposal/Admin/GetApproved`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};

export default {
  generateInsuranceQuote,
  getApprovedProposals,
};



