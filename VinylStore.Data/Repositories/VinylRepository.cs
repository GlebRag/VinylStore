using VinylStore.Data.Models;
using VinylStore.Data;
using Microsoft.EntityFrameworkCore;
using VinylStore.Data.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using VinylStore.Data.Interfaces.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Net;
using System.Globalization;


namespace VinylStore.Data.Repositories
{
    public interface IVinylRepositoryReal : IVinylRepository<VinylData>
    {

        void Create(VinylData dataVinyl);
        IEnumerable<VinylData> SearchVinyl(string name, string executor, string genre, string purchasePrice, string totalPrice, int count);
        void Update(VinylData dataVinyl, int vinylId);

    }

    public class VinylRepository : BaseRepository<VinylData>, IVinylRepositoryReal
    {
        public VinylRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        public void Create(VinylData dataVinyl)
        {
            Add(dataVinyl);
        }


        public IEnumerable<VinylData> SearchVinyl(string name, string executor, string genre, string purchasePrice, string totalPrice, int count)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.Vinyl WHERE 1=1");

            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND Name = @Name");
                parameters.Add(new SqlParameter("@Name", name));
            }

            if (!string.IsNullOrEmpty(executor))
            {
                sql.Append(" AND Executor = @Executor");
                parameters.Add(new SqlParameter("@Executor", executor));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                sql.Append(" AND Genre = @Genre");
                parameters.Add(new SqlParameter("@Genre", genre));
            }

            // Поиск по PurchasePrice (столбец типа decimal)
            if (!string.IsNullOrEmpty(purchasePrice))
            {
                decimal purchasePriceValue;
                // Для преобразования заменяем запятую на точку, чтобы использовать InvariantCulture
                if (decimal.TryParse(purchasePrice.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out purchasePriceValue))
                {
                    sql.Append(" AND PurchasePrice = @PurchasePrice");
                    parameters.Add(new SqlParameter("@PurchasePrice", purchasePriceValue));
                }
            }

            // Поиск по Count (столбец типа int)
            // Если count больше 0, считаем, что его вводили для фильтрации
            if (count > 0)
            {
                sql.Append(" AND Count = @Count");
                parameters.Add(new SqlParameter("@Count", count));
            }

            // Поиск по totalPrice, который вычисляется по формуле PurchasePrice*Count
            if (!string.IsNullOrEmpty(totalPrice))
            {
                decimal totalPriceValue;
                if (decimal.TryParse(totalPrice.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out totalPriceValue))
                {
                    sql.Append(" AND (PurchasePrice * Count) = @TotalPrice");
                    parameters.Add(new SqlParameter("@TotalPrice", totalPriceValue));
                }
            }

            var result = _webDbContext
                .Database
                .SqlQueryRaw<VinylData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }

        public void Update(VinylData dataVinyl, int vinylId)
        {
            var vinyl = _dbSet.First(x => x.Id == vinylId);
            vinyl.Name = dataVinyl.Name;
            vinyl.Executor = dataVinyl.Executor;
            vinyl.Genre = dataVinyl.Genre;
            vinyl.PurchasePrice = dataVinyl.PurchasePrice;
            vinyl.Count = dataVinyl.Count;
            _webDbContext.SaveChanges();
        }


    }
}
