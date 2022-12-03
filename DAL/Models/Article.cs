namespace DAL.Models
{
    public partial class Article
    {
        public string Articlenumber { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        public Article()
        {
            Orders = new HashSet<Order>();
        }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
