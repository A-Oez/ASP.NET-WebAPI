namespace DAL.Models
{
    public partial class Customer
    {
        public string UserId { get; set; } = null!;
        public string? Forename { get; set; }
        public string? Surename { get; set; }
        public string? Birthday { get; set; }
        public string? City { get; set; }
        public string? Code { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }

        public Customer()
        {
            Orders = new HashSet<Order>();
        }


        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
