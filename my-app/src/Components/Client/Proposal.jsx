
import React, { useState } from "react";
import { initialProposalData } from "../../Models/proposal";
import { submitProposal } from "../../Services/ProposalService";
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

const Proposal = () => {
  const [formData, setFormData] = useState(initialProposalData);
  const [errors, setErrors] = useState({});
  const [submissionResult, setSubmissionResult] = useState(null);
  const [step, setStep] = useState(1);
  const [fileErrors, setFileErrors] = useState({});
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [successData, setSuccessData] = useState({});

  const vehicleTypes = ["Car", "Bike", "Camper Van"];
  const makers = ["Maruti Suzuki", "Hyundai", "Tata", "Mahindra", "Honda", "Toyota", "Kia", "Volkswagen", "BMW", "Mercedes", "Audi", "Ford", "Renault", "Skoda", "MG"];
  const colors = ["White", "Black", "Silver", "Gray", "Red", "Blue", "Brown", "Green", "Yellow", "Orange"];
  const fuelTypes = ["Petrol", "Diesel", "Electric", "Gas"];
  const insuranceTypes = ["Comprehensive", "Third-Party", "Own Damage (OD) Only"];
  const insuranceSums = [50000, 75000, 100000, 125000, 150000, 200000, 250000, 300000, 500000, 750000, 1000000];
  const damageInsuranceOptions = ["Yes", "No"];
  const liabilityOptions = ["Third-Party", "Comprehensive", "Own Damage"];
  const planOptions = ["Silver", "Gold", "Platinum"];

  const handleChange = (section, field, value) => {
    setFormData((prev) => ({
      ...prev,
      [section]: {
        ...prev[section],
        [field]: value,
      },
    }));

    setErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[field];
      return newErrors;
    });
  };

  const handleFileChange = (field, file) => {
    if (!file) {
      setFormData(prev => ({
        ...prev,
        documents: {
          ...prev.documents,
          [field]: null
        }
      }));
      setFileErrors(prev => ({ ...prev, [field]: null }));
      return;
    }

    const validExtensions = [".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"];
    const extension = file.name.substring(file.name.lastIndexOf(".")).toLowerCase();
    const isValidExtension = validExtensions.includes(extension);
    const isValidSize = file.size <= 5 * 1024 * 1024;

    if (!isValidExtension) {
      setFileErrors(prev => ({ ...prev, [field]: "Invalid file type. Allowed: PDF, DOC, JPG, PNG" }));
      return;
    }

    if (!isValidSize) {
      setFileErrors(prev => ({ ...prev, [field]: "File size must be less than 5MB" }));
      return;
    }

    setFileErrors(prev => ({ ...prev, [field]: null }));
    setFormData((prev) => ({
      ...prev,
      documents: {
        ...prev.documents,
        [field]: file,
      },
    }));
  };

  const validateStep = () => {
    const newErrors = {};
    const p = formData.proposal;
    switch (step) {
      case 1:
        const v = formData.vehicle;
        if (!v.vehicleType) newErrors.vehicleType = "Vehicle type is required";
        if (!v.vehicleNumber) newErrors.vehicleNumber = "Vehicle number is required";
        if (!v.chassisNumber) newErrors.chassisNumber = "Chassis number is required";
        if (!v.engineNumber) newErrors.engineNumber = "Engine number is required";
        if (!v.makerName) newErrors.makerName = "Maker name is required";
        if (!v.modelName) newErrors.modelName = "Model name is required";
        if (!v.vehicleColor) newErrors.vehicleColor = "Vehicle color is required";
        if (!v.fuelType) newErrors.fuelType = "Fuel type is required";
        if (!v.registrationDate) newErrors.registrationDate = "Registration date is required";
        break;

      case 2:
        
        if (!p.insuranceType) newErrors.insuranceType = "Insurance type is required";
        if (!p.insuranceValidUpto) newErrors.insuranceValidUpto = "Insurance valid upto is required"; // Allow blank as valid
        if (!p.fitnessValidUpto) newErrors.fitnessValidUpto = "Fitness valid upto is required";
        break;

        case 3:
  const i = formData.insuranceDetails;
  
  // Reusing the 'p' variable from case 2 here
  if (!i.insuranceStartDate) newErrors.insuranceStartDate = "Insurance start date is required";
  else {
    const startDate = new Date(i.insuranceStartDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time portion to midnight for today

    // Validate that the start date is at least 2 days after today
    const minStartDate = new Date(today);
    minStartDate.setDate(today.getDate() + 2); // Add 2 days to today's date

    if (startDate < minStartDate) {
      newErrors.insuranceStartDate = "Start date must be at least 2 days after today";
    }

    // Proceed with insuranceValidUpto logic if needed
    if (p.insuranceValidUpto && p.insuranceValidUpto !== "") {
      const validUpto = new Date(p.insuranceValidUpto);
      validUpto.setDate(validUpto.getDate() + 1);

      if (startDate <= validUpto) {
        newErrors.insuranceStartDate = "Start date must be at least 1 day after Insurance Valid Upto date";
      }
    }
  }

  if (!i.insuranceSum || i.insuranceSum <= 0) newErrors.insuranceSum = "Insurance sum is required";
  if (!i.plan) newErrors.plan = "Plan is required";
  if (!i.damageInsurance) newErrors.damageInsurance = "Damage insurance option is required";
  if (!i.liabilityOption) newErrors.liabilityOption = "Liability option is required";
  break;
      case 4:
        const d = formData.documents;
        if (!d.license) newErrors.license = "License is required";
        else if (fileErrors.license) newErrors.license = fileErrors.license;

        if (!d.rcBook) newErrors.rcBook = "RC Book is required";
        else if (fileErrors.rcBook) newErrors.rcBook = fileErrors.rcBook;

        if (!d.pollutionCertificate) newErrors.pollutionCertificate = "Pollution Certificate is required";
        else if (fileErrors.pollutionCertificate) newErrors.pollutionCertificate = fileErrors.pollutionCertificate;
        break;
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const nextStep = () => {
    if (validateStep()) {
      setStep((prev) => prev + 1);
    }
  };

  const prevStep = () => {
    setStep((prev) => prev - 1);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateStep()) return;

    const token = localStorage.getItem("token");

    try {
      const form = new FormData();

      // Append Vehicle Details
      Object.entries(formData.vehicle).forEach(([key, value]) => {
        form.append(`vehicle.${key}`, value);
      });

      // Append Proposal Details
      Object.entries(formData.proposal).forEach(([key, value]) => {
        form.append(`proposal.${key}`, value);
      });

      // Append Insurance Details
      form.append('insuranceDetails.insuranceStartDate', formData.insuranceDetails.insuranceStartDate);
      form.append('insuranceDetails.insuranceSum', formData.insuranceDetails.insuranceSum.toString());
      form.append('insuranceDetails.damageInsurance', formData.insuranceDetails.damageInsurance === "Yes" ? "true" : "false");
      form.append('insuranceDetails.liabilityOption', formData.insuranceDetails.liabilityOption);
      form.append('insuranceDetails.plan', formData.insuranceDetails.plan);

      // Append Files
      form.append("documents.license", formData.documents.license);
      form.append("documents.rcBook", formData.documents.rcBook);
      form.append("documents.pollutionCertificate", formData.documents.pollutionCertificate);

      const result = await submitProposal(form, token);
      setSuccessData({
        proposalId: result.proposalId,
        premium: result.calculatedPremium
      });
      setShowSuccessModal(true);
      setSubmissionResult({ success: true });
    } catch (error) {
      setSubmissionResult({ 
        success: false, 
        error: error.response?.data?.message || error.message || "Submission failed" 
      });
    }
  };
  // Render methods remain the same as before
  const renderDropdown = (label, section, field, options, allowCustom = false) => (
    <div className="mb-3">
      <label className="form-label">{label}</label>
      {allowCustom ? (
        <>
          <input
            list={`${field}-list`}
            className={`form-control ${errors[field] ? 'is-invalid' : ''}`}
            value={formData[section][field]}
            onChange={(e) => handleChange(section, field, e.target.value)}
          />
          <datalist id={`${field}-list`}>
            {options.map((option) => (
              <option key={option} value={option} />
            ))}
          </datalist>
        </>
      ) : (
        <select
          className={`form-select ${errors[field] ? 'is-invalid' : ''}`}
          value={formData[section][field]}
          onChange={(e) => handleChange(section, field, e.target.value)}
        >
          <option value="">Select {label.toLowerCase()}</option>
          {options.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
      )}
      {errors[field] && <div className="invalid-feedback">{errors[field]}</div>}
    </div>
  );

  const renderInput = (label, section, field, type = "text") => (
    <div className="mb-3">
      <label className="form-label">{label}</label>
      <input
        type={type}
        className={`form-control ${errors[field] ? 'is-invalid' : ''}`}
        value={formData[section][field]}
        onChange={(e) => handleChange(section, field, e.target.value)}
      />
      {errors[field] && <div className="invalid-feedback">{errors[field]}</div>}
    </div>
  );

  const renderFileInput = (label, field) => (
    <div className="mb-3">
      <label className="form-label">{label}</label>
      <input 
        type="file" 
        className={`form-control ${errors[field] ? 'is-invalid' : ''}`}
        onChange={(e) => handleFileChange(field, e.target.files[0])}
        accept=".pdf,.doc,.docx,.jpg,.jpeg,.png"
      />
      <small className="text-muted">Allowed: PDF, DOC, JPG, PNG (Max 5MB)</small>
      {errors[field] && <div className="invalid-feedback">{errors[field]}</div>}
    </div>
  );

  return (
    <div style={{
      background: "linear-gradient(to right, #e0f7fa, #b2ebf2)",
      backgroundImage: "url('/carInsure.jpeg')",
      backgroundRepeat: "no-repeat",
      backgroundSize: "cover",
      backgroundPosition: "center",
      display: "flex",
      justifyContent: "center",
      alignItems: "flex-start",
      minHeight: "100vh",
      width: "100vw",
      paddingTop: "40px",
      paddingBottom: "40px"
    }}>
      <div
        className="bg-white p-4 rounded shadow"
        style={{
          maxWidth: "700px",
          width: "100%",
          margin: "0 auto",
          marginTop: "10px",
          marginLeft: "28%"
        }}
      >
        <h2 className="text-center mb-4">Insurance Proposal Submission</h2>
        
        {/* Step Progress Bar */}
        <div style={{ width: "100%", display: "flex", flexDirection: "column", alignItems: "center", marginBottom: "30px" }}>
          <div style={{ display: "flex", justifyContent: "space-between", maxWidth: "300px", width: "100%", marginTop: "10px", fontSize: "14px" }}>
            {[1, 2, 3, 4].map((s) => (
              <React.Fragment key={s}>
                <div
                  style={{
                    width: "30px",
                    height: "30px",
                    borderRadius: "50%",
                    backgroundColor: step > s ? "green" : step === s ? "gray" : "#ccc",
                    color: "white",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    fontWeight: "bold",
                    fontSize: "18px"
                  }}
                >
                  {step > s ? "✓" : s}
                </div>
                {s < 4 && (
                  <div style={{
                    width: "50px",
                    height: "4px",
                    backgroundColor: step > s ? "green" : "#ccc"
                  }} />
                )}
              </React.Fragment>
            ))}
          </div>
          <div style={{ display: "flex", justifyContent: "space-between", width: "240px", marginTop: "10px", fontSize: "14px" }}>
            <span>Vehicle</span>
            <span>Proposal</span>
            <span>Insurance</span>
            <span>Documents</span>
          </div>
        </div>

        <form onSubmit={handleSubmit}>
          {step === 1 && (
            <>
              <h4>Vehicle Details</h4>
              {renderDropdown("Vehicle Type:", "vehicle", "vehicleType", vehicleTypes)}
              {renderInput("Vehicle Number:", "vehicle", "vehicleNumber")}
              {renderInput("Chassis Number:", "vehicle", "chassisNumber")}
              {renderInput("Engine Number:", "vehicle", "engineNumber")}
              {renderDropdown("Maker Name:", "vehicle", "makerName", makers, true)}
              {renderInput("Model Name:", "vehicle", "modelName")}
              {renderDropdown("Vehicle Color:", "vehicle", "vehicleColor", colors, true)}
              {renderDropdown("Fuel Type:", "vehicle", "fuelType", fuelTypes)}
              {renderInput("Registration Date:", "vehicle", "registrationDate", "date")}
            </>
          )}

          {step === 2 && (
            <>
              <h4>Proposal Details</h4>
              {renderDropdown("Insurance Type:", "proposal", "insuranceType", insuranceTypes)}
              {renderInput("Insurance Valid Upto:", "proposal", "insuranceValidUpto", "date")}
              {renderInput("Fitness Valid Upto:", "proposal", "fitnessValidUpto", "date")}
            </>
          )}

          {step === 3 && (
            <>
              <h4>Insurance Details</h4>
              {renderInput("Insurance Start Date:", "insuranceDetails", "insuranceStartDate", "date")}
              {renderDropdown("Insurance Sum (₹):", "insuranceDetails", "insuranceSum", insuranceSums.map(sum => sum.toLocaleString('en-IN')))}
              {renderDropdown("Damage Insurance:", "insuranceDetails", "damageInsurance", damageInsuranceOptions)}
              {renderDropdown("Liability Option:", "insuranceDetails", "liabilityOption", liabilityOptions)}
              {renderDropdown("Plan:", "insuranceDetails", "plan", planOptions)}
            </>
          )}

          {step === 4 && (
            <>
              <h4>Upload Documents</h4>
              {renderFileInput("License:", "license")}
              {renderFileInput("RC Book:", "rcBook")}
              {renderFileInput("Pollution Certificate:", "pollutionCertificate")}

            </>
          )}

          <div className="d-flex justify-content-between">
            {step > 1 && <button type="button" className="btn btn-secondary" onClick={prevStep}>Back</button>}
            {step < 4 ? (
              <button type="button" className="btn btn-primary" onClick={nextStep}>Next</button>
            ) : (
              <button type="submit" className="btn btn-success">Submit Proposal</button>
            )}
          </div>
        </form>

        {/* Success Modal */}
        <Modal show={showSuccessModal} onHide={() => setShowSuccessModal(false)} centered>
          <Modal.Header closeButton>
            <Modal.Title>Proposal Submitted Successfully!</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            <p>Your insurance proposal has been successfully submitted.</p>
            <p><strong>Proposal ID:</strong> {successData.proposalId}</p>
            <p><strong>Calculated Premium:</strong> ₹{successData.premium}</p>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="success" onClick={() => setShowSuccessModal(false)}>
              Close
            </Button>
          </Modal.Footer>
        </Modal>

        {submissionResult && !submissionResult.success && (
          <div className="mt-4">
            <div className="alert alert-danger">{submissionResult.error}</div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Proposal;