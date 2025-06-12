import React, { useState } from "react";
import registerModel from "../../Models/register";
import { registerClient } from "../../Services/ClientRegister";
import { useNavigate, Link } from "react-router-dom";
import "./LoginRegister.css";

const RegisterPage = () => {
  const [registerData, setRegisterData] = useState(registerModel);
  const [step, setStep] = useState(1);
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();

  const validateStep1 = () => {
    const newErrors = {};
    if (!registerData.name?.trim()) newErrors.name = "Name is required";
    if (!registerData.dateOfBirth) newErrors.dateOfBirth = "Date of Birth is required";
    if (!registerData.gender) newErrors.gender = "Gender is required";
    if (!/^\d{10}$/.test(registerData.phoneNumber)) newErrors.phoneNumber = "Phone must be 10 digits";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const validateStep2 = () => {
    const newErrors = {};
    if (!registerData.email?.trim()) newErrors.email = "Email is required";
    if (!registerData.password) newErrors.password = "Password is required";
    if (!/^\d{12}$/.test(registerData.aadhaarNumber)) newErrors.aadhaarNumber = "Aadhaar must be 12 digits";
    if (!/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/.test(registerData.panNumber)) newErrors.panNumber = "Invalid PAN format";
    if (!registerData.address?.trim()) newErrors.address = "Address is required";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep1()) {
      setStep(2);
      setErrors({});
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    if (!validateStep2()) return;
    try {
      await registerClient(registerData);
      alert("Registration successful! Please login.");
      navigate("/login");
    } catch (err) {
      alert("Registration failed: " + (err.response?.data || err.message));
    }
  };

  return (
    <div className="login-register-wrapper">
      <nav className="navbar navbar-expand-lg navbar-dark fixed-top shadow-sm"  style={{backgroundColor:"#1a2a5a"}}>
        <Link to="/" className="navbar-brand">🏠 Home</Link>
      </nav>
      <div className="auth-card">
        <form onSubmit={handleRegister}>
          <h2>Register</h2>

          {step === 1 && (
            <>
              <div className="form-group">
                <label>Name</label>
                <input
                  type="text"
                  className={`form-control ${errors.name && "is-invalid"}`}
                  value={registerData.name}
                  onChange={(e) => setRegisterData({ ...registerData, name: e.target.value })}
                />
                {errors.name && <div className="invalid-feedback">{errors.name}</div>}
              </div>

              <div className="form-group">
                <label>Date of Birth</label>
                <input
                  type="date"
                  className={`form-control ${errors.dateOfBirth && "is-invalid"}`}
                  value={registerData.dateOfBirth}
                  onChange={(e) => setRegisterData({ ...registerData, dateOfBirth: e.target.value })}
                />
                {errors.dateOfBirth && <div className="invalid-feedback">{errors.dateOfBirth}</div>}
              </div>

              <div className="form-group">
                <label>Gender</label>
                <select
                  className={`form-control ${errors.gender && "is-invalid"}`}
                  value={registerData.gender}
                  onChange={(e) => setRegisterData({ ...registerData, gender: e.target.value })}
                >
                  <option value="">-- Select Gender --</option>
                  <option value="Female">Female</option>
                  <option value="Male">Male</option>
                  <option value="Other">Other</option>
                </select>
                {errors.gender && <div className="invalid-feedback">{errors.gender}</div>}
              </div>

              <div className="form-group">
                <label>Phone Number</label>
                <input
                  type="tel"
                  className={`form-control ${errors.phoneNumber && "is-invalid"}`}
                  value={registerData.phoneNumber}
                  onChange={(e) => setRegisterData({ ...registerData, phoneNumber: e.target.value })}
                />
                {errors.phoneNumber && <div className="invalid-feedback">{errors.phoneNumber}</div>}
              </div>

              <br />
              <button type="button" className="btn" style={{ backgroundColor: "#1a2a5a", color: "white" }} onClick={handleNext}>Next</button>
            </>
          )}

          {step === 2 && (
            <>
              <div className="form-group">
                <label>Email</label>
                <input
                  type="email"
                  className={`form-control ${errors.email && "is-invalid"}`}
                  value={registerData.email}
                  onChange={(e) => setRegisterData({ ...registerData, email: e.target.value })}
                />
                {errors.email && <div className="invalid-feedback">{errors.email}</div>}
              </div>

              <div className="form-group">
                <label>Password</label>
                <input
                  type="password"
                  className={`form-control ${errors.password && "is-invalid"}`}
                  value={registerData.password}
                  onChange={(e) => setRegisterData({ ...registerData, password: e.target.value })}
                />
                {errors.password && <div className="invalid-feedback">{errors.password}</div>}
              </div>

              <div className="form-group">
                <label>Aadhaar Number</label>
                <input
                  type="text"
                  className={`form-control ${errors.aadhaarNumber && "is-invalid"}`}
                  maxLength="12"
                  value={registerData.aadhaarNumber}
                  onChange={(e) => setRegisterData({ ...registerData, aadhaarNumber: e.target.value })}
                />
                {errors.aadhaarNumber && <div className="invalid-feedback">{errors.aadhaarNumber}</div>}
              </div>

              <div className="form-group">
                <label>PAN Number</label>
                <input
                  type="text"
                  className={`form-control ${errors.panNumber && "is-invalid"}`}
                  maxLength="10"
                  value={registerData.panNumber}
                  onChange={(e) => setRegisterData({ ...registerData, panNumber: e.target.value })}
                />
                {errors.panNumber && <div className="invalid-feedback">{errors.panNumber}</div>}
              </div>

              <div className="form-group">
                <label>Address</label>
                <textarea
                  className={`form-control ${errors.address && "is-invalid"}`}
                  rows="3"
                  value={registerData.address}
                  onChange={(e) => setRegisterData({ ...registerData, address: e.target.value })}
                ></textarea>
                {errors.address && <div className="invalid-feedback">{errors.address}</div>}
              </div>

              <br />
              <div className="d-flex justify-content-between">
                <button type="button" className="btn" style={{ backgroundColor: "#1a2a5a", color: "white" }} onClick={() => setStep(1)}>Back</button>
                <button type="submit" className="btn" style={{ backgroundColor: "#1a2a5a", color: "white" }}>Register</button>
              </div>

              <br />
              <div className="switch-text">
                Already registered? <Link to="/login"><span>Login</span></Link>
              </div>
            </>
          )}
        </form>
      </div>
    </div>
  );
};

export default RegisterPage;
