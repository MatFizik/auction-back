using Auction.Enum;
using Auction.Models;

namespace Auction.Dto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SalesmanId { get; set; }
        public int CategoryId{ get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public double StartPrice { get; set; }
        public double Step { get; set; }
    }
}
