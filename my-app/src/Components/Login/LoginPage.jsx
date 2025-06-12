import React, { useState, useEffect } from "react";
import { loginUser } from "../../Services/AuthenticationService";
import loginModel from "../../Models/login";
import { useNavigate, Link } from "react-router-dom";
import "./LoginRegister.css";

const LoginPage = () => {
  const [loginData, setLoginData] = useState(loginModel);
  const [isLoading, setIsLoading] = useState(false); // Prevent double submission
  const navigate = useNavigate();
  useEffect(() => {
    // Check if the user is already logged in based on sessionStorage
    const user = JSON.parse(sessionStorage.getItem("user"));
    if (user && user.token) {
      if (user.role === "Client") {
        navigate("/client-dashboard", { replace: true });  // Using replace to prevent the back button from going to login
      } else if (user.role === "Admin") {
        navigate("/admin-dashboard", { replace: true });
      }
    }
  }, []);  // Empty dependency array ensures this runs only once when the component is mounted
  
 

  const handleLogin = async (e) => {
    e.preventDefault();
    if (isLoading) return; // Prevent double-clicking the login button
    setIsLoading(true);

    try {
      const response = await loginUser(loginData);
      alert("Login successful!");
      console.log("Login successful, role:", response.role);

      // Redirect after successful login
      if (response.role === "Client") navigate("/client-dashboard");
      else if (response.role === "Admin") navigate("/admin-dashboard");
    } catch (error) {
      alert("Login failed. Please check your username/password.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-register-wrapper">
      <nav className="navbar navbar-expand-lg navbar-dark fixed-top shadow-sm" style={{backgroundColor:"#1a2a5a"}}>
        <Link to="/" className="navbar-brand">🏠 Home</Link>
      </nav>
      <div className="auth-card">
        <form onSubmit={handleLogin}>
          <h2>Login</h2>
          <div className="form-group">
            <label>Username</label>
            <input
              type="text"
              className="form-control"
              value={loginData.username}
              onChange={(e) => setLoginData({ ...loginData, username: e.target.value })}
              required
              autoComplete="username" 
            />
          </div>
          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              className="form-control"
              value={loginData.password}
              onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
              required
              autoComplete="current-password"
            />
          </div>
          <div className="text-end">
            <Link to="/forgot-password" className="forgot-password">Forgot password?</Link>
          </div>
          <button type="submit" className="btn" style={{ backgroundColor: "#1a2a5a", color: "white" }} disabled={isLoading}>
            {isLoading ? 'Logging in...' : 'Login'}
          </button>
          <div className="switch-text">
            New user? <Link to="/register"><span>Sign up</span></Link>
          </div>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
