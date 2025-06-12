using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class ClientRepository: Repository<int,Client>
    {
        public ClientRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<Client> GetById(int key)
        {
            var client = await _context.Clients
                .SingleOrDefaultAsync(e => e.Id == key);

            if (client == null)
                throw new Exception($"Client with ID {key} not present");

            return client;
        }

        public override async Task<IEnumerable<Client>> GetAll()
        {

            var clients = _context.Clients.Include(e => e.User);
            if (clients.Count() == 0)
                throw new Exception("No clients found");

            return clients;
        }
    }
}

    

