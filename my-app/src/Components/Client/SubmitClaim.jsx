import React, { useState } from 'react';
import ClaimService from '../../Services/ClaimService';

const SubmitClaim = () => {
    const [formData, setFormData] = useState({
        insurancePolicyNumber: '',
        incidentDate: '',
        description: '',
        claimAmount: '',
        repairEstimateCost: null,
        accidentCopy: null,
        firCopy: null
    });

    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false);
    const [formErrors, setFormErrors] = useState({});

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleFileChange = (e) => {
        const { name, files } = e.target;
        setFormData({ ...formData, [name]: files[0] });
    };
    const validateForm = () => {
        const errors = {};
        if (!formData.insurancePolicyNumber.trim()) errors.insurancePolicyNumber = 'Policy number is required.';
        if (!formData.incidentDate) errors.incidentDate = 'Incident date is required.';
        if (!formData.description.trim()) errors.description = 'Description is required.';
        if (!formData.claimAmount) errors.claimAmount = 'Claim amount is required.';
        if (!formData.repairEstimateCost) errors.repairEstimateCost = 'Repair estimate is required.';
        if (!formData.accidentCopy) errors.accidentCopy = 'Accident copy is required.';
        if (!formData.firCopy) errors.firCopy = 'FIR copy is required.';
        return errors;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
       
        setMessage('');
        const errors = validateForm();
    setFormErrors(errors);

    if (Object.keys(errors).length > 0) {
        return;
    }

    setLoading(true);

        try {
            const token = localStorage.getItem('token');
            const result = await ClaimService.submitClaim(formData, token);
            setMessage(result.message || 'Claim submitted successfully.');
        } catch (err) {
            console.error(err);
            setMessage('Failed to submit claim.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container-fluid min-vh-100 d-flex flex-column justify-content-center" 
     style={{
       paddingTop:"2%",
       backgroundImage: "url('claim.jpeg')",
       backgroundSize: "cover",
       backgroundPosition: "center",
       backgroundRepeat: "no-repeat",
       backgroundAttachment: "fixed"
     }}
     id="submitclaim">
  <div className="container bg-white p-4 rounded" style={{maxWidth: '800px'}}>
            <h2 style={{color:"#1a2a5a"}}>Submit Insurance Claim</h2>
            <form onSubmit={handleSubmit} encType="multipart/form-data">
            <div className="mb-3">
                <label className="form-label">Insurance Policy Number</label>
                <input type="text" className={`form-control ${formErrors.insurancePolicyNumber ? 'is-invalid' : ''}`} name="insurancePolicyNumber"
                value={formData.insurancePolicyNumber}
                onChange={handleChange}
                required/>
                <div className="form-text">e.g. IP0</div>
                {formErrors.insurancePolicyNumber && (
                    <div className="invalid-feedback">{formErrors.insurancePolicyNumber}</div>
                )}
                </div>

                <div className="mb-3">
                    <label className="form-label">Incident Date</label>
                    <input type="datetime-local" className="form-control" name="incidentDate" value={formData.incidentDate} onChange={handleChange} required />
                </div>

                <div className="mb-3">
                    <label className="form-label">Description</label>
                    <textarea className="form-control" name="description" value={formData.description} onChange={handleChange} required />
                </div>

                <div className="mb-3">
                    <label className="form-label">Claim Amount</label>
                    <input type="number" className="form-control" name="claimAmount" value={formData.claimAmount} onChange={handleChange} required />
                    {formErrors.claimAmount && (
                        <div className="text-danger">{formErrors.claimAmount}</div>
                    )}

                </div>

                <div className="mb-3">
                    <label className="form-label">Repair Estimate Cost</label>
                    <input type="file" className="form-control" name="repairEstimateCost" onChange={handleFileChange} accept=".pdf,.doc,.docx,.jpg,.jpeg,.png" required />
                </div>

                <div className="mb-3">
                    <label className="form-label">Accident Copy</label>
                    <input type="file" className="form-control" name="accidentCopy" onChange={handleFileChange} accept=".pdf,.doc,.docx,.jpg,.jpeg,.png" required />
                </div>

                <div className="mb-3">
                    <label className="form-label">FIR Copy</label>
                    <input type="file" className="form-control" name="firCopy" onChange={handleFileChange} accept=".pdf,.doc,.docx,.jpg,.jpeg,.png" required />
                </div>

                <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? 'Submitting...' : 'Submit Claim'}
                </button>
             
            </form>

            {message && <div className="alert alert-info mt-3">{message}</div>}
        </div>
        </div>
    );
};

export default SubmitClaim;
