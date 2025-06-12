// src/Services/QuoteService.js
import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

// Utility function to get Authorization header with token
const getAuthHeaders = () => ({
  headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
});

const ClientQuotes = {
  // Fetch quotes for the client
  async getQuotesForClient() {
    const response = await axios.get(`${baseUrl}/Proposal/Client/GetQuotes`, getAuthHeaders());
    return response.data;
  },

  // Download a specific quote
  async downloadQuote(proposalId) {
    const response = await axios.get(
      `${baseUrl}/Quote/download-quote/${proposalId}`,
      {
        ...getAuthHeaders(),
        responseType: "blob", // Important for file download
      }
    );
    return response;
  },
};

export default ClientQuotes;
