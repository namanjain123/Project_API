using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using Buisness_Layer.Repository;
using Database_Layer;
using Database_Layer.DTOs;
using Database_Layer.Model;

namespace API_Testing.Buisness_Layer
{


    [TestFixture]
    public class CustomerTest
    {
        private Mock<DatabaseContext> _mockDatabase;
        private Mock<IMapper> _mockMapper;
        private Customers _customers;

        [SetUp]
        public void Setup()
        {
            _mockDatabase = new Mock<DatabaseContext>();
            _mockMapper = new Mock<IMapper>();
            _customers = new Customers(_mockDatabase.Object, _mockMapper.Object);
        }
        [Test]
        public async Task AddCustomer_NewCustomer()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Email = "test@example.com",
                Name="Test",
                Id=1,
                Mobile_no="00000000000"
            };

            _mockDatabase.Setup(c => c.Customers.Any(It.IsAny<Func<Database_Layer.Model.Customer, bool>>()))
                        .Returns(false);

            // Act
            var result = await _customers.AddCustomer(customer);

            // Assert
            Assert.AreEqual("Added", result);
            _mockDatabase.Verify(c => c.Customers.AddAsync(It.IsAny<Customer>(), default), Times.Never);

        }
        [Test]
        public async Task AddCustomer_CustomerWithEmailExists()
        {
            // Arrange
            var customerData = new CustomerDTO
            {
                Email = "test@example.com",
                Name = "Test",
                Id = 1,
                Mobile_no = "00000000000"
            };

            _mockDatabase.Setup(c => c.Customers.Any(It.IsAny<Func<Customer, bool>>()))
                        .Returns(true);

            // Act
            var result = await _customers.AddCustomer(customerData);

            // Assert
            Assert.AreEqual("Already Exsist", result);
            _mockDatabase.Verify(c => c.Customers.AddAsync(It.IsAny<Customer>(), default), Times.Never);

        }
        [Test]
        public async Task DeleteCustomer_ValidId_DeletesCustomer()
        {
            // Arrange
            int customerId = 1;
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerId))
                        .ReturnsAsync(new Customer { Id = customerId });

            // Act
            var result = await _customers.DeleteCustomer(customerId.ToString());

            // Assert
            Assert.AreEqual("Deleted", result);
            _mockDatabase.Verify(c => c.Customers.Remove(It.IsAny<Customer>()), Times.Once);
            
        }

        [Test]
        public async Task DeleteCustomer_InvalidIdFormat()
        {
            // Act
            var result = await _customers.DeleteCustomer("invalid");

            // Assert
            Assert.AreEqual("Invalid customer ID format", result);
            _mockDatabase.Verify(c => c.Customers.FindAsync(It.IsAny<int>()), Times.Never);
            _mockDatabase.Verify(c => c.Customers.Remove(It.IsAny<Customer>()), Times.Never);
           
        }

        [Test]
        public async Task DeleteCustomer_CustomerNotFound()
        {
            // Arrange
            int customerId = 1;
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerId))
                        .ReturnsAsync((Customer)null);

            // Act
            var result = await _customers.DeleteCustomer(customerId.ToString());

            // Assert
            Assert.AreEqual("Customer not found", result);
            _mockDatabase.Verify(c => c.Customers.Remove(It.IsAny<Customer>()), Times.Never);
           
        }

        [Test]
        public async Task GetCustomer()
        {
            // Arrange
            var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Customer 1" },
            new Customer { Id = 2, Name = "Customer 2" }
        };
            
            var customerDtos = customers.Select(c => new CustomerDTO { Id = c.Id, Name = c.Name });
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(customers)).Returns(customerDtos.ToList());

            // Act
            var result = await _customers.GetCustomer();

            // Assert
            CollectionAssert.AreEqual(customerDtos.ToList(), result);
        }

        [Test]
        public async Task GetCustomerID_ValidId()
        {
            // Arrange
            int customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Customer 1" };
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerId)).ReturnsAsync(customer);
            var customerDto = new CustomerDTO { Id = customer.Id, Name = customer.Name };
            _mockMapper.Setup(m => m.Map<CustomerDTO>(customer)).Returns(customerDto);

            // Act
            var result = await _customers.GetCustomerID(customerId.ToString());

            // Assert
            Assert.AreEqual(customerDto, result);
        }

        [Test]
        public async Task GetCustomerID_InvalidIdFormat()
        {
            // Act
            var result = await _customers.GetCustomerID("invalid");

            // Assert
            Assert.IsNull(result);
            _mockDatabase.Verify(c => c.Customers.FindAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task UpdateCustomer_ValidCustomer()
        {
            // Arrange
            var customerDto = new CustomerDTO { Id = 1, Name = "Updated Customer", Email = "updated@example.com" };
            var existingCustomer = new Customer { Id = 1, Name = "Old Customer", Email = "old@example.com" };
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerDto.Id)).ReturnsAsync(existingCustomer);
            _mockDatabase.Setup(c => c.Customers.Any(c => c.Id != customerDto.Id && c.Email == customerDto.Email)).Returns(false);

            // Act
            var result = await _customers.UpdateCustomer(customerDto);

            // Assert
            Assert.AreEqual("Updated", result);
            _mockMapper.Verify(m => m.Map(customerDto, existingCustomer), Times.Once);
            
        }

        [Test]
        public async Task UpdateCustomer_CustomerNotFound()
        {
            // Arrange
            var customerDto = new CustomerDTO { Id = 1, Name = "Updated Customer", Email = "updated@example.com" };
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerDto.Id)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customers.UpdateCustomer(customerDto);

            // Assert
            Assert.AreEqual("Customer not found", result);
            _mockMapper.Verify(m => m.Map(It.IsAny<CustomerDTO>(), It.IsAny<Customer>()), Times.Never);
            
        }

        [Test]
        public async Task UpdateCustomer_DuplicateEmail()
        {
            // Arrange
            var customerDto = new CustomerDTO { Id = 1, Name = "Updated Customer", Email = "duplicate@example.com" };
            var existingCustomer = new Customer { Id = 2, Name = "Another Customer", Email = "duplicate@example.com" };
            _mockDatabase.Setup(c => c.Customers.FindAsync(customerDto.Id)).ReturnsAsync(existingCustomer);
            _mockDatabase.Setup(c => c.Customers.Any(c => c.Id != customerDto.Id && c.Email == customerDto.Email)).Returns(true);

            // Act
            var result = await _customers.UpdateCustomer(customerDto);

            // Assert
            Assert.AreEqual("Update Done", result);
            _mockMapper.Verify(m => m.Map(It.IsAny<CustomerDTO>(), It.IsAny<Customer>()), Times.Never);
        }

    }
}
