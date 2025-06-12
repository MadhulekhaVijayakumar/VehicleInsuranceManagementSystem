// src/components/MyClaims.jsx
import React, { useEffect, useState } from 'react';
import ClaimStatusService from '../../Services/ClaimStatusService';


const MyClaims = () => {
  const [claims, setClaims] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      setError("Please log in to view your claims.");
      return;
    }

    const fetchClaims = async () => {
      try {
        const data = await ClaimStatusService.getMyClaims(token);
        setClaims(data);
      } catch (err) {
        setError(err);
      }
    };

    fetchClaims();
  }, []);

  return (
    <div className="container-fluid min-vh-100 d-flex flex-column justify-content-center" 
    style={{
      paddingTop:"2%",
      backgroundImage: "url('claim.jpeg')",
      backgroundSize: "cover",
      backgroundPosition: "center",
      backgroundRepeat: "no-repeat",
      backgroundAttachment: "fixed"
    }}
    id="myclaim">
 <div className="container bg-white p-4 rounded" style={{maxWidth: '1050px'}}>
      <h2 className="mb-4" style={{color:"#1a2a5a"}}>My Insurance Claims</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      {claims.length === 0 && !error && <p>No claims found.</p>}

      {claims.length > 0 && (
        <table className="table table-bordered table-hover">
          <thead className="thead-dark">
            <tr>
              <th>Claim ID</th>
              <th>Policy Number</th>
              <th>Incident Date</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {claims.map((claim) => (
              <tr key={claim.claimId}>
                <td>{claim.claimId}</td>
                <td>{claim.insurancePolicyNumber}</td>
                <td>{new Date(claim.incidentDate).toLocaleDateString()}</td>
                <td>₹ {claim.claimAmount.toFixed(2)}</td>
                <td><span className={`badge ${getStatusBadge(claim.status)}`}>{claim.status}</span></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
    </div>
  );
};

const getStatusBadge = (status) => {
  switch (status.toLowerCase()) {
    case "pending": return "bg-warning text-dark";
    case "approved": return "bg-success";
    case "rejected": return "bg-danger";
    default: return "bg-secondary";
  }
};

export default MyClaims;
