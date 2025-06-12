// src/Components/Client/PaymentPage.jsx
import React, { useState } from "react";
import ClientPayment from "../../Services/ClientPayment";
import { CreatePaymentRequest } from "../../Models/Payment";
import "./ClientNavbar.css";

const PaymentPage = ({ selectedProposal, onPaymentSuccess }) => {
  const [payerName, setPayerName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [paymentMode, setPaymentMode] = useState("UPI");
  const [upiId, setUpiId] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [expiry, setExpiry] = useState("");
  const [cvv, setCvv] = useState("");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const handlePayment = async () => {
    setLoading(true);
    setError("");
    setSuccessMessage("");

    if (!payerName || !email || !phone) {
      setError("Please fill all contact details.");
      setLoading(false);
      return;
    }

    if (paymentMode === "UPI" && !upiId) {
      setError("Please enter your UPI ID.");
      setLoading(false);
      return;
    }

    if (paymentMode === "Card" && (!cardNumber || !expiry || !cvv)) {
      setError("Please enter all card details.");
      setLoading(false);
      return;
    }

    if (!selectedProposal) {
      setError("Invalid proposal selected. Please go back and try again.");
      setLoading(false);
      return;
    }

    try {
      const request = new CreatePaymentRequest(
        selectedProposal.proposalId,
        selectedProposal.calculatedPremium,
        paymentMode
      );

      console.log("Sending payment request:", request);
      await ClientPayment.makePayment(request);

      setSuccessMessage("🎉 Payment successful!");
      setTimeout(() => {
    onPaymentSuccess(); // Delay navigation
  }, 3000);
    } catch (err) {
      console.error("Payment error:", err);

      if (err.response) {
        if (err.response.status === 409) {
          setError("⚠️ Payment has already been made for this proposal.");
        } else if (err.response.data?.message) {
          setError(`❌ ${err.response.data.message}`);
        } else {
          setError("❌ Payment failed due to server error. Please try again.");
        }
      } else {
        setError("❌ Network error or server unavailable.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container payment-container mt-5">
      <h3>💳 Complete Your Payment</h3>

      <div className="mb-3">
        <label>Name on Card / Payer Name</label>
        <input className="form-control" value={payerName} onChange={(e) => setPayerName(e.target.value)} />
      </div>

      <div className="row">
        <div className="col-md-6 mb-3">
          <label>Email</label>
          <input type="email" className="form-control" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="col-md-6 mb-3">
          <label>Phone Number</label>
          <input type="tel" className="form-control" value={phone} onChange={(e) => setPhone(e.target.value)} />
        </div>
      </div>

      <div className="mb-3">
        <label>Select Payment Mode</label>
        <select className="form-select" value={paymentMode} onChange={(e) => setPaymentMode(e.target.value)}>
          <option value="UPI">UPI</option>
          <option value="Card">Credit/Debit Card</option>
          <option value="NetBanking">Net Banking</option>
        </select>
      </div>

      {paymentMode === "UPI" && (
        <div className="mb-3">
          <label>Enter UPI ID</label>
          <input className="form-control" value={upiId} onChange={(e) => setUpiId(e.target.value)} placeholder="someone@upi" />
        </div>
      )}

      {paymentMode === "Card" && (
        <>
          <div className="mb-3">
            <label>Card Number</label>
            <input className="form-control" maxLength="16" value={cardNumber} onChange={(e) => setCardNumber(e.target.value)} />
          </div>
          <div className="row">
            <div className="col-md-6 mb-3">
              <label>Expiry (MM/YY)</label>
              <input className="form-control" value={expiry} onChange={(e) => setExpiry(e.target.value)} />
            </div>
            <div className="col-md-6 mb-3">
              <label>CVV</label>
              <input className="form-control" maxLength="3" value={cvv} onChange={(e) => setCvv(e.target.value)} />
            </div>
          </div>
        </>
      )}

      {paymentMode === "NetBanking" && (
        <div className="mb-3">
          <label>Select Bank</label>
          <select className="form-select">
            <option>HDFC Bank</option>
            <option>ICICI Bank</option>
            <option>SBI</option>
            <option>Axis Bank</option>
          </select>
        </div>
      )}

      <div className="mt-4">
        <button className="btn btn-primary" onClick={handlePayment} disabled={loading}>
          {loading ? "Processing..." : `Pay ₹${selectedProposal.calculatedPremium}`}
        </button>
      </div>

      {error && <div className="alert alert-danger mt-3">{error}</div>}
      {successMessage && <div className="alert alert-success mt-3">{successMessage}</div>}
    </div>
  );
};

export default PaymentPage;