import React from "react";
import { Link } from "react-router-dom"; // For navigation if needed
import "./ClientNavbar.css"; // optional CSS file

const ClientNavbar = ({ onNavigate, onLogout }) => {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark  shadow-sm navbar-custom">
      <div className="container-fluid">
        <button className="navbar-brand btn btn-link text-white text-decoration-none" onClick={() => onNavigate("home")}>
          InsureWise
        </button>

         {/* Navbar Toggle for smaller screens */}
         <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span className="navbar-toggler-icon"></span>
        </button>
       

        {/* Navbar items (links and buttons) */}
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("home")}>Home</button>
            </li>

            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("proposals")}>Proposals</button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("track")}>Track Status</button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("quotes")}>View Quotes</button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("policy")}>My Policy</button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("raiseClaim")}>Raise Claim</button>
            </li>
            <li className="nav-item">
              <button className="btn nav-link text-white" onClick={() => onNavigate("claims")}>My Claims</button>
            </li>
          </ul>
        </div>

        {/* Profile and Logout Button */}
        <div className="d-flex align-items-center gap-3">
        <button className="btn btn-profile" onClick={() => onNavigate("profile")}>
  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12 12C14.7614 12 17 9.76142 17 7C17 4.23858 14.7614 2 12 2C9.23858 2 7 4.23858 7 7C7 9.76142 9.23858 12 12 12Z" fill="currentColor"/>
    <path d="M12 14C7.58172 14 4 17.5817 4 22H20C20 17.5817 16.4183 14 12 14Z" fill="currentColor"/>
  </svg>
</button>
<button 
  className="btn btn-logout-custom" 
  onClick={onLogout}
  style={{
    display: "flex",
    alignItems: "center",
    gap: "6px"
  }}
>Logout</button>
        </div>
      </div>
    </nav>
  );
};

export default ClientNavbar;
