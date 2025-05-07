using VinylStore.Data.Interfaces.Models;
using System.Numerics;

namespace VinylStore.Data.Models
{
    public class VinylData : BaseModel, IVinylData
    {
        public string Name { get; set; }
        public string Executor { get; set; }

        public string Genre { get; set; }

        public decimal PurchasePrice { get; set; }

        public int Count { get; set; }


    }
}
