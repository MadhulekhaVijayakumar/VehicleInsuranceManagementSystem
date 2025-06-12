import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";
const API_URL = baseUrl + "/AdminDashboard/summary";

// replace with your actual backend URL

export const getAdminSummary = async (token) => {
  try {
    const response = await axios.get(API_URL, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching dashboard summary:", error);
    throw error;
  }
};
