namespace StoresManagement.Models
{
    public class EntityUser
    {
        public int EntityId { get; set; }
        public virtual Entity Entity { get; set; }

        public string UserEmail { get; set; }
    }
}