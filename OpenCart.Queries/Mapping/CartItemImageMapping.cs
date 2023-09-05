using AutoMapper;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;

namespace OpenCart.Operations.Mapping
{
    public class CartItemImageMapping : Profile
    {
        public CartItemImageMapping()
        {
            CreateMap<CartItemImage, CartItemImageDto>();
            CreateMap<CartItemDto, CartItem>();
        }
    }
}
