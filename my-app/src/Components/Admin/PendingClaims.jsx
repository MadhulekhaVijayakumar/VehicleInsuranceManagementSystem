import React, { useEffect, useState } from 'react';
import ClaimService from '../../Services/AdminClaimService';

const PendingClaims = ({ selectedClaimId, setSelectedClaimId }) => {
  const [claims, setClaims] = useState([]);
  const [claimDetails, setClaimDetails] = useState(null);

  useEffect(() => {
    ClaimService.getAllPendingClaims()
      .then(res => setClaims(res.data))
      .catch(err => console.error(err));
  }, []);

  useEffect(() => {
    if (selectedClaimId) {
      ClaimService.getClaimDetails(selectedClaimId)
        .then(res => setClaimDetails(res.data))
        .catch(err => console.error(err));
    }
  }, [selectedClaimId]);

  const downloadFile = (fileType) => {
    ClaimService.downloadDocument(selectedClaimId, fileType)
      .then(response => {
        const blob = new Blob([response.data]);
        const link = document.createElement('a');
        const fileName = `${fileType}.${response.headers['content-type'].split('/')[1]}`; // Deriving file extension from the content type (e.g., .png, .pdf)
  
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;  // Set the file name dynamically
        link.click();
      })
      .catch(() => alert("Failed to download file"));
  };
  const handleStatusUpdate = (status) => {
    if (!window.confirm(`Are you sure you want to ${status.toLowerCase()} this claim?`)) return;
  
    ClaimService.updateClaimStatus(selectedClaimId, status)
      .then(() => {
        alert(`Claim ${status.toLowerCase()} successfully.`);
        setClaimDetails(null);
        setSelectedClaimId(null);
        // Refresh claim list
        ClaimService.getAllPendingClaims()
          .then(res => setClaims(res.data))
          .catch(err => console.error(err));
      })
      .catch(err => {
        console.error(err);
        alert('Failed to update claim status.');
      });
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
    <div className="container" style={{marginBottom:"100%"}}>
      <h2 style={{color:"#1a2a5a"}}>Pending Insurance Claims</h2>
      <table className="table table-striped mt-3">
        <thead>
          <tr>
            <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Claim ID</th>
            <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Policy Number</th>
            <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Claim Amount</th>
            <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Status</th>
            <th style={{backgroundColor:"#1a2a5a",color:'white'}}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {claims.map(claim => (
            <tr key={claim.claimId}>
              <td>{claim.claimId}</td>
              <td>{claim.insurancePolicyNumber}</td>
              <td>₹{claim.claimAmount}</td>
              <td>{claim.claimStatus}</td>
              <td>
                <button
                  className="btn btn-primary btn-sm"
                  onClick={() => setSelectedClaimId(claim.claimId)}
                >
                  View Details
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {claimDetails && selectedClaimId && (
        <div className="card p-4 mt-5 shadow-lg">
          <h4>Claim Details - #{claimDetails.claimId}</h4>
          <hr />
          <h5>Insurance Info</h5>
          <p><strong>Policy No:</strong> {claimDetails.insurancePolicyNumber}</p>
          <p><strong>Insurance Sum:</strong> ₹{claimDetails.insuranceSum}</p>
          <p><strong>Premium:</strong> ₹{claimDetails.premiumAmount}</p>
          <p><strong>Status:</strong> {claimDetails.insuranceStatus}</p>

          <h5 className="mt-3">Client Info</h5>
          <p><strong>Name:</strong> {claimDetails.clientName}</p>
          <p><strong>Email:</strong> {claimDetails.email}</p>
          <p><strong>Phone:</strong> {claimDetails.phoneNumber}</p>
          <p><strong>Address:</strong> {claimDetails.address}</p>

          <h5 className="mt-3">Vehicle Info</h5>
          <p><strong>Vehicle Number:</strong> {claimDetails.vehicleNumber}</p>
          <p><strong>Type:</strong> {claimDetails.vehicleType}</p>
          <p><strong>Make:</strong> {claimDetails.make}</p>
          <p><strong>Model:</strong> {claimDetails.model}</p>
          <p><strong>Registration Date:</strong> {new Date(claimDetails.registrationDate).toLocaleDateString()}</p>

          <h5 className="mt-3">Claim Info</h5>
          <p><strong>Incident Date:</strong> {new Date(claimDetails.incidentDate).toLocaleDateString()}</p>
          <p><strong>Amount:</strong> ₹{claimDetails.claimAmount}</p>
          <p><strong>Status:</strong> {claimDetails.status}</p>
          <p><strong>Description:</strong> {claimDetails.description}</p>

          <h5 className="mt-3">Documents</h5>
          <ul>
            {claimDetails.documents?.repairEstimateCost && (
              <li>
                <button className="btn btn-link" onClick={() => downloadFile('RepairEstimateCost')}>
                  {claimDetails.documents.repairEstimateCost}
                </button>
              </li>
            )}
            {claimDetails.documents?.accidentCopy && (
              <li>
                <button className="btn btn-link" onClick={() => downloadFile('AccidentCopy')}>
                  {claimDetails.documents.accidentCopy}
                </button>
              </li>
            )}
            {claimDetails.documents?.firCopy && (
              <li>
                <button className="btn btn-link" onClick={() => downloadFile('FIRCopy')}>
                  {claimDetails.documents.firCopy}
                </button>
              </li>
            )}
          </ul>
          {/* Action Buttons */}
<div className="mt-4">
  <button
    className="btn btn-success me-3"
    onClick={() => handleStatusUpdate('Approved')}
  >
    Approve
  </button>
  <button
    className="btn btn-danger"
    onClick={() => handleStatusUpdate('Rejected')}
  >
    Reject
  </button>
</div>


          <button
            className="btn btn-secondary mt-3"
            onClick={() => {
              setSelectedClaimId(null);
              setClaimDetails(null);
            }}
          >
            Close
          </button>
        </div>
      )}
    </div>
    </div>
  );
};

export default PendingClaims;
