// src/Models/Payment.js
export class CreatePaymentRequest {
    constructor(proposalId, amountPaid, paymentMode) {
      this.proposalId = proposalId;
      this.amountPaid = amountPaid;
      this.paymentMode = paymentMode;
    }
  }
  
  export class PaymentResponse {
    constructor(paymentId, proposalId, amountPaid, paymentDate, paymentMode, transactionStatus) {
      this.paymentId = paymentId;
      this.proposalId = proposalId;
      this.amountPaid = amountPaid;
      this.paymentDate = paymentDate;
      this.paymentMode = paymentMode;
      this.transactionStatus = transactionStatus;
    }
  }
  