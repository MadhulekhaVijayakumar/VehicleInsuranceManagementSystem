import React, { useEffect, useState } from "react";
import { getClientProfile, updateClientProfile } from "../../Services/ClientService";
import { FaEdit, FaSave } from "react-icons/fa";

const ProfilePage = () => {
  const [profile, setProfile] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [editedProfile, setEditedProfile] = useState({});
  const [errors, setErrors] = useState({});

  const fetchProfile = async () => {
    try {
      const token = localStorage.getItem("token");
      const data = await getClientProfile(token);
      setProfile(data);
      setEditedProfile(data);
    } catch (error) {
      console.error("Error fetching client profile:", error);
    }
  };

  const validateFields = () => {
    const newErrors = {};
    if (!editedProfile.name?.trim()) newErrors.name = "Name is required";
    if (!/^\d{10}$/.test(editedProfile.phoneNumber)) newErrors.phoneNumber = "Phone must be 10 digits";
    if (!/^\d{12}$/.test(editedProfile.aadhaarNumber)) newErrors.aadhaarNumber = "Aadhaar must be 12 digits";
    if (!/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/.test(editedProfile.panNumber)) newErrors.panNumber = "Invalid PAN format";
    if (!editedProfile.address?.trim()) newErrors.address = "Address is required";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const saveProfile = async () => {
    if (!validateFields()) return;
    try {
      const token = localStorage.getItem("token");
      const updated = await updateClientProfile(token, editedProfile);
      setProfile(updated);
      setEditedProfile(updated);
      setIsEditing(false);
      setErrors({});
    } catch (error) {
      console.error("Error updating profile:", error);
    }
  };

  useEffect(() => {
    fetchProfile();
  }, []);

  if (!profile) return <p>Loading profile...</p>;

  return (

    <div className="card shadow-sm"  style={{maxWidth: '800px', margin: '2rem auto'}}>
      <div className="card-body">
        <div className="d-flex justify-content-between align-items-center">
          <h4 className="fw-semibold  mb-4" style={{color:"#1a2a5a"}}>👤 Profile</h4>
          <button className="btn btn-sm btn-outline-primary" onClick={() => isEditing ? saveProfile() : setIsEditing(true)}>
            {isEditing ? <><FaSave /> Save</> : <><FaEdit /> Edit</>}
          </button>
        </div>
        <hr />
        <div className="row  g-3">
          {/* Name */}
          <div className="col-md-6 mb-3">
            <label>Name:</label>
            <input type="text" className={`form-control form-control-sm ${errors.name && 'is-invalid'}`} value={editedProfile.name} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, name: e.target.value })} />
            {errors.name && <div className="invalid-feedback">{errors.name}</div>}
          </div>

          {/* Date of Birth */}
          <div className="col-md-6 mb-3">
            <label>DOB:</label>
            <input type="date" className="form-control form-control-sm" value={editedProfile.dateOfBirth.slice(0, 10)} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, dateOfBirth: e.target.value })} />
          </div>

          {/* Gender */}
          <div className="col-md-6 mb-3">
            <label>Gender:</label>
            <input type="text" className="form-control form-control-sm" value={editedProfile.gender} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, gender: e.target.value })} />
          </div>

          {/* Phone */}
          <div className="col-md-6 mb-3">
            <label>Phone:</label>
            <input type="text" className={`form-control form-control-sm  ${errors.phoneNumber && 'is-invalid'}`} value={editedProfile.phoneNumber} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, phoneNumber: e.target.value })} />
            {errors.phoneNumber && <div className="invalid-feedback">{errors.phoneNumber}</div>}
          </div>

          {/* Aadhaar */}
          <div className="col-md-6 mb-3">
            <label>Aadhaar:</label>
            <input type="text" className={`form-control form-control-sm  ${errors.aadhaarNumber && 'is-invalid'}`} value={editedProfile.aadhaarNumber} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, aadhaarNumber: e.target.value })} />
            {errors.aadhaarNumber && <div className="invalid-feedback">{errors.aadhaarNumber}</div>}
          </div>

          {/* PAN */}
          <div className="col-md-6 mb-3">
            <label>PAN:</label>
            <input type="text" className={`form-control form-control-sm ${errors.panNumber && 'is-invalid'}`} value={editedProfile.panNumber} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, panNumber: e.target.value })} />
            {errors.panNumber && <div className="invalid-feedback">{errors.panNumber}</div>}
          </div>

          {/* Address */}
          <div className="col-12 mb-3">
            <label>Address:</label>
            <textarea className={`form-control form-control-sm ${errors.address && 'is-invalid'}`} rows={2} value={editedProfile.address} disabled={!isEditing}
              onChange={e => setEditedProfile({ ...editedProfile, address: e.target.value })}></textarea>
            {errors.address && <div className="invalid-feedback">{errors.address}</div>}
          </div>

          {/* Email (read-only) */}
          <div className="col-12 mb-3">
            <label>Email:</label>
            <input type="email" className="form-control form-control-sm" value={profile.email} disabled />
          </div>
        </div>
      </div>
    </div>
   
  );
};

export default ProfilePage;
