using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Dashboard.Helpers;
using Dashboard.Models;
using Dashboard.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAmazonDynamoDB _dynamoDB;
        private readonly DynamoDbHelper _dynamoHelper;

        public HomeController(ILogger<HomeController> logger, IAmazonDynamoDB dynamoDB, DynamoDbHelper dynamoHelper)
        {
            _logger = logger;
            _dynamoDB = dynamoDB;
            _dynamoHelper = dynamoHelper;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await GetCustomersAsync();
            var orders = await GetOrdersAsync();
            var claims = await GetClaimsAsync();

            var data = from customer in customers
                       let purchases = orders.Where(o => o.Username == customer.Username)
                       let complains = claims.Where(c => c.Username == customer.Username)
                       select new Summary
                       {
                            Customer = new Customer
                            {
                                Username = customer.Username,
                                Name = customer.Name,
                                Date = customer.Date
                            },
                            Orders = purchases.ToList(),
                            Claims = complains.ToList()
                       };

            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<Customer>> GetCustomersAsync()
        {
            // Scan request for customers
            var customersFilter = new ScanRequest
            {
                TableName = "customers",
                ProjectionExpression = "username, #formattedDate, #fullName",
                ExpressionAttributeNames = new Dictionary<string, string>
                {   // Replaces reserved keywords
                    { "#formattedDate", "date" },
                    { "#fullName", "name" }
                }
            };

            // Get customers
            var customersDocument = await _dynamoDB.ScanAsync(customersFilter);
            var customers = customersDocument.Items.Select(_dynamoHelper.MapToCustomer).ToList();

            return customers;
        }

        private async Task<List<Order>> GetOrdersAsync()
        {
            // Scan request for customers
            var ordersFilter = new ScanRequest
            {
                TableName = "orders",
                ProjectionExpression = "username, #formattedDate, #productItem, quantity",
                ExpressionAttributeNames = new Dictionary<string, string>
                {   // Replaces reserved keywords
                    { "#formattedDate", "date" },
                    { "#productItem", "item" }
                }
            };

            // Get customers
            var document = await _dynamoDB.ScanAsync(ordersFilter);
            var orders = document.Items.Select(_dynamoHelper.MapToOrder).ToList();

            return orders;
        }

        private async Task<List<Claim>> GetClaimsAsync()
        {
            // Scan request for customers
            var claimsFilter = new ScanRequest
            {
                TableName = "claims",
                ProjectionExpression = "username, #formattedDate, #productItem, summary",
                ExpressionAttributeNames = new Dictionary<string, string>
                {   // Replaces reserved keywords
                    { "#formattedDate", "date" },
                    { "#productItem", "item" }
                }
            };

            // Get customers
            var document = await _dynamoDB.ScanAsync(claimsFilter);
            var claims = document.Items.Select(_dynamoHelper.MapToClaim).ToList();

            return claims;
        }
    }
}
