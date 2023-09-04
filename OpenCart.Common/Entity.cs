namespace OpenCart.Common
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
