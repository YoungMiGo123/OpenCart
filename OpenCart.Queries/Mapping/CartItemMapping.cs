using AutoMapper;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Operations.Mapping
{
    public class CartItemMapping : Profile
    {
        public CartItemMapping()
        {
            CreateMap<CartItem, CartItemDto>();
            CreateMap<CartItemDto, CartItem>();
        }
    }
 
}
