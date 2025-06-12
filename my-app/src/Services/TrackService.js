// src/services/PolicyService.js
import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

const API_URL = baseUrl + "/Insurance/Client/TrackStatus"; // Replace with your API base URL

const getClientPolicyStatus = async () => {
  const token = localStorage.getItem("token");
  const response = await axios.get(API_URL, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};

export default {
  getClientPolicyStatus,
};
