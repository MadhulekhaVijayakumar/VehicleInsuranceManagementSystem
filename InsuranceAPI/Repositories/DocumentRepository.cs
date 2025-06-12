using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class DocumentRepository : Repository<int, Document>
    {
        public DocumentRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<IEnumerable<Document>> GetAll()
        {
            return await _context.Documents.ToListAsync();
        }

        public override async Task<Document> GetById(int key)
        {
            var document = await _context.Documents.FindAsync(key);
            if (document == null)
                throw new Exception($"Document with ID {key} not found");

            return document;
        }
    }

}
