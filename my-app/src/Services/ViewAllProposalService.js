// src/services/ViewAllProposalService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';

const API_URL = baseUrl + "/Proposal/paged";; // <-- change this to your backend URL

const getPagedProposals = async (page, pageSize, status = '', search = '', token) => {
  try {
    const response = await axios.get(API_URL, {
      params: { Page: page, PageSize: pageSize, Status: status, Search: search },
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching proposals:', error);
    throw error;
  }
};

export default {
  getPagedProposals
};
