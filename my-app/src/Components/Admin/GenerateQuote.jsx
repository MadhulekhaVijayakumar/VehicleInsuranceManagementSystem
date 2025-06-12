// src/components/GenerateQuote.jsx
import React, { useEffect, useState } from "react";
import InsuranceService from "../../Services/QuoteService";

const GenerateQuote = () => {
  const [proposals, setProposals] = useState([]);
  const [quote, setQuote] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    loadApprovedProposals();
  }, []);

  const loadApprovedProposals = async () => {
    try {
      const data = await InsuranceService.getApprovedProposals();
      setProposals(data);
      setError("");
    } catch (err) {
      console.error(err);
      setError("Failed to fetch approved proposals.");
    }
  };

  const handleGenerateQuote = async (proposalId) => {
    try {
      const result = await InsuranceService.generateInsuranceQuote(proposalId);
      setQuote(result);
      setError("");
    } catch (err) {
      console.error(err);
      setError(
        err.response?.data?.message || "Failed to generate quote. Please try again."
      );
      setQuote(null);
    }
  };

  return (
    <div className="container-fluid min-vh-100 d-flex flex-column justify-content-center" 
     style={{
       paddingTop:"2%",
       backgroundImage: "url('Home2.jpeg')",
       backgroundSize: "cover",
       backgroundPosition: "center",
       backgroundRepeat: "no-repeat",
       backgroundAttachment: "fixed"
     }}>
    <div className="container"style={{marginBottom:"100%"}}>
      <h2 style={{color:"#1a2a5a"}}>Approved Proposals</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      {proposals.length === 0 ? (
        <p style={{color:"#1a2a5a"}}>No approved proposals found.</p>
      ) : (
        <table className="proposals-table">
          <thead className="thead-dark">
            <tr>
              <th>Proposal ID</th>
              <th>Client Name</th>
              <th>Insurance Type</th>
              <th>Status</th>
              <th>Generate Quote</th>
            </tr>
          </thead>
          <tbody>
            {proposals.map((proposal) => (
              <tr key={proposal.proposalId}>
                <td>{proposal.proposalId}</td>
                <td>{proposal.clientName}</td>
                <td>{proposal.insuranceType}</td>
                <td>{proposal.status.charAt(0).toUpperCase() + proposal.status.slice(1)}</td>
            
                <td>
                  <button
                    className="btn btn-success"
                    onClick={() => handleGenerateQuote(proposal.proposalId)}
                  >
                    Generate Quote
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {quote && (
        <div className="quote-result mt-5">
          <h4>Quote Details</h4>
          <p><strong>Proposal ID:</strong> {quote.proposalId}</p>
          <p><strong>Premium Amount:</strong> ₹{quote.premiumAmount}</p>
          <p><strong>Insurance Sum:</strong> ₹{quote.insuranceSum}</p>
          <p><strong>Insurance Start Date:</strong> {new Date(quote.insuranceStartDate).toLocaleDateString()}</p>
          <p><strong>Status:</strong> {quote.status.charAt(0).toUpperCase() + quote.status.slice(1)}</p>
        </div>
      )}
    </div>
    </div>
  );
};

export default GenerateQuote;
