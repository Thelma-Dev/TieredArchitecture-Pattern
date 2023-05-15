namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IRepository<T> where T : class
    {
        void Create(T entity);

        T? Get(int? id);

        ICollection<T> GetAll();

        void Update(T entity);

        void Delete(T entity);

        void SaveChanges();

    }
}
