using core.Services;

namespace core.Interfaces
{
    public interface IProductServiceFactory
    {
        ProductService GetProductService();
    }
}
