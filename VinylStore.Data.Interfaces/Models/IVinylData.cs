using System.Numerics;

namespace VinylStore.Data.Interfaces.Models
{
    public interface IVinylData : IBaseModel
    {
        public string Name { get; set; }
        public string Executor { get; set; }

        public string Genre { get; set; }
        public decimal PurchasePrice { get; set; }

        public int Count { get; set; }
    }
}
