using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InsuranceAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly InsuranceManagementContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(InsuranceManagementContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<object> ProcessPaymentAsync(CreatePaymentRequest request)
        {
            try
            {
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.ProposalId == request.ProposalId && p.TransactionStatus == "success");

                if (existingPayment != null)
                {
                    _logger.LogInformation($"ℹ️ Payment already exists for ProposalId: {request.ProposalId}");
                    return new { Message = "Payment has already been made for this proposal." };
                }

                var payment = new Payment
                {
                    ProposalId = request.ProposalId,
                    AmountPaid = request.AmountPaid,
                    PaymentDate = DateTime.Now,
                    PaymentMode = request.PaymentMode,
                    TransactionStatus = "success"
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                var proposal = await _context.Proposals
                    .FirstOrDefaultAsync(p => p.ProposalId == request.ProposalId);

                if (proposal != null)
                {
                    proposal.Status = "payment successful";
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"✅ Payment successful. PaymentId: {payment.PaymentId}");

                return new PaymentResponse
                {
                    PaymentId = payment.PaymentId,
                    ProposalId = payment.ProposalId,
                    AmountPaid = payment.AmountPaid,
                    PaymentDate = payment.PaymentDate,
                    PaymentMode = payment.PaymentMode,
                    TransactionStatus = payment.TransactionStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Payment failed for ProposalId: {request.ProposalId} - {ex.Message}");
                throw new Exception("An unexpected error occurred during payment processing.", ex);
            }
        }





    }

}