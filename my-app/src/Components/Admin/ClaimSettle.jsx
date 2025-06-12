// src/components/ClaimList.jsx
import React, { useEffect, useState } from 'react';
import ClaimService from '../../Services/ClaimSettle';
import { Table, Button, Alert, Spinner } from 'react-bootstrap';

const ClaimSettle = () => {
  const [claims, setClaims] = useState([]);
  const [loading, setLoading] = useState(true);
  const [successMsg, setSuccessMsg] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    fetchApprovedClaims();
  }, []);

  const fetchApprovedClaims = async () => {
    try {
      setLoading(true);
      const data = await ClaimService.getApprovedClaims();
      setClaims(data);
    } catch (error) {
        console.log("Error fetching approved claims:", error);
      setErrorMsg(error.response?.data?.message || 'Failed to fetch claims');
    } finally {
      setLoading(false);
    }
  };

  const handleSettleClaims = async () => {
    const confirmed = window.confirm('Are you sure you want to settle all approved claims?');
    if (!confirmed) return;
  
    setLoading(true);  // Show loading spinner or disable button
  
    try {
      const response = await ClaimService.settleApprovedClaims();
      setSuccessMsg(response.message || 'Claims settled successfully.');
      fetchApprovedClaims(); // Refresh list after settling
    } catch (error) {
      setErrorMsg(error.response?.data?.message || 'Failed to settle claims');
    } finally {
      setLoading(false);  // Hide spinner and enable button again
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
      <h2 style={{color:"#1a2a5a"}}>Approved Claims</h2>

      {loading && <Spinner animation="border" />}
      {successMsg && <Alert variant="success">{successMsg}</Alert>}
      {errorMsg && <Alert variant="danger">{errorMsg}</Alert>}

      {!loading && claims.length > 0 && (
        <>
          <Table striped bordered hover>
            <thead>
              <tr>
                <th>Claim ID</th>
                <th>Policy Number</th>
                <th>Claim Amount</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {claims.map((claim) => (
                <tr key={claim.claimId}>
                  <td>{claim.claimId}</td>
                  <td>{claim.insurancePolicyNumber}</td>
                  <td>₹{claim.claimAmount.toFixed(2)}</td>
                  <td>{claim.claimStatus}</td>
                </tr>
              ))}
            </tbody>
          </Table>

          <Button
  variant="primary"
  onClick={handleSettleClaims}
  disabled={loading} // Disable button while processing
>
  {loading ? 'Settling Claims...' : 'Settle Claims'}
</Button>

        </>
      )}
    </div>
    </div>
  );
};

export default ClaimSettle;
