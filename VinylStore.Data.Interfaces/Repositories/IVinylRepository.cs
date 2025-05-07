using VinylStore.Data.Interfaces.Models;

namespace VinylStore.Data.Interfaces.Repositories
{
    public interface IVinylRepository<T> : IBaseRepository<T>
        where T : IVinylData
    {
    }
}
