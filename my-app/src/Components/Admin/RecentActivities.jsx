import React, { useEffect, useState } from 'react';
import { Card, Table, Spinner } from 'react-bootstrap';
import { getRecentActivities } from '../../Services/ActivityService';

const RecentActivities = () => {
  const [activities, setActivities] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const token = localStorage.getItem("token");
        const data = await getRecentActivities(token);
        setActivities(data);
      } catch (error) {
        console.error("Failed to fetch recent activities", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  return (
    <Card className="border-0 shadow-sm mt-4">
      <Card.Body>
        <h5 className="mb-3">Recent Activities</h5>
        {loading ? (
          <div className="text-center">
            <Spinner animation="border" variant="primary" />
          </div>
        ) : (
          <div className="table-responsive">
            <Table hover className="mb-0">
              <thead>
                <tr>
                  <th>User</th>
                  <th>Action</th>
                  <th>Time</th>
                </tr>
              </thead>
              <tbody>
                {activities.map((activity, index) => (
                  <tr key={index}>
                    <td>{activity.username}</td>
                    <td>{activity.action}</td>
                    <td>{new Date(activity.timestamp).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </div>
        )}
      </Card.Body>
    </Card>
  );
};

export default RecentActivities;
