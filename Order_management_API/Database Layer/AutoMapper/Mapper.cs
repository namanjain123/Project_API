using AutoMapper;
using Database_Layer.DTOs;
using Database_Layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Layer.AutoMapper
{
    public static class Mapper
    {
        public static IMapper Mappers;
        static Mapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Order, OrderDTO>();
                cfg.CreateMap<Item, ItemDTO>();
                cfg.CreateMap<OrderItem,OrderItemDTO>();

            });
            Mappers = config.CreateMapper();
        }
    }
}
