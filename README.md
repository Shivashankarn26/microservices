# Microservices
Implementation of microservices using .net core

## project structure
There are 4 API projects 
- Customers
- Orders
- Products
- Search

The Search project uses other 3 APIs as microservices. All APIs are stand alone and it can be deployed seperately.

This is a example of synchronous communication between microservices. 
Search API gets information from other APIs and serves to requests.

If any API is unavailable then serch API tries to make connection 5 times before it sends error. This is implemented using the *Microsoft.Extenstion.Http.Polly* nuget package.

example for customer service configuaration in ./Ms.Api.Search/Startup.cs
```
services.AddHttpClient("CustomersService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Services:Customers"]);
            }).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500)));
```
