using AutoMapper;
using Buisness_Layer.Repository.Interface;
using Database_Layer;
using Database_Layer.DTOs;
using Database_Layer.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Layer.Repository
{
    public class Orders : IOrders
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public Orders(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<OrderDTO>> GetOrders(int pageIndex, int pageSize)
        {

            var orders = _context.Database.SqlQueryRaw<Order>("EXEC GetOrdersByPage @PageIndex, @PageSize",
                                                         new SqlParameter("PageIndex", pageIndex),
                                                         new SqlParameter("PageSize", pageSize))
                                 .ToList();

            var orderViewModels = _mapper.Map<List<OrderDTO>>(orders);

            return orderViewModels;
        }

        public async Task<string> AddOrder(OrderDTO orders)
        {
            string result;
            if (_context.Orders.Any(o => o.Id == orders.Id))
            {
                result = "Already Exsist";
            }
            else
            {
                var order = _mapper.Map<Order>(orders);

                
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                result = "Added";

            }

            return result;
        }

        public async Task<string> DeleteOrder(string id)
        {
            if (int.TryParse(id, out int orderId))
            {
                var order = await _context.Orders.FindAsync(orderId);

                if (order != null)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    return "Deleted";
                }
                else
                {
                    return "Customer not found";
                }
            }
            else
            {
                return "Invalid customer ID format";
            }
        }

        
        

        public async Task<string> UpdateOrder(OrderDTO orders)
        {
            var existingOrders = await _context.Orders.FindAsync(orders.Id);

            if (existingOrders == null)
            {
                return "Order not found";
            }

            
            _mapper.Map(orders, existingOrders);

            
            await _context.SaveChangesAsync();

            return "Updated";
        }
    }
}
