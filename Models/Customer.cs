namespace StoresManagement.Models
{
    public class Customer
    {
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public string Identification { get; set; }

        public string Name { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
    }
}