using Buisness_Layer.Repository.Interface;
using Database_Layer.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Order_management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrders _orders;
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public OrdersController(IOrders orders, IDistributedCache cache, ILogger logger)
        {
            _orders = orders;
            _cache = cache;
            _logger = logger;
        }
        [HttpGet]
        public async Task<List<OrderDTO>> GetOrders(int pageIndex, int pageSize)
        {
            try
            {
                
                string cacheKey = $"Orders_Page_{pageIndex}_Size_{pageSize}";
                string cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData != null)
                {
                    
                    return JsonSerializer.Deserialize<List<OrderDTO>>(cachedData);
                }

                
                List<OrderDTO> result = await _orders.GetOrders(pageIndex, pageSize);

                
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(40)
                };
                string serializedData = JsonSerializer.Serialize(result);
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching orders for PageIndex={pageIndex} and PageSize={pageSize}.");

                
                return new List<OrderDTO>();
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDTO order)
        {
            try
            {
                string result = await _orders.AddOrder(order);
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an order.");
                return BadRequest("Failed to add order");
            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateOrder(OrderDTO order)
        {
            try
            {
                string result = await _orders.UpdateOrder(order);
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating an order.");
                return BadRequest("Failed to update order");
            }

        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOrder(string id)
        {
            try
            {
                string result = await _orders.DeleteOrder(id);
                _logger.LogInformation(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting an order.");
                return BadRequest("Failed to delete order");
            }
        }
    }
}
