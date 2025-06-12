import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import AdminNavbar from "./AdminNavbar";
import AdminHome from "./AdminHome";
import AdminProposals from "./AdminProposal";
import ViewAllProposal from "./ViewAllProposal";
import GenerateQuote from "./GenerateQuote";
import GenerateInsurance from "./GenerateInsurance";
import AdminInsuranceList from "./AdminInsuranceList";
import PendingClaims from "./PendingClaims";
import ViewAllClaims from "./ViewAllClaims";
import ClaimSettle from "./ClaimSettle";
import ClientList from "./ClientList";

const AdminDashboard = () => {
  const [activePage, setActivePage] = useState("home");
  const navigate = useNavigate();
  const [selectedClaimId, setSelectedClaimId] = useState(null);

  const handleLogout = () => {
    localStorage.clear();
    sessionStorage.clear(); 
    navigate("/login");
  };
 



  return (
    <>
      <div style={{ 
  display: 'flex',
  flexDirection: 'column',
  minHeight: '100vh' // This makes sure the container takes at least the full viewport height
}}>
      <AdminNavbar onNavigate={setActivePage} onLogout={handleLogout} />
      <div style={{ flex: 1 }}>
        {activePage === "home" && <AdminHome />} 
        {activePage === "clients" &&<ClientList/>}
        {/* {activePage === "proposals" &&<AdminProposals /> } */}
        {activePage === "viewSubmittedProposal" &&<AdminProposals />}
        {activePage === "viewAllProposals" && <ViewAllProposal/>}
        {activePage === "generatequote" && <GenerateQuote/>}
        {activePage === "generateinsurance" && <GenerateInsurance />}
        {activePage === "allpolicy" && <AdminInsuranceList/>}
        {activePage === "claims" && (
  <PendingClaims
    selectedClaimId={selectedClaimId}
    setSelectedClaimId={setSelectedClaimId}
  />
)}

        {activePage === "viewAllClaims" && <ViewAllClaims/>}
       
        {activePage === "settle" && <ClaimSettle/>}
      </div>

      <footer style={{ 
    backgroundColor: "#1a2a5a",
    color: "white",
    textAlign: "center",
    padding: "1rem 0",
    marginTop: "auto", // This makes it stick to bottom
    width: "99vw",
    
  }}>
    <p style={{ margin: 0 }}>
      &copy; {new Date().getFullYear()} InsureWise Admin Panel
    </p>
  </footer>
      </div>
    </>
  );
};

export default AdminDashboard;
