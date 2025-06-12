import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import ClientNavbar from "./ClientNavbar";
import ProfilePage from "./ProfilePage";
import ClientHome from "./ClientHome";
import Proposal from "./Proposal";
import TrackStatus from "./TrackStatus";
import QuoteCards from "./ClientQuotes";
import PaymentPage from "./Payment";
import ClientPolicies from "./ClientPolicies";
import SubmitClaim from "./SubmitClaim";
import MyClaims from "./MyClaims";

const ClientDashboard = () => {
  const [activePage, setActivePage] = useState("home"); // default page
  const [selectedProposal, setSelectedProposalId] = useState(null); 
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.clear();
    sessionStorage.clear();
    navigate("/login");
  };

  return (
    <>
      <ClientNavbar onNavigate={setActivePage} onLogout={handleLogout} />
      <div style={{ paddingTop: "0px" ,paddingRight:"0"}}>
        {activePage === "home" && <ClientHome />}
        {activePage === "profile" && <ProfilePage />}
        {/* placeholders for future pages */}
        {activePage === "proposals" && <Proposal/>}
        {activePage === "track" && <TrackStatus/>}
        {activePage === "quotes" && (
          <QuoteCards setActivePage={setActivePage} setSelectedProposalId={setSelectedProposalId} />
        )}
        {activePage === "payment" && selectedProposal && (
          <PaymentPage selectedProposal={selectedProposal} onPaymentSuccess={() => setActivePage("home")} />
        )}

        {activePage === "policy" && <ClientPolicies/>}
        {activePage === "raiseClaim" && <SubmitClaim/>}
        {activePage === "claims" && <MyClaims/>}
      </div>
      
      <footer className="text-white text-center py-3 mt-auto"style={{backgroundColor: "#1a2a5a", width:"99vw" }}>
        <p className="mb-0">&copy; {new Date().getFullYear()} InsureWise. All rights reserved.</p>
      </footer>
    </>
  );
};

export default ClientDashboard;
