using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using Ms.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ms.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomerService customerService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomerService customerService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customerService = customerService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customerResult = await customerService.GetCustomerAsync(customerId);

            if (ordersResult.IsSuccess)
            {
                foreach (var orders in ordersResult.Orders)
                {
                    foreach (var item in orders.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ?
                            productsResult.Products.FirstOrDefault(v => v.Id == item.ProductId)?.Name :
                            "Products service is not available";
                    }
                }
                var result = new
                {
                    Customer = customerResult.IsSuccess ?
                        customerResult.Customer :
                        new Models.Customer { Name = "Customer service is not avilable" },
                    ordersResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
