// src/components/AdminInsuranceList.jsx
import React, { useEffect, useState } from 'react';
import AdminInsuranceService from '../../Services/AdminInsuranceService';

const AdminInsuranceList = () => {
  const [insurances, setInsurances] = useState([]);
  const [error, setError] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalRecords, setTotalRecords] = useState(0);

  const token = localStorage.getItem('token');

  useEffect(() => {
    if (token) {
      AdminInsuranceService.getAllInsurances(page, pageSize, token)
        .then(data => {
          setInsurances(data.data);
          setTotalRecords(data.totalRecords);
        })
        .catch(err => {
          console.error(err);
          setError('Failed to load insurances.');
        });
    }
  }, [page, pageSize, token]);

  const totalPages = Math.ceil(totalRecords / pageSize);

  return (
    <div className="container mt-4">
      <h2 className="text-center" style={{ color: '#1a2a5a', fontWeight: 'bold' }}>
        All Issued Insurances
      </h2>
      {error && <div className="alert alert-danger">{error}</div>}
      {insurances.length === 0 ? (
        <p>No insurances found.</p>
      ) : (
        <div className="table-responsive">
          <table className="table table-bordered table-striped">
            <thead className="table-secondary">
              <tr>
                <th>Policy Number</th>
                <th>Client Name</th>
                <th>Vehicle Number</th>
                <th>Insurance Type</th>
                <th>Plan</th>
                <th>Start Date</th>
                <th>Expiry Date</th>
                <th>Sum Insured</th>
                <th>Premium</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {insurances.map((i, idx) => (
                <tr key={idx}>
                  <td>{i.insurancePolicyNumber}</td>
                  <td>{i.name}</td>
                  <td>{i.vehicleNumber}</td>
                  <td>{i.insuranceType}</td>
                  <td>{i.plan}</td>
                  <td>{new Date(i.insuranceStartDate).toLocaleDateString()}</td>
                  <td>{new Date(i.expiryDate).toLocaleDateString()}</td>
                  <td>₹{i.insuranceSum}</td>
                  <td>₹{i.premiumAmount}</td>
                  <td>{i.status.charAt(0).toUpperCase() + i.status.slice(1)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination */}
      <div className="d-flex justify-content-between mt-3">
        <button
          className="btn btn-primary"
          disabled={page <= 1}
          onClick={() => setPage(page - 1)}
        >
          Previous
        </button>
        <span className="align-self-center">Page {page} of {totalPages}</span>
        <button
          className="btn btn-primary"
          disabled={page >= totalPages}
          onClick={() => setPage(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
};

export default AdminInsuranceList;
