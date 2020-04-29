using Amazon.DynamoDBv2.Model;
using Dashboard.Types;
using System;
using System.Collections.Generic;

namespace Dashboard.Helpers
{
    public class DynamoDbHelper
    {
        public Customer MapToCustomer(Dictionary<string, AttributeValue> record)
        {
            return new Customer
            {
                Username = record["username"].S,
                Name = record["name"].S,
                Date = record["date"].S
            };
        }

        public Order MapToOrder(Dictionary<string, AttributeValue> record)
        {
            return new Order
            {
                Username = record["username"].S,
                Item = record["item"].S,
                Quantity = Convert.ToInt32(record["quantity"].N),
                Date = record["date"].S
            };
        }

        public Claim MapToClaim(Dictionary<string, AttributeValue> record)
        {
            return new Claim
            {
                Username = record["username"].S,
                Item = record["item"].S,
                Summary = record["summary"].S,
                Date = record["date"].S
            };
        }
    }
}
