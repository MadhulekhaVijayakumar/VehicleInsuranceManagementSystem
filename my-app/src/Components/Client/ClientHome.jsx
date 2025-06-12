import React from "react";

const ClientHome = () => {
  return (
    <div>
      {/* Hero Section */}
      <div className="container position-relative" style={{ width: "100vw", paddingLeft: 0 }}>
        {/* Blurred Background Layer */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            backgroundImage: "url('/grasscar.jpg')",
            backgroundSize: "cover",
            backgroundPosition: "center",
            filter: "blur(6px)",
            zIndex: 1
          }}
        ></div>

        {/* Foreground Content */}
        <div
          className="d-flex justify-content-center align-items-center text-center"
          style={{
            minHeight: "50vh",
            position: "relative",
            zIndex: 2,
            color: "black"
          }}
        >
          <div className="text-center mb-5">
            <h1 className="display-5 fw-bold">Welcome to InsureWise 🚗</h1>
            <p className="lead fw-bold">Your trusted partner in Commercial Automobile Insurance</p>
          </div>
        </div>
      </div>

      {/* Feature Cards Section */}
      <div
        className="row g-4 d-flex justify-content-center align-items-center"
        style={{ minHeight: "50vh", backgroundPosition: "center", color: "white", width: "96vw", margin: "0 auto" }}
      >
        {/* Feature 1 */}
        <div className="col-md-4">
          <div className="card h-100 text-center" style={{ boxShadow: "0 4px 12px rgba(38, 158, 238, 0.6)" }}>
            <div className="card-body">
              <img src="https://img.icons8.com/fluency/48/car--v1.png" alt="Vehicle Coverage" className="mb-3" />
              <h5 className="card-title">Custom Vehicle Coverage</h5>
              <p className="card-text">Choose the best-fit policy for your fleet, drivers, and unique business needs.</p>
            </div>
          </div>
        </div>

        {/* Feature 2 */}
        <div className="col-md-4">
          <div className="card h-100 text-center" style={{ boxShadow: "0 4px 12px rgba(38, 158, 238, 0.6)" }}>
            <div className="card-body">
              <img src="https://img.icons8.com/color/48/customer-support.png" alt="Claims Support" className="mb-3" />
              <h5 className="card-title">24/7 Claims Support</h5>
              <p className="card-text">Raise and track claims anytime, anywhere with real-time updates and guidance.</p>
            </div>
          </div>
        </div>

        {/* Feature 3 */}
        <div className="col-md-4">
          <div className="card h-100 text-center" style={{ boxShadow: "0 4px 12px rgba(38, 158, 238, 0.6)" }}>
            <div className="card-body">
              <img src="https://img.icons8.com/fluency/48/documents.png" alt="Policy Management" className="mb-3" />
              <h5 className="card-title">Policy Management</h5>
              <p className="card-text">View, renew, or modify your policies easily from one centralized dashboard.</p>
            </div>
          </div>
        </div>
      </div>

      {/* Footer/Call to Action Section */}
      <div className="mt-5 text-center">
        <h4>🛡 Empowering Your Business on Every Journey</h4>
        <p>
          Whether you manage delivery trucks, transport vehicles, or commercial fleets,
          InsureWise ensures protection that drives your business forward.
        </p>
      </div>
    </div>
  );
};

export default ClientHome;
