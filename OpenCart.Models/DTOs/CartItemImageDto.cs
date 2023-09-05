namespace OpenCart.Models.DTOs
{
    public class CartItemImageDto
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public byte[]? FileBytes { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public long Length { get; set; }
        public string? Name { get; set; }
    }
}
