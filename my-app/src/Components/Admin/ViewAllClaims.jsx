// components/AdminClaims.jsx
import React, { useEffect, useState } from 'react';
import { getAllClaimsForAdmin } from '../../Services/ViewAllClaims';

const ViewAllClaims = () => {
  const [claims, setClaims] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(7);
  const [totalRecords, setTotalRecords] = useState(0);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchClaims(currentPage);
  }, [currentPage]);

  const fetchClaims = async (page) => {
    setLoading(true);
    try {
      const result = await getAllClaimsForAdmin(page, pageSize);
      setClaims(result.data);
      setTotalRecords(result.totalRecords);
      setError('');
    } catch (err) {
      setError(err.toString());
    } finally {
      setLoading(false);
    }
  };

  const totalPages = Math.ceil(totalRecords / pageSize);

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setCurrentPage(newPage);
    }
  };

  const getStatusBadge = (status) => {
    let badgeClass = '';
    switch (status.toLowerCase()) {
      case 'approved':
        badgeClass = 'bg-success';
        break;
      case 'pending':
        badgeClass = 'bg-warning text-dark';
        break;
      case 'rejected':
        badgeClass = 'bg-danger';
        break;
      case 'processing':
        badgeClass = 'bg-info';
        break;
      default:
        badgeClass = 'bg-secondary';
    }
    return <span className={`badge ${badgeClass}`}>{status}</span>;
  };

  return (
    <div className="container-fluid py-4">
      <div className="card shadow-sm">
        <div className="card-header  text-white" style={{backgroundColor:"#1a2a5a"}}>
          <h2 className="h4 mb-0">Claims Management</h2>
        </div>
        
        <div className="card-body">
          {error && (
            <div className="alert alert-danger alert-dismissible fade show" role="alert">
              {error}
              <button type="button" className="btn-close" onClick={() => setError('')}></button>
            </div>
          )}

          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
              <p className="mt-2">Loading claims data...</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <table className="table table-hover table-striped align-middle">
                  <thead className="table-light">
                    <tr>
                      <th scope="col" className="py-3">Claim ID</th>
                      <th scope="col" className="py-3">Policy #</th>
                      <th scope="col" className="py-3">Incident Date</th>
                      <th scope="col" className="py-3">Description</th>
                      <th scope="col" className="py-3 text-end">Amount</th>
                      <th scope="col" className="py-3 text-center">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {claims.length > 0 ? (
                      claims.map((claim) => (
                        <tr key={claim.claimId} className="cursor-pointer" onClick={() => {/* Add click handler if needed */}}>
                          <td className="py-2 fw-semibold">{claim.claimId}</td>
                          <td className="py-2">{claim.insurancePolicyNumber}</td>
                          <td className="py-2">{new Date(claim.incidentDate).toLocaleDateString()}</td>
                          <td className="py-2 text-truncate" style={{maxWidth: '200px'}} title={claim.description}>
                            {claim.description}
                          </td>
                          <td className="py-2 text-end">${parseFloat(claim.claimAmount).toFixed(2)}</td>
                          <td className="py-2 text-center">{getStatusBadge(claim.status)}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="6" className="text-center py-4 text-muted">
                          No claims found
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>

              <div className="d-flex flex-column flex-md-row justify-content-between align-items-center mt-4">
                <div className="mb-2 mb-md-0">
                  Showing {(currentPage - 1) * pageSize + 1} to {Math.min(currentPage * pageSize, totalRecords)} of {totalRecords} entries
                </div>
                
                <nav aria-label="Page navigation">
                  <ul className="pagination pagination-sm mb-0">
                    <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
                      <button className="page-link" onClick={() => handlePageChange(currentPage - 1)}>
                        &laquo; Previous
                      </button>
                    </li>
                    
                    {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                      let pageNum;
                      if (totalPages <= 5) {
                        pageNum = i + 1;
                      } else if (currentPage <= 3) {
                        pageNum = i + 1;
                      } else if (currentPage >= totalPages - 2) {
                        pageNum = totalPages - 4 + i;
                      } else {
                        pageNum = currentPage - 2 + i;
                      }
                      
                      return (
                        <li key={pageNum} className={`page-item ${currentPage === pageNum ? 'active' : ''}`}>
                          <button className="page-link" onClick={() => handlePageChange(pageNum)}>
                            {pageNum}
                          </button>
                        </li>
                      );
                    })}
                    
                    <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
                      <button className="page-link" onClick={() => handlePageChange(currentPage + 1)}>
                        Next &raquo;
                      </button>
                    </li>
                  </ul>
                </nav>
              </div>
            </>
          )}
        </div>
        
        <div className="card-footer bg-light">
          <small className="text-muted">Last updated: {new Date().toLocaleString()}</small>
        </div>
      </div>
    </div>
  );
};

export default ViewAllClaims;