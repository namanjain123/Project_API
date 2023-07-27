using Buisness_Layer.Repository.Interface;
using Database_Layer.DTOs;
using Database_Layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Order_management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomers _customer;
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;

        public CustomerController(ICustomers customer, ILogger logger, IDistributedCache cache)
        {
            _customer = customer;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<List<CustomerDTO>> GetCustomer()
        {
            try
            {
                
                string cacheKey = "AllCustomers";
                string cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData != null)
                {
                    
                    return JsonSerializer.Deserialize<List<CustomerDTO>>(cachedData);
                }

                
                List<CustomerDTO> result = await _customer.GetCustomer();

                
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(40)
                };
                string serializedData = JsonSerializer.Serialize(result);
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

                _logger.LogInformation("All Customer Request");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all customers.");
                return new List<CustomerDTO>();
            }
        }

        [HttpGet]
        public async Task<CustomerDTO> GetCustomerID(string id)
        {
            try
            {
                
                string cacheKey = $"Customer_{id}";
                string cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData != null)
                {
                    
                    return JsonSerializer.Deserialize<CustomerDTO>(cachedData);
                }

                
                CustomerDTO result = await _customer.GetCustomerID(id);

                
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(40)
                };
                string serializedData = JsonSerializer.Serialize(result);
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

                _logger.LogInformation($"Get of custom id={id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching customer with ID={id}.");

                
                return new CustomerDTO();
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDTO customers)
        {
            try
            {
                string result = await _customer.AddCustomer(customers)??"";
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return BadRequest("Failed to add customer");
            }
        }
       
        [HttpPatch]
        public async Task<IActionResult> UpdateCustomer(CustomerDTO customer)
        {
            try
            {
                string result = await _customer.UpdateCustomer(customer);
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return BadRequest("Failed to Update customer");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            try
            {
                string result = await _customer.DeleteCustomer(id);
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return BadRequest("Failed to Delete customer");
            }
        }
    }
}