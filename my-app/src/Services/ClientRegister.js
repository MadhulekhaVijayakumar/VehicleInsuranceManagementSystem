import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

const API_URL = baseUrl+"/Client/Register";

export const registerClient = async (clientData) => {
  const response = await axios.post(API_URL, clientData);
  return response.data;
};
