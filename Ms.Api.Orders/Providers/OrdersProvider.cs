using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Ms.Api.Orders.Db;
using Ms.Api.Orders.Interfaces;
using Ms.Api.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ms.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = DateTime.Today,
                    Total = 100,
                    Items = new List<Db.OrderItem>()
                    {
                        new Db.OrderItem(){ Id=1, OrderId =1 , ProductId=1, Quantity=10, UnitPrice= 10},
                        new Db.OrderItem(){ Id=2, OrderId =1 , ProductId=2, Quantity=20, UnitPrice= 15},
                        new Db.OrderItem(){ Id=3, OrderId =1 , ProductId=3, Quantity=30, UnitPrice= 8},
                        new Db.OrderItem(){ Id=4, OrderId =1 , ProductId=4, Quantity=40, UnitPrice= 5}
                    }
                });
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = DateTime.Today,
                    Total = 100,
                    Items = new List<Db.OrderItem>()
                    {
                        new Db.OrderItem(){ Id=5, OrderId =2 , ProductId=1, Quantity=10, UnitPrice= 10},
                        new Db.OrderItem(){ Id=6, OrderId =2 , ProductId=2, Quantity=20, UnitPrice= 15},
                        new Db.OrderItem(){ Id=7, OrderId =2 , ProductId=3, Quantity=30, UnitPrice= 8},
                        new Db.OrderItem(){ Id=8, OrderId =2 , ProductId=4, Quantity=40, UnitPrice= 5},
                        new Db.OrderItem(){ Id=9, OrderId =2 , ProductId=5, Quantity=30, UnitPrice= 8},
                        new Db.OrderItem(){ Id=10, OrderId =2 , ProductId=6, Quantity=40, UnitPrice= 5}
                    }
                });
                dbContext.SaveChanges();
            }
        }
        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders.Include(v => v.Items).Where(v => v.CustomerId == customerId).ToListAsync();

                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
