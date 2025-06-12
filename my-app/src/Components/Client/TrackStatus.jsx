// src/components/TrackStatus.jsx
import React, { useState, useEffect } from "react";
import PolicyService from "../../Services/TrackService";
import "./ClientNavbar.css"

const TrackStatus = () => {
  const [policies, setPolicies] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchPolicies = async () => {
      try {
        const result = await PolicyService.getClientPolicyStatus();
        setPolicies(result);
        setError("");
      } catch (err) {
        console.error(err);
        setError(
          err.response?.data?.message || "Failed to fetch policies. Please try again."
        );
        setPolicies(Array.isArray(result) ? result : []);

      }
    };

    fetchPolicies();
  }, []);

  return (
    <div className="track-status-container">
      <h2>My Proposal Status</h2>
      {error && <div className="alert alert-danger">{error}</div>}

      {policies.length === 0 ? (
        <div>No policies found.</div>
      ) : (
        <div className="policy-list">
          {policies.map((policy) => (
            <div key={policy.proposalId} className="policy-card">
              <h4>Proposal ID: {policy.proposalId}</h4>
              <p><strong>Vehicle Number:</strong> {policy.vehicleNumber}</p>
              <p><strong>Vehicle Type:</strong> {policy.vehicleType}</p>
              <p><strong>Insurance Type:</strong> {policy.insuranceType}</p>
              <p><strong>Status:</strong> {policy.proposalStatus.charAt(0).toUpperCase() + policy.proposalStatus.slice(1)}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default TrackStatus;
