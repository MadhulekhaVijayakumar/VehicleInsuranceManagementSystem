import React from "react";
import "../Client/ClientNavbar.css"; // Reusing the same CSS
// You can rename this CSS file if you'd like a separate style

const AdminNavbar = ({ onNavigate, onLogout }) => {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark shadow-sm navbar-custom">
      <div className="container-fluid">
        <button
          className="navbar-brand btn btn-link text-white text-decoration-none"
          onClick={() => onNavigate("home")}
        >
          InsureWise Admin
        </button>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("home")}>
                Home
              </button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("clients")}>
                Clients
              </button>
            </li>
            <li className="nav-item dropdown">
  <button
    className="btn nav-link  text-white"
    id="proposalDropdown"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Proposals
  </button>
  <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="proposalDropdown">
    <li>
      <button className="dropdown-item text-dark" onClick={() => onNavigate("viewSubmittedProposal")}>
        View Submitted Proposals
      </button>
    </li>
    <li>
      <button className="dropdown-item text-dark" onClick={() => onNavigate("viewAllProposals")}>
        View All Proposals
      </button>
    </li>
  </ul>
</li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("generatequote")}>
                Generate Quotes
              </button>
            </li>

            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("generateinsurance")}>
                Generate Insurance
              </button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("allpolicy")}>
                Policies
              </button>
            </li>
            <li className="nav-item dropdown">
  <button
    className="btn nav-link  text-white"
    id="proposalDropdown"
    data-bs-toggle="dropdown"
    aria-expanded="false"
  >
    Claims
  </button>
  <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="proposalDropdown">
    <li>
      <button className="dropdown-item text-dark" onClick={() => onNavigate("claims")}>
        View Pending Claims
      </button>
    </li>
    <li>
      <button className="dropdown-item text-dark" onClick={() => onNavigate("viewAllClaims")}>
        View All claims
      </button>
    </li>
    <li>
      <button className="dropdown-item text-dark" onClick={() => onNavigate("settle")}>
        Claim Settlement
      </button>
    </li>
  </ul>
</li>

            
          </ul>
        </div>

        <div className="d-flex align-items-center gap-3">
          <button className="btn btn-profile" onClick={() => onNavigate("profile")}>
            <svg width="24" height="24" fill="currentColor" viewBox="0 0 24 24">
              <path d="M12 12c2.76 0 5-2.24 5-5s-2.24-5-5-5S7 4.24 7 7s2.24 5 5 5z" />
              <path d="M12 14c-4.42 0-8 3.58-8 8h16c0-4.42-3.58-8-8-8z" />
            </svg>
          </button>
          <button className="btn btn-logout-custom" onClick={onLogout}>
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
};

export default AdminNavbar;
