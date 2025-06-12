import React, { useEffect, useState } from 'react';
import AllClient from "../../Services/AllClient"

const ClientList = () => {
  const [clients, setClients] = useState([]);
  const [totalRecords, setTotalRecords] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchClients = async (pageNumber, pageSize) => {
    setLoading(true);
    try {
      const result = await AllClient.getAllClients(pageNumber, pageSize);
      setClients(result.data);
      setTotalRecords(result.totalRecords);
      setCurrentPage(result.currentPage);
    } catch (error) {
      setError("Failed to load clients.");
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (newPage) => {
    setCurrentPage(newPage);
    fetchClients(newPage, pageSize);
  };

  const handlePageSizeChange = (event) => {
    setPageSize(event.target.value);
    fetchClients(currentPage, event.target.value);
  };

  useEffect(() => {
    fetchClients(currentPage, pageSize);
  }, [currentPage, pageSize]);

  return (
    <div className="container">
      <h2>Client List</h2>
      
      {error && <div className="alert alert-danger">{error}</div>}
      
      {loading ? (
        <div>Loading...</div>
      ) : (
        <>
          <table className="table">
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Email</th>
                <th>Phone Number</th>
              </tr>
            </thead>
            <tbody>
            {clients.map((client) => (
  <tr key={client.id}>
                  <td>{client.id}</td>
                  <td>{client.name}</td>
                  <td>{client.email}</td>
                  <td>{client.phoneNumber}</td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="pagination">
            <button
              onClick={() => handlePageChange(currentPage - 1)}
              disabled={currentPage === 1}
            >
              Previous
            </button>
            <span>{currentPage}</span>
            <button
              onClick={() => handlePageChange(currentPage + 1)}
              disabled={currentPage * pageSize >= totalRecords}
            >
              Next
            </button>

            <select onChange={handlePageSizeChange} value={pageSize}>
              <option value={5}>5</option>
              <option value={10}>10</option>
              <option value={20}>20</option>
            </select>
          </div>
        </>
      )}
    </div>
  );
};

export default ClientList;
