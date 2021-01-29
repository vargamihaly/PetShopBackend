namespace core.DTO
{
    public class ProductDto
    {
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
    }
}