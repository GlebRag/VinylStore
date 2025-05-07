
namespace VinylStore.Models.Vinyl
{
    public class VinylViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Executor { get; set; }

        public string Genre { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int Count { get; set; }
    }
}