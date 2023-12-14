using Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Auction.Services
{
    public class CheckService: ICheckService
    {
        private readonly AuctionContext _context;

        public CheckService(AuctionContext context)
        {
            _context = context;
        }

        public void checkStart() 
        {
            List<Product> products = _context.Product.ToList();
            foreach (Product product in products)
            {
                if (product.Status == "New") 
                {
                    if (product.StartDate <= DateTime.Now)
                    {
                        product.Status = "Active";
                    }
                }
                if (product.Status == "Active") 
                {
                    if (product.FinishDate <= DateTime.Now)
                    {
                        product.Status = "Finished";
                        List<Bet> bets = _context.Bet.Include(x => x.Player).Where(x => x.Product.Id == product.Id).ToList();
                        if (bets.Count!=0) 
                        {
                            product.Buyer = bets.Last().Player;
                        }
                       
                    }
                }
                    
            }
            _context.SaveChanges();
        }
    }
}
