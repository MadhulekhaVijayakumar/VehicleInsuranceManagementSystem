import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import LoginPage from './Components/Login/LoginPage';
import RegisterPage from './Components/Login/RegisterPage';
import HomePage from './Components/Home/Home';
import ClientDashboard from './Components/Client/ClientDashboard';
import AdminDashboard from './Components/Admin/AdminDashboard';

// Protects routes from unauthenticated access
const ProtectedRoute = ({ element }) => {
  const user = JSON.parse(sessionStorage.getItem("user")) || localStorage.getItem("token");
  return user ? element : <Navigate to="/login" />;
};

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/client-dashboard" element={<ProtectedRoute element={<ClientDashboard />} />} />
        <Route path="/admin-dashboard" element={<AdminDashboard />} />
      </Routes>
    </Router>
  );
}

export default App;
