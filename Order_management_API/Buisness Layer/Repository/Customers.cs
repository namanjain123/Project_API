using AutoMapper;
using Buisness_Layer.Repository.Interface;
using Database_Layer;
using Database_Layer.AutoMapper;
using Database_Layer.DTOs;
using Database_Layer.Model;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Layer.Repository
{
    public class Customers : ICustomers
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        public Customers(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> AddCustomer(CustomerDTO customerDto)
        {
            string result;
            if (_context.Customers.Any(o => o.Email == customerDto.Email))
            {
                result="Already Exsist";
            }
            else {
                var customer = _mapper.Map<Customer>(customerDto);

                
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                result = "Added";
                
            }

            return result;
        }

        public async Task<string> DeleteCustomer(string id)
        {
            if (int.TryParse(id, out int customerId))
            {
                var customer = await _context.Customers.FindAsync(customerId);

                if (customer != null)
                {
                    _context.Customers.Remove(customer);
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

        public async Task<List<CustomerDTO>> GetCustomer()
        {
            var customers = await _context.Customers.ToListAsync();

            
            var customerDtos = _mapper.Map<List<CustomerDTO>>(customers);

            return customerDtos;
        }

        public async Task<CustomerDTO> GetCustomerID(string id)
        {
            int customerId = int.Parse(id);
            var customer = await _context.Customers.FindAsync(customerId);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<string> UpdateCustomer(CustomerDTO customerDto)
        {
            var existingCustomer = await _context.Customers.FindAsync(customerDto.Id);

            if (existingCustomer == null)
            {
                return "Customer not found";
            }

            
            if (_context.Customers.Any(c => c.Id != customerDto.Id && c.Email == customerDto.Email))
            {
                return "Another customer with this email already exists";
            }

            
            _mapper.Map(customerDto, existingCustomer);

            
            await _context.SaveChangesAsync();

            return "Updated";
        }
    }
}
