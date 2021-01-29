using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace core.Models
{
    public class Review : TableEntity
    {
        public Review(string productName, string invertedTimeKey)
        {
            PartitionKey = productName;
            RowKey = invertedTimeKey;
        }

        public Review() { }

        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
