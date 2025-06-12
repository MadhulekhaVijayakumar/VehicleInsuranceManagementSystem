// src/Components/Client/QuoteCards.jsx
import React, { useEffect, useState } from "react";
import ClientQuotes from "../../Services/ClientQuotes";
import "./ClientNavbar.css";

const QuoteCards = ({ setActivePage, setSelectedProposalId }) => {
  const [quotes, setQuotes] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchQuotes();
  }, []);

  const fetchQuotes = async () => {
    try {
      const data = await ClientQuotes.getQuotesForClient();
      setQuotes(data);
    } catch (err) {
      console.error(err);
      setError("Failed to fetch quotes.");
    }
  };

  const handlePayNow = (proposalId) => {
    const proposal = quotes.find(q => q.proposalId === proposalId);

    if (!proposal) {
      alert("❌ Proposal not found.");
      return;
    }

    if (proposal.status.toLowerCase() === "payment successful") {
      alert("✅ Payment has already been made for this proposal.");
      return;
    }

    setSelectedProposalId(proposal);
    setActivePage("payment");
  };

  const handleDownload = async (proposalId) => {
    try {
      const response = await ClientQuotes.downloadQuote(proposalId);
      const blob = new Blob([response.data], { type: "application/pdf" });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `Quote_${proposalId}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (err) {
      console.error(err);
      setError("Failed to download quote.");
    }
  };

  return (
    <div className=" quote-container">
      <h2 className="quote-title">Your Generated Quotes</h2>
      {error && <div className="alert alert-danger">{error}</div>}

      <div className="row">
        {quotes.length === 0 ? (
          <p>No quotes found.</p>
        ) : (
          quotes.map((quote) => (
            <div className="col-md-4 mb-4" key={quote.proposalId}>
              <div className="card shadow rounded ms-5">
                <div className="card-body">
                  <h5 className="card-title">Proposal #{quote.proposalId}</h5>
                  <p className="card-text">
                    <strong>Insurance Type:</strong> {quote.insuranceType}
                  </p>
                  <p className="card-text">
                    <strong>Status:</strong>{" "}
                    {quote.status.charAt(0).toUpperCase() + quote.status.slice(1)}
                  </p>
                  <p className="card-text">
                    <strong>Insurance Sum:</strong> ₹{quote.insuranceSum}
                  </p>
                  <p className="card-text">
                    <strong>Premium Amount:</strong> ₹{quote.calculatedPremium}
                  </p>
                  <p className="card-text">
                    <strong>Insurance Start Date:</strong>{" "}
                    {new Date(quote.insuranceStartDate).toLocaleDateString()}
                  </p>

                  <button
                    className="btn btn-primary me-2"
                    onClick={() => handlePayNow(quote.proposalId)}
                  >
                    Pay Now
                  </button>
                  <button
                    className="btn btn-outline-secondary"
                    onClick={() => handleDownload(quote.proposalId)}
                  >
                    Download Quote
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default QuoteCards;
