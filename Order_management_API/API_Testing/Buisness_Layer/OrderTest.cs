using AutoMapper;
using Buisness_Layer.Repository;
using Database_Layer.DTOs;
using Database_Layer;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buisness_Layer.Repository.Interface;
using Database_Layer.Model;

namespace API_Testing.Buisness_Layer
{
    [TestFixture]
    public class OrderTest
    {
        private Orders _orders;
        private Mock<DatabaseContext> _mockDatabase;
        private Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            // Mock DatabaseContext and IMapper
            _mockDatabase = new Mock<DatabaseContext>();
            _mockMapper = new Mock<IMapper>();

            // Initialize the Orders class with the mock objects
            _orders = new Orders(_mockDatabase.Object, _mockMapper.Object);
        }


        [Test]
        public async Task GetOrders_ValidParameters_ReturnsList()
        {
            // Arrange
            int pageIndex = 1;
            int pageSize = 10;
            var ordersFromDatabase = new List<Order>
        {
            new Order { Id = 1, OrderNumber = "001", OrderDate = DateTime.Now, CustomerId = 101 },
            new Order { Id = 2, OrderNumber = "002", OrderDate = DateTime.Now, CustomerId = 102 }
        };
            var expectedOrderDTOs = new List<OrderDTO>
        {
            new OrderDTO { Id = 1, OrderNumber = "001", OrderDate = DateTime.Now, CustomerId = 101 },
            new OrderDTO { Id = 2, OrderNumber = "002", OrderDate = DateTime.Now, CustomerId = 102 }
        };

            _mockDatabase.Setup(c => c.Database.SqlQueryRaw<Order>("EXEC GetOrdersByPage @PageIndex, @PageSize",
                            "1", "2"))
                        .Returns((IQueryable<Order>)ordersFromDatabase);

            _mockMapper.Setup(m => m.Map<List<OrderDTO>>(ordersFromDatabase))
                       .Returns(expectedOrderDTOs);

            // Act
            var result = await _orders.GetOrders(pageIndex, pageSize);

            // Assert
            Assert.AreEqual(expectedOrderDTOs.Count, result.Count);
            for (int i = 0; i < expectedOrderDTOs.Count; i++)
            {
                Assert.AreEqual(expectedOrderDTOs[i].Id, result[i].Id);
                Assert.AreEqual(expectedOrderDTOs[i].OrderNumber, result[i].OrderNumber);
                Assert.AreEqual(expectedOrderDTOs[i].OrderDate, result[i].OrderDate);
                Assert.AreEqual(expectedOrderDTOs[i].CustomerId, result[i].CustomerId);
            }
        }

        [Test]
        public async Task AddOrder_NewOrder()
        {
            // Arrange
            var orderDto = new OrderDTO { Id = 1, OrderNumber = "ORD001", OrderDate = DateTime.Now, CustomerId = 101 };
            var orderEntity = new Order { Id = 1, OrderNumber = "ORD001", OrderDate = DateTime.Now, CustomerId = 101 };

            _mockDatabase.Setup(c => c.Orders.Any(o => o.Id == orderDto.Id)).Returns(false);

            _mockMapper.Setup(m => m.Map<Order>(orderDto)).Returns(orderEntity);

            // Act
            var result = await _orders.AddOrder(orderDto);

            // Assert
            Assert.AreEqual("Added", result);
            _mockDatabase.Verify(c => c.Orders.AddAsync(It.IsAny<Order>(),default), Times.Once);
            

        }

        [Test]
        public async Task AddOrder_ExistingOrder()
        {
            // Arrange
            var orderDto = new OrderDTO { Id = 1, OrderNumber = "ORD001", OrderDate = DateTime.Now, CustomerId = 101 };

            _mockDatabase.Setup(c => c.Orders.Any(o => o.Id == orderDto.Id)).Returns(true);

            // Act
            var result = await _orders.AddOrder(orderDto);

            // Assert
            Assert.AreEqual("Already Exsist", result);
            _mockDatabase.Verify(c => c.Orders.AddAsync(It.IsAny<Order>(), default), Times.Never);
            
        }

        [Test]
        public async Task DeleteOrder_ExistingOrderId()
        {
            // Arrange
            int orderId = 1;
            var orderEntity = new Order { Id = orderId, OrderNumber = "ORD001", OrderDate = DateTime.Now, CustomerId = 101 };

            _mockDatabase.Setup(c => c.Orders.FindAsync(orderId)).ReturnsAsync(orderEntity);

            // Act
            var result = await _orders.DeleteOrder(orderId.ToString());

            // Assert
            Assert.AreEqual("Deleted", result);
            _mockDatabase.Verify(c => c.Orders.Remove(orderEntity), Times.Once);
            
        }

        [Test]
        public async Task DeleteOrder_NonExistingOrderId()
        {
            // Arrange
            int orderId = 1;

            _mockDatabase.Setup(c => c.Orders.FindAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _orders.DeleteOrder(orderId.ToString());

            // Assert
            Assert.AreEqual("Customer not found", result);
            _mockDatabase.Verify(c => c.Orders.Remove(It.IsAny<Order>()), Times.Never);

        }

        [Test]
        public async Task DeleteOrder_InvalidOrderIdFormat()
        {
            // Arrange
            string orderId = "abc";

            // Act
            var result = await _orders.DeleteOrder(orderId);

            // Assert
            Assert.AreEqual("Invalid customer ID format", result);
            _mockDatabase.Verify(c => c.Orders.Remove(It.IsAny<Order>()), Times.Never);
            
        }

        [Test]
        public async Task UpdateOrder_ExistingOrder()
        {
            // Arrange
            var orderDto = new OrderDTO { Id = 1, OrderNumber = "001", OrderDate = DateTime.Now, CustomerId = 101 };
            var existingOrderEntity = new Order { Id = 1, OrderNumber = "001", OrderDate = DateTime.Now, CustomerId = 101 };

            _mockDatabase.Setup(c => c.Orders.FindAsync(orderDto.Id)).ReturnsAsync(existingOrderEntity);

            // Act
            var result = await _orders.UpdateOrder(orderDto);

            // Assert
            Assert.AreEqual("Updated", result);
            _mockMapper.Verify(m => m.Map(orderDto, existingOrderEntity), Times.Once);
            
        }

        [Test]
        public async Task UpdateOrder_NonExistingOrder()
        {
            // Arrange
            var orderDto = new OrderDTO { Id = 1, OrderNumber = "001", OrderDate = DateTime.Now, CustomerId = 101 };

            _mockDatabase.Setup(c => c.Orders.FindAsync(orderDto.Id)).ReturnsAsync((Order)null);

            // Act
            var result = await _orders.UpdateOrder(orderDto);

            // Assert
            Assert.AreEqual("Order not found", result);
            _mockMapper.Verify(m => m.Map(orderDto, It.IsAny<Order>()), Times.Never);
            
        }



    }
}
