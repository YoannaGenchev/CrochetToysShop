using CrochetToysShop.Common;
using CrochetToysShop.Data.Models;

namespace CrochetToysShop.Services.Tests.Infrastructure.Builders
{
    internal class OrderBuilder
    {
        private readonly Order order = new()
        {
            CustomerName = "Test Customer",
            PhoneNumber = "123456",
            Address = "Test Address 1",
            Status = OrderStatus.New,
            ToyId = 1,
            CreatedOn = DateTime.UtcNow
        };

        public OrderBuilder WithId(int id)
        {
            order.Id = id;
            return this;
        }

        public OrderBuilder WithCustomerName(string customerName)
        {
            order.CustomerName = customerName;
            return this;
        }

        public OrderBuilder WithToyId(int toyId)
        {
            order.ToyId = toyId;
            return this;
        }

        public OrderBuilder WithStatus(string status)
        {
            order.Status = status;
            return this;
        }

        public OrderBuilder WithCreatedOn(DateTime createdOn)
        {
            order.CreatedOn = createdOn;
            return this;
        }

        public OrderBuilder WithPhoneNumber(string phoneNumber)
        {
            order.PhoneNumber = phoneNumber;
            return this;
        }

        public OrderBuilder WithAddress(string address)
        {
            order.Address = address;
            return this;
        }

        public Order Build() => order;
    }
}
