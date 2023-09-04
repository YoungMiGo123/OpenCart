using OpenCart.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Models.Entities
{
    public class CartItemImage : Entity
    {
        public string? FileName { get; set; }
        public byte[]? FileBytes { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public long Length { get; set; }
        public string? Name { get; set; }
    }
}
