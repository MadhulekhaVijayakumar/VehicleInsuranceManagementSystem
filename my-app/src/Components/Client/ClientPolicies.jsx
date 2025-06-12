// src/components/ClientPolicies.jsx
import React, { useEffect, useState } from 'react';
import InsuranceService from '../../Services/InsuranceService';
import "./ClientNavbar.css";

const ClientPolicies = () => {
  const [policies, setPolicies] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('token'); // Or however you're storing JWT

    if (token) {
      InsuranceService.getClientPolicies(token)
        .then(data => setPolicies(data))
        .catch(err => {
          console.error(err);
          setError('Failed to fetch policies.');
        });
    } else {
      setError('User not authenticated.');
    }
  }, []);
 

  const handleDownload = async (policyNumber) => {
    const token = localStorage.getItem('token');
  
    try {
      const blob = await InsuranceService.downloadPolicyPdf(policyNumber, token);
      const url = window.URL.createObjectURL(blob);
  
      const a = document.createElement('a');
      a.href = url;
      a.download = `Policy_${policyNumber}.pdf`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error(error);
      alert('Could not download the policy document.');
    }
  };
  


  return (
    
    <div className="container mt-4" id="Policy">
 
      <h2 className="text-center mb-4" style={{color:"#1a2a5a",fontWeight:"bold"}}>Your Insurance Policies</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      {policies.length === 0 ? (
        <p>No policies found.</p>
      ) : (
        <div className="table-responsive">
          <table className="table table-bordered table-striped">
            <thead className="table-primary">
              <tr>
                <th>Policy Number</th>
                <th>Vehicle Number</th>
                <th>Insurance Type</th>
                <th>Plan</th>
                <th>Start Date</th>
                <th>Expiry Date</th>
                <th>Sum Insured</th>
                <th>Premium</th>
                <th>Status</th>
                <th>Download</th>
              </tr>
            </thead>
            <tbody>
              {policies.map((policy, index) => (
                <tr key={index}>
                  <td>{policy.insurancePolicyNumber}</td>
                  <td>{policy.vehicleNumber}</td>
                  <td>{policy.insuranceType}</td>
                  <td>{policy.plan}</td>
                  <td>{new Date(policy.insuranceStartDate).toLocaleDateString()}</td>
                  <td>{new Date(policy.expiryDate).toLocaleDateString()}</td>
                  <td>₹{policy.insuranceSum}</td>
                  <td>₹{policy.premiumAmount}</td>
                  <td>{policy.status.charAt(0).toUpperCase() + policy.status.slice(1)}</td>
                  <td>
                    {policy.status === "active" ? (
                    <button
                    className="btn btn-link"
                    onClick={() => handleDownload(policy.insurancePolicyNumber)}
                    title="Download Policy"
                    >
                    📄
                    </button>
                    ) : (
                    "-"
                    )}
                    </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
   
  );
};

export default ClientPolicies;
