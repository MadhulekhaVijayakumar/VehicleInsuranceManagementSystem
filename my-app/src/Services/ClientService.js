// ClientService.js
import axios from "axios";
import { baseUrl } from "../Environments/environment.dev";

const API_URL = baseUrl + "/Client/Profile";

export const getClientProfile = async (token) => {
  const response = await axios.get(API_URL, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};

export const updateClientProfile = async (token, updatedProfile) => {
    const payload = {
      NameUpdate: { NewName: updatedProfile.name },
      DateOfBirthUpdate: { NewDateOfBirth: updatedProfile.dateOfBirth },
      GenderUpdate: { NewGender: updatedProfile.gender },
      PhoneUpdate: { NewPhoneNumber: updatedProfile.phoneNumber },
      AadhaarUpdate: { NewAadhaarNumber: updatedProfile.aadhaarNumber },
      PANUpdate: { NewPANNumber: updatedProfile.panNumber },
      AddressUpdate: { NewAddress: updatedProfile.address },
    };
  
    const response = await axios.put(`${baseUrl}/Client/Update-profile`, payload, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  
    return response.data;
  };
