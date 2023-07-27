using Database_Layer.DTOs;
using Database_Layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Layer.Repository.Interface
{
    public interface IOrders
    {
        public Task<List<OrderDTO>> GetOrders(int pageIndex, int pageSize);
        public Task<string> UpdateOrder(OrderDTO orders);
        public Task<string> DeleteOrder(string id);
        public Task<string> AddOrder(OrderDTO orders);
     
    }
}
