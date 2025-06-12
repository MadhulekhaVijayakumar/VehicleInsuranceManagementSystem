namespace InsuranceAPI.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> Add(T entity);
        Task<T> Update(K key, T entity);
        Task<T> Delete(K key);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(K key);
    }
}
