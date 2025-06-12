import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";



export const submitProposal = async (formData, token) => {
  try {
    const API_URL = baseUrl + "/Proposal/SubmitProposal";
    const response = await axios.post(
      API_URL,
      formData,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          // DO NOT manually set Content-Type! Browser will set it automatically.
        },
      }
    );
    return response.data;
  } catch (error) {
    throw error;
  }
};