using core.Services;

namespace core.Interfaces
{
    public interface IReviewServiceFactory
    {
        ReviewService GetReviewService();
    }
}
