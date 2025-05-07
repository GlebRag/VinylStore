using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using VinylStore.Data;
using VinylStore.Data.Models;
using VinylStore.Data.Repositories;
using VinylStore.Models;
using VinylStore.Models.Vinyl;

namespace VinylStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IVinylRepositoryReal _vinylRepository;
        private WebDbContext _webDbContext;
        public HomeController(ILogger<HomeController> logger,
            IVinylRepositoryReal vinylRepository,
            WebDbContext webDbContext)
        {
            _vinylRepository = vinylRepository;
            _logger = logger;
            _webDbContext = webDbContext;
        }

        public IActionResult Index(string sort, string dir)
        {
            // ≈сли не задан столбец сортировки, по умолчанию сортируем по названию
            if (string.IsNullOrEmpty(sort))
            {
                sort = "name";
            }
            // ≈сли направление сортировки не задано, по умолчанию ASC
            if (string.IsNullOrEmpty(dir))
            {
                dir = "asc";
            }

            IEnumerable<VinylData> vinylData = _vinylRepository.GetAll();

            switch (sort.ToLower())
            {
                case "name":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.Name) : vinylData.OrderByDescending(v => v.Name);
                    break;
                case "executor":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.Executor) : vinylData.OrderByDescending(v => v.Executor);
                    break;
                case "genre":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.Genre) : vinylData.OrderByDescending(v => v.Genre);
                    break;
                case "purchaseprice":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.PurchasePrice) : vinylData.OrderByDescending(v => v.PurchasePrice);
                    break;
                case "count":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.Count) : vinylData.OrderByDescending(v => v.Count);
                    break;
                case "totalprice":
                    vinylData = (dir.ToLower() == "asc") ? vinylData.OrderBy(v => v.PurchasePrice * v.Count) : vinylData.OrderByDescending(v => v.PurchasePrice * v.Count);
                    break;
                default:
                    break;
            }

            var model = vinylData.Select(v => new VinylViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Executor = v.Executor,
                Genre = v.Genre,
                PurchasePrice = v.PurchasePrice,
                Count = v.Count,
                TotalPrice = v.PurchasePrice * v.Count
            }).ToList();

            ViewBag.CurrentSort = sort;
            ViewBag.CurrentDir = dir.ToLower();


            return View("Index", model);
        }



        [HttpGet]
        public IActionResult CreateVinyl()
        {

            var viewModel = new VinylCreationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateVinyl(VinylCreationViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataVinyl = new VinylData
            {
                Name = viewModel.Name,
                Executor = viewModel.Executor,
                Genre = viewModel.Genre,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count,
            };

            _vinylRepository.Create(dataVinyl);


            return RedirectToAction("Index");
        }

        public IActionResult DeleteVinyl(int vinylId)
        {

            _vinylRepository.Delete(vinylId);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditVinyl(int vinylId)
        {

            var viewModel = new VinylEditViewModel();

            var vinyl = _webDbContext.Vinyl.First(x => x.Id == vinylId);
            viewModel.Name = vinyl.Name;
            viewModel.Executor = vinyl.Executor;
            viewModel.Genre = vinyl.Genre;
            viewModel.PurchasePrice = vinyl.PurchasePrice;
            viewModel.Count = vinyl.Count;
            viewModel.Id = vinylId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditVinyl(VinylEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var vinylId = viewModel.Id;


            var dataVinyl = new VinylData
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Executor = viewModel.Executor,
                Genre = viewModel.Genre,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count
                
            };


            _vinylRepository.Update(dataVinyl, vinylId);



            return RedirectToAction("Index");
        }
        public IActionResult SearchVinyl(string name, string executor, string genre, string purchasePrice, string totalPrice, int count)
        {

            var viewModels = _vinylRepository
                .SearchVinyl(name,  executor,  genre,  purchasePrice,  totalPrice,  count)
                .Select(dbBook => new VinylViewModel
                {

                    Id = dbBook.Id,
                    Name = dbBook.Name,
                    Executor = dbBook.Executor,
                    Genre = dbBook.Genre,
                    PurchasePrice = dbBook.PurchasePrice,
                    Count = dbBook.Count,
                    TotalPrice = dbBook.PurchasePrice * dbBook.Count

                })
                .ToList();

            return View("Index", viewModels);
        }
        public IActionResult Create50Vinyl()
        {
            var random = new Random();

            // —писки дл€ случайного выбора
            string[] names = new[]
            {
        "Abbey Road", "Back in Black", "Rumours", "Thriller", "The Dark Side of the Moon",
        "Hotel California", "Led Zeppelin IV", "Kind of Blue", "Sgt. Pepper's Lonely Hearts Club Band", "The Wall"
    };

            string[] executors = new[]
            {
        "The Beatles", "AC/DC", "Fleetwood Mac", "Michael Jackson", "Pink Floyd",
        "Eagles", "Led Zeppelin", "Miles Davis", "Queen", "David Bowie"
    };

            string[] genres = new[]
            {
        "Rock", "Pop", "Jazz", "R&B", "Classical", "Blues", "Hip-Hop", "Electronic", "Indie", "Folk"
    };

            // ƒиапазоны дл€ цены и количества
            decimal priceMin = 5.00M;
            decimal priceMax = 50.00M;

            for (int i = 0; i < 50; i++)
            {
                // ¬ыбор случайных значений из массивов
                string randomName = names[random.Next(names.Length)];
                string randomExecutor = executors[random.Next(executors.Length)];
                string randomGenre = genres[random.Next(genres.Length)];

                // √енераци€ случайной цены типа decimal
                decimal randomPrice = Math.Round((decimal)(random.NextDouble() * ((double)priceMax - (double)priceMin) + (double)priceMin), 2);

                // √енераци€ количества в диапазоне от 1 до 20
                int randomCount = random.Next(1, 21);

                var dataModel = new VinylData
                {
                    Name = randomName,
                    Executor = randomExecutor,
                    Genre = randomGenre,
                    PurchasePrice = randomPrice,
                    Count = randomCount
                };

                _vinylRepository.Add(dataModel);
            }

            return RedirectToAction("Index");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
