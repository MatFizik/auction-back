using Auction.Models;

namespace Auction.Dto
{
    public class BetDto
    {
        public int PlayerId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}