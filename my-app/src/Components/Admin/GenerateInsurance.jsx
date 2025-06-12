// src/components/PaidProposals.jsx
import React, { useEffect, useState } from 'react';
import { getPaidProposals, generateInsurance } from '../../Services/GenerateInsuranceService';

const GenerateInsurance = () => {
  const [proposals, setProposals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const fetchProposals = async () => {
    try {
      const data = await getPaidProposals();
      setProposals(data);
    } catch (error) {
      setErrorMessage("Failed to fetch proposals.");
    } finally {
      setLoading(false);
    }
  };

  const handleGenerate = async (proposalId) => {
    try {
      const response = await generateInsurance(proposalId);
      setSuccessMessage(`Insurance generated successfully: ${response.insurancePolicyNumber}`);
      fetchProposals(); // Refresh list
    } catch (error) {
      setErrorMessage("Failed to generate insurance.");
    }
  };

  useEffect(() => {
    fetchProposals();
  }, []);

  return (
    <div className="container-fluid min-vh-100 d-flex flex-column justify-content-center" 
     style={{
       paddingTop:"2%",
       backgroundImage: "url('bluecar.jpg')",
       backgroundSize: "cover",
       backgroundPosition: "center",
       backgroundRepeat: "no-repeat",
       backgroundAttachment: "fixed"
     }}>
    <div className="container mt-4" style={{marginBottom:"100%"}}>
      <h2 style={{color:"#1a2a5a"}}>Generate Insurance</h2>

      {successMessage && <div className="alert alert-success">{successMessage}</div>}
      {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}

      {loading ? (
        <p>Loading proposals...</p>
      ) : (
        <table className="table table-bordered mt-3">
          <thead >
            <tr>
              <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Proposal ID</th>
              <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Client Name</th>
              <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Insurance Type</th>
              <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Status</th>
              <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Action</th>
            </tr>
          </thead>
          <tbody>
            {proposals.map((proposal) => (
              <tr key={proposal.proposalId}>
                <td>{proposal.proposalId}</td>
                <td>{proposal.clientName}</td>
                <td>{proposal.insuranceType}</td>
                <td>{proposal.status}</td>
                <td>
                  <button
                    className="btn btn-primary"
                    onClick={() => handleGenerate(proposal.proposalId)}
                  >
                    Generate Insurance
                  </button>
                </td>
              </tr>
            ))}
            {proposals.length === 0 && (
              <tr>
                <td colSpan="5" className="text-center">
                  No proposals ready for insurance.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      )}
    </div>
    </div>
  );
};

export default GenerateInsurance;
