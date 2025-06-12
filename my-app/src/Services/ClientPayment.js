import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

// Retrieve token from localStorage (or sessionStorage if preferred)
const token = localStorage.getItem("token");

if (!token) {
  console.error("No token found. User may not be logged in.");
}

const API_URL = `${baseUrl}/Payment/make-payment`;

const makePayment = async (paymentRequest) => {
  const response = await axios.post(API_URL, paymentRequest, {
   

    headers: {
      Authorization: `Bearer ${token}`, // Pass the token in the Authorization header
    },
  });
  console.log("Token being sent:", token);
  return response.data;
};

const ClientPayment = {
  makePayment,
};

export default ClientPayment;