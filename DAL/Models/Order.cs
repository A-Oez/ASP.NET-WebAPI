namespace DAL.Models
{
    public partial class Order
    {
        public string? Orderdate { get; set; }
        public string Ordernumber { get; set; } = null!;
        public string? UserId { get; set; }
        public string? Article { get; set; }

        [JsonIgnore]
        public virtual Article? ArticleNavigation { get; set; }

        [JsonIgnore]
        public virtual Customer? User { get; set; }
    }
}
