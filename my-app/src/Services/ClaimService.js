// src/Services/ClaimService.js
import axios from 'axios';
import { baseUrl } from '../Environments/environment.dev';



const submitClaim = async (formData, token) => {
    const data = new FormData();
    data.append("InsurancePolicyNumber", formData.insurancePolicyNumber);
    data.append("IncidentDate", formData.incidentDate);
    data.append("Description", formData.description);
    data.append("ClaimAmount", formData.claimAmount);

    // Append documents with required field names
    data.append("Documents.RepairEstimateCost", formData.repairEstimateCost);
    data.append("Documents.AccidentCopy", formData.accidentCopy);
    data.append("Documents.FIRCopy", formData.firCopy);

    const response = await axios.post(`${baseUrl}/InsuranceClaim/Client/SubmitClaim`, data, {
        headers: {
            "Content-Type": "multipart/form-data",
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
};

export default {
    submitClaim
};
