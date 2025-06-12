import React, { useEffect, useState } from 'react';
import AdminProposalService from '../../Services/AdminProposalService';
import { Button, Modal, Spinner } from 'react-bootstrap';
import { Form } from 'react-bootstrap';

import "./Admin.css";

const AdminProposals = () => {
  const [proposals, setProposals] = useState([]);
  const [selectedProposalId, setSelectedProposalId] = useState(null);
  const [selectedProposal, setSelectedProposal] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [loadingDetails, setLoadingDetails] = useState(false);
  const [downloadingFileType, setDownloadingFileType] = useState('');
  const [verificationStatus, setVerificationStatus] = useState('approve');
  const [verificationMessage, setVerificationMessage] = useState('');
  const [verifying, setVerifying] = useState(false);

  const documentFields = {
    License: 'licenseFileName',
    'RC Book': 'rcBookFileName',
    'Pollution Certificate': 'pollutionCertificateFileName'
  };

  useEffect(() => {
    const fetchProposals = async () => {
      try {
        const data = await AdminProposalService.getSubmittedProposals();
        setProposals(data);
      } catch (err) {
        console.error('Error loading proposals:', err);
      }
    };
    fetchProposals();
  }, []);

  const handleViewDetails = async (proposalId) => {
    try {
      setLoadingDetails(true);
      setSelectedProposalId(proposalId);
      const data = await AdminProposalService.getProposalDetails(proposalId);
      setSelectedProposal(data);
      setVerificationStatus('approve'); // default
      setVerificationMessage('');
      setShowModal(true);
    } catch (error) {
      console.error('Error fetching details:', error);
    } finally {
      setLoadingDetails(false);
    }
  };

  const handleVerifySubmit = async () => {
    if (!selectedProposalId) return;
    try {
      setVerifying(true);
      await AdminProposalService.verifyProposal(selectedProposalId, verificationStatus === 'approve');
      setVerificationMessage(`Proposal ${verificationStatus === 'approve' ? 'approved' : 'rejected'} successfully.`);
      setProposals(prev => prev.filter(p => p.proposalId !== selectedProposalId)); // remove from list after action
    } catch (error) {
      console.error('Verification failed:', error);
      setVerificationMessage('Verification failed. Please try again.');
    } finally {
      setVerifying(false);
    }
  };

  const handleDownload = async (fileType) => {
    if (!selectedProposal) {
      console.error('Proposal ID is missing');
      return;
    }
    try {
      setDownloadingFileType(fileType);
  
      const response = await AdminProposalService.downloadDocument(selectedProposalId, fileType);
      // Get filename from Content-Disposition if present
      const disposition = response.headers['content-disposition'];
      let fileName = `${fileType.replace(/\s+/g, '_')}`;
      const contentType = response.headers['content-type'] || 'application/octet-stream';
  
      // Add extension based on Content-Type if filename not provided
      const extensionMap = {
        'application/pdf': '.pdf',
        'application/msword': '.doc',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document': '.docx',
        'image/jpeg': '.jpg',
        'image/png': '.png'
      };
  
      if (disposition && disposition.includes('filename=')) {
        fileName = disposition.split('filename=')[1].replace(/['"]/g, '').trim();
      } else {
        const ext = extensionMap[contentType] || '';
        fileName += ext;
      }
  
      const blob = new Blob([response.data], { type: contentType });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (err) {
      //console.error('Download failed:', err);
      console.error('Download failed:', err.response || err.message || err);

    } finally {
      setDownloadingFileType('');
    }
  };
  

  const formatDate = (date) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('en-GB');
  };

  return (
    <div className="admin-proposals-container">
       <h3 className="admin-proposals-heading">Submitted Proposals</h3>
       <table className="proposals-table">
        <thead>
          <tr>
            <th>Proposal ID</th>
            <th>Client ID</th>
            <th>Client Name</th>
            <th>Insurance Type</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {proposals.length > 0 ? (
            proposals.map((p) => (
              <tr key={p.proposalId}>
                <td>{p.proposalId}</td>
                <td>{p.clientId}</td>
                <td>{p.clientName}</td>
                <td>{p.insuranceType}</td>
                <td>{p.status.charAt(0).toUpperCase() + p.status.slice(1)}</td>

                <td>
                  <Button className="view-details-btn" onClick={() => handleViewDetails(p.proposalId)}>
                    View & Verify
                  </Button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="6" className="no-proposals-message">No proposals found.</td>
            </tr>
          )}
        </tbody>
      </table>

      {/* Modal for Proposal Details */}
      <Modal show={showModal} onHide={() => setShowModal(false)} size="lg" centered>
        <Modal.Header closeButton>
          <Modal.Title>Proposal Details</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {loadingDetails ? (
            <div className="loading-spinner">
              <Spinner animation="border" role="status" />
            </div>
          ) : selectedProposal ? (
            <>
              {/* Vehicle Info */}
              <h5 className="section-heading">Vehicle Info</h5>
              <ul className="detail-list">
                <li><strong>Number:</strong> {selectedProposal.vehicle?.vehicleNumber || '-'}</li>
                <li><strong>Model:</strong> {selectedProposal.vehicle?.modelName || '-'}</li>
                <li><strong>Maker Name:</strong> {selectedProposal.vehicle?.makerName || '-'}</li>
                <li><strong>Chassis Number:</strong> {selectedProposal.vehicle?.chassisNumber || '-'}</li>
                <li><strong>Engine Number:</strong> {selectedProposal.vehicle?.engineNumber || '-'}</li>
                <li><strong>Fuel Type:</strong> {selectedProposal.vehicle?.fuelType || '-'}</li>
                <li><strong>Vehicle Color:</strong> {selectedProposal.vehicle?.vehicleColor || '-'}</li>
                <li><strong>Seat Capacity:</strong> {selectedProposal.vehicle?.seatCapacity || '-'}</li>
                <li><strong>Registration Date:</strong> {formatDate(selectedProposal.vehicle?.registrationDate)}</li>
              </ul>

              {/* Proposal Info */}
              <h5 className="section-heading">Proposal Info</h5>
              <ul className="detail-list">

                <li><strong>Insurance Type:</strong> {selectedProposal.proposal?.insuranceType || '-'}</li>
                <li><strong>Insurance Valid Upto:</strong> {formatDate(selectedProposal.proposal?.insuranceValidUpto)}</li>
                <li><strong>Fitness Valid Upto:</strong> {formatDate(selectedProposal.proposal?.fitnessValidUpto)}</li>
              </ul>

              {/* Insurance Details */}
              <h5 className="section-heading">Insurance Details</h5>
              <ul className="detail-list">
                <li><strong>Sum Insured:</strong> ₹{selectedProposal.insuranceDetails?.insuranceSum?.toLocaleString() || '0'}</li>
                <li><strong>Plan:</strong> {selectedProposal.insuranceDetails?.plan || '-'}</li>
                <li><strong>Damage Insurance:</strong> {selectedProposal.insuranceDetails?.damageInsurance || '-'}</li>
                <li><strong>Liability Option:</strong> {selectedProposal.insuranceDetails?.liabilityOption || '-'}</li>
                <li><strong>Insurance Start Date:</strong> {formatDate(selectedProposal.insuranceDetails?.insuranceStartDate)}</li>
              </ul>

              {/* Documents */}
              <h5 className="section-heading">Documents</h5>
              {Object.keys(documentFields).map((type) => {
                const fileKey = documentFields[type];
                const fileName = selectedProposal.documents?.[fileKey];
                return fileName ? (
                    <div key={type} className="document-item">
                      <span className="document-label">{type}:</span>
                      <span>{fileName}</span>
                      <Button 
                        variant="secondary" 
                        size="sm" 
                        className="download-btn ml-2"
                        onClick={() => handleDownload(type)} 
                        disabled={downloadingFileType === type}
                      >
                        {downloadingFileType === type ? 'Downloading...' : 'Download'}
                      </Button>
                    </div>
                  ) : (
                    <div key={type} className="document-item">
                    <span className="document-label">{type}:</span>
                    <span>Not Uploaded</span>
                  </div>
                );
              })}
                {/* Approve/Reject Form */}
                <h5 className="section-heading mt-4">Verify Proposal</h5>
              <Form>
                <Form.Group>
                  <Form.Label>Status</Form.Label>
                  <Form.Select value={verificationStatus} onChange={(e) => setVerificationStatus(e.target.value)}>
                    <option value="approve">Approve</option>
                    <option value="reject">Reject</option>
                  </Form.Select>
                </Form.Group>
                {verificationMessage && (
                  <div className="alert alert-info mt-2">{verificationMessage}</div>
                )}
                <div className="text-center mt-3">
                  <Button
                    variant="primary"
                    onClick={handleVerifySubmit}
                    disabled={verifying}
                  >
                    {verifying ? "Processing..." : "Submit Verification"}
                  </Button>
                </div>
              </Form>
            </>
          ) : (
            <p className="no-proposals-message">Loading proposal...</p>
          )}
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default AdminProposals;
