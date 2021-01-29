using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace core.Models
{
    public class Product : TableEntity
    {
        public Product(string productCategory, string productName)
        {
            PartitionKey = productCategory;
            RowKey = productName;
            
        }

        public Product() { }

        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
