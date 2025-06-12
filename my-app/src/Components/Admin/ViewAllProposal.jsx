// src/components/ViewAllProposal.jsx
import React, { useEffect, useState } from 'react';
import ProposalService from '../../Services/ViewAllProposalService';
import { useNavigate } from 'react-router-dom';

function ViewAllProposal() {
  const [proposals, setProposals] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');

  const token = localStorage.getItem('token'); // assuming you stored token here
  const navigate = useNavigate();

  useEffect(() => {
    fetchProposals();
  }, [page, status]);

  const fetchProposals = async () => {
    try {
      const response = await ProposalService.getPagedProposals(page, pageSize, status, search, token);
      setProposals(response.proposals);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error(error);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    setPage(1); // reset to first page on new search
    fetchProposals();
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="container mt-5">
      <h2>View All Proposals</h2>

      <form onSubmit={handleSearch} className="d-flex mb-3">
        <input
          type="text"
          className="form-control me-2"
          placeholder="Search by Client Name or Vehicle Number"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select className="form-select me-2" value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="">All Status</option>
          <option value="approved">Approved</option>
          <option value="rejected">Rejected</option>
          <option value="submitted">Submitted</option>
          <option value="quote generated">Quote Generated</option>
        </select>
        <button className="btn btn-primary" type="submit">Search</button>
      </form>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Proposal ID</th>
            <th>Client Name</th>
            <th>Vehicle Number</th>
            <th>Insurance Type</th>
            <th>Insurance Valid Upto</th>
            <th>Fitness Valid Upto</th>
            <th>Status</th>
            <th>Created At</th>
          </tr>
        </thead>
        <tbody>
          {proposals.map((proposal) => (
            <tr key={proposal.proposalId}>
              <td>{proposal.proposalId}</td>
              <td>{proposal.clientName}</td>
              <td>{proposal.vehicleNumber}</td>
              <td>{proposal.insuranceType}</td>
              <td>{proposal.insuranceValidUpto.split('T')[0]}</td>
              <td>{proposal.fitnessValidUpto.split('T')[0]}</td>
              <td>{proposal.status.charAt(0).toUpperCase() + proposal.status.slice(1)}</td>

              <td>{proposal.createdAt.split('T')[0]}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Pagination Controls */}
      <div className="d-flex justify-content-center">
        <button
          className="btn btn-secondary me-2"
          disabled={page === 1}
          onClick={() => setPage(page - 1)}
        >
          Previous
        </button>
        <span className="align-self-center">Page {page} of {totalPages}</span>
        <button
          className="btn btn-secondary ms-2"
          disabled={page === totalPages}
          onClick={() => setPage(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
}

export default ViewAllProposal;
