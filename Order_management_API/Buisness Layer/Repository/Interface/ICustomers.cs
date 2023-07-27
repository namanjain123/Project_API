using Database_Layer.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Layer.Repository.Interface
{
    public interface ICustomers
    {
        public Task<List<CustomerDTO>> GetCustomer();
        public Task<CustomerDTO> GetCustomerID(string id);
        public Task<string> AddCustomer(CustomerDTO customer);
        public Task<string> UpdateCustomer(CustomerDTO customer);
        public Task<string> DeleteCustomer(string id);
    }
}
