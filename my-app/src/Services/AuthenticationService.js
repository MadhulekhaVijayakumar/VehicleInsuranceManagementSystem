import axios from "axios";
import { baseUrl } from "../Environments/environment.dev.js";

const API_URL = baseUrl + "/Authentication/login";

export const loginUser = async (credentials) => {
  try {
    const response = await axios.post(API_URL, credentials);
    const { token, role, name, id } = response.data.data; // ✅ extract from .data.data

    if (token && role) {
      // Save in localStorage
      localStorage.setItem("token", token);
      localStorage.setItem("role", role);
      localStorage.setItem("name", name);
      localStorage.setItem("userId", id);

      // Save in sessionStorage as well
      sessionStorage.setItem(
        "user",
        JSON.stringify({ token, role, name, id })
      );
    }

    return response.data.data;
  } catch (error) {
    console.error("Login failed:", error.response?.data || error.message);
    throw error;
  }
};
