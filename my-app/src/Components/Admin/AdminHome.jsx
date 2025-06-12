import React, { useEffect, useState } from "react";
import { Card, Row, Col, Table, Spinner } from "react-bootstrap";
import {
  FiUsers, FiFileText, FiAlertCircle, FiUpload, FiDollarSign
} from "react-icons/fi";
import { getAdminSummary } from "../../Services/AdminDashboardService";
import AdminSummaryModel from "../../Models/AdminSummaryModel";
import "./Admin.css";
import RecentActivities from "./RecentActivities";

const AdminHome = () => {
  const [summary, setSummary] = useState(new AdminSummaryModel());
  const [loading, setLoading] = useState(true);

  const fetchSummary = async () => {
    try {
      const token = localStorage.getItem("token");
      const data = await getAdminSummary(token);
      const populatedSummary = Object.assign(new AdminSummaryModel(), data);
      setSummary(populatedSummary);
    } catch (err) {
      console.error("Failed to load admin summary", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSummary();
  }, []);

  const stats = [
    { title: "Total Clients", value: summary.totalClients, icon: <FiUsers size={30} />, color: "primary" },
    { title: "Pending Proposals", value: summary.pendingProposals, icon: <FiFileText size={30} />, color: "warning" },
    { title: "Claims to Review", value: summary.claimsToReview, icon: <FiAlertCircle size={30} />, color: "danger" },
    { title: "Total Revenue", value: `$${summary.totalRevenue.toLocaleString()}`, icon: <FiDollarSign size={30} />, color: "success" },
  ];

 

  return (
    <div className="container-fluid py-4">
      <h2 className="mb-4">Admin Dashboard</h2>

      {loading ? (
        <div className="text-center">
          <Spinner animation="border" variant="primary" />
        </div>
      ) : (
        <>
          <Row className="stats-container"> {/* or "stats-grid" for Option 2 */}
  {stats.map((stat, index) => (
    <Col key={index} className="stat-card">
      <Card className={`border-0 shadow-sm bg-${stat.color}-light h-100`}> {/* h-100 makes card full height */}
        <Card.Body className="stat-card-body">
          <div className="stat-content">
            <h6 className="text-muted mb-2">{stat.title}</h6>
            <h3 className="mb-0">{stat.value}</h3>
          </div>
          <div className={`icon-shape icon-lg bg-${stat.color} text-white rounded-circle mt-3`}>
            {stat.icon}
          </div>
        </Card.Body>
      </Card>
    </Col>
  ))}
</Row>

          <Row className="mb-4">
            <Col>
              <Card className="border-0 shadow-sm">
                <Card.Body>
                  <h5 className="mb-3">Quick Actions</h5>
                  <div className="d-flex flex-wrap gap-2">
                    <button className="btn btn-outline-primary">Review Proposals</button>
                    <button className="btn btn-outline-danger">Process Claims</button>
                    <button className="btn btn-outline-success">Add New Admin</button>
                    <button className="btn btn-outline-info">Generate Reports</button>
                    <button className="btn btn-outline-warning">Send Notifications</button>
                  </div>
                </Card.Body>
              </Card>
            </Col>
          </Row>

          <Row>
  <Col>
    <RecentActivities />
  </Col>
</Row>

        </>
      )}
    </div>
  );
};

export default AdminHome;
