using Auction.Dto;
using Auction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
namespace Auction.Controllers
{
    public class ProductController
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AuctionContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ProductController(IWebHostEnvironment webHostEnvironment, AuctionContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpPost("test")]
        public ResponseDto test() 
        {
            try
            {
                var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
                var contents = provider.GetDirectoryContents(Path.Combine("Products"));
                var objFiles = contents.OrderBy(m => m.LastModified).ToArray();
                User user = _context.User.FirstOrDefault(x => x.Id == 2);
                Category category = _context.Category.FirstOrDefault(x => x.Id == 1);
                Random rnd = new Random();
                foreach (var photo in objFiles)
                {
                    _context.Add(new Product
                    {
                        Name = photo.Name,
                        Description = $"Продваю машину {photo.Name} в отличном состоянии. Не бит, не крашен!",
                        Salesman = user,
                        Category = category,
                        PhotoUrl = "/Products/" + photo.Name,
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now.AddMinutes(30),
                        StartPrice = rnd.Next(1000, 5000),
                        Step = 100
                    });
                }
                _context.SaveChanges();
                return new ResponseDto(0, "Success");
            }
            catch (Exception ex)
            {

                return new ResponseDto(-1, ex.Message);
            }
           
        }
        [HttpPost("api/createAuction")]
        public async Task<ResponseDto> createAuction([FromForm] ProductDto product1) 
        {
            try
            {
                Product product = new Product();
                product.Name = product1.Name;
                product.Description = product1.Description;
                product.FinishDate = product1.FinishDate.AddHours(6);
                product.StartDate = product1.StartDate.AddHours(6);
                product.Step = product1.Step;
                product.PhotoUrl = product1.PhotoUrl;
                product.StartPrice = product1.StartPrice;
                Category category = _context.Category.FirstOrDefault(x=>x.Id==product1.CategoryId);
                User Salesman = _context.User.Include(x=>x.Role).FirstOrDefault(x=>x.Id==product1.SalesmanId);
                product.Salesman = Salesman;
                product.Category = category;
                string fileName = $"{Guid.NewGuid()}.jpg";
                
                string filepath = "D:/Учеба/4 курс/Разрабы вместе/auction/src/components/Assets/img/" + fileName;
                var bytess = Convert.FromBase64String(product.PhotoUrl);
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();
                }
                string imageUrl =  "../Assets/img/" + fileName;
                product.Status = "New";
                product.PhotoUrl = product1.PhotoUrl;
                _context.Product.Add(product);
                _context.SaveChanges();
                return new ResponseDto(0, "Success");
            }
            catch (Exception ex)
            {

                return new ResponseDto(-1, ex.Message);
            }
        }



        [HttpGet("api/getAuctions")]
        public ResponseDto<List<Product>> getAuctions() 
        {
            try
            {
                List<Product> auctions = _context.Product.Include(x=>x.Salesman).Include(x => x.Category).ToList();
                return new ResponseDto<List<Product>>(0, "Success", auctions);
            }
            catch (Exception ex)
            {

                return new ResponseDto<List<Product>>(-1, ex.Message, null);
            }
        }

        [HttpPost("api/createBet")]
        public ResponseDto createBet([FromForm] BetDto bet1)
        {
            try
            {
                bet1.Date.AddHours(6);
                Bet bet = new Bet();
                bet.Date = bet1.Date;
                bet.Price = bet1.Price;
                Product auction = _context.Product.Include(x=>x.Salesman).FirstOrDefault(x => x.Id == bet1.ProductId);
                if (auction.Salesman.Id == bet1.PlayerId)
                {
                    return new ResponseDto(2, "Создатель не может участвовать в своём аукционе");
                }
                if (auction.Status == "New")
                {
                    return new ResponseDto(2, "Аукцион ещё не начался");
                }
                if (auction.Status == "Finished")
                {
                    return new ResponseDto(2, "Аукцион уже закончился");
                }
                List<Bet> bets = _context.Bet.Where(x => x.Product.Id == bet1.ProductId).ToList();
                if (bets.Count == 0)
                {
                    if (auction.StartPrice >= bet.Price)
                    {
                        return new ResponseDto(1, "Ставка должна быть больше начальной");
                    }
                }
                else          
                {
                    if (bets.Last().Price >= bet.Price)
                    {
                        return new ResponseDto(1, "Ставка должна быть больше последней");
                    }
                }
                
                bet.Player = _context.User.FirstOrDefault(x => x.Id == bet1.PlayerId);
                bet.Product = auction;
                _context.Bet.Add(bet);
                auction.FinishPrice = bet.Price;
                _context.SaveChanges();
                return new ResponseDto(0, "Success");
            }
            catch (Exception ex)
            {

                return new ResponseDto(-1, ex.Message);
            }
        }

        [HttpPost("api/getBets")]
        public ResponseDto<List<Bet>> getBets(int auctionId) 
        {
            try
            {
                List<Bet> bets = _context.Bet.Include(x=>x.Player).Include(x=>x.Product).Where(x=>x.Product.Id==auctionId).ToList();
                return new ResponseDto<List<Bet>>(-1, "Success", bets);
            }
            catch (Exception ex)
            {

                return new ResponseDto<List<Bet>>(-1, ex.Message, null);
            }
        }

    }
}
