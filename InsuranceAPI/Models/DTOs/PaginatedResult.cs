namespace InsuranceAPI.Models.DTOs
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
