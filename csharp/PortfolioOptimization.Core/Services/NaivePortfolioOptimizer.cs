using PortfolioOptimization.Core.Models;

namespace PortfolioOptimization.Core.Services;

/// <summary>
/// Placeholder optimizer that returns a naive equal-weight solution.
/// Will be replaced with Python-based optimization via subprocess.
/// </summary>
public class NaivePortfolioOptimizer : IPortfolioOptimizer
{
    public async Task<OptimizedPortfolio> OptimizeAsync(
        List<Asset> assets,
        PortfolioConstraints constraints,
        double[,]? correlationMatrix = null,
        double riskFreeRate = 0.02)
    {
        return await Task.Run(() =>
        {
            if (assets.Count == 0)
                throw new ArgumentException("No assets provided for optimization.");

            // Naive: equal weight
            var equalWeight = 1.0 / assets.Count;
            var weights = assets.ToDictionary(a => a.Id, _ => equalWeight);

            // Calculate portfolio metrics
            var expectedReturn = assets.Average(a => a.ExpectedReturn);
            var expectedVolatility = assets.Average(a => a.Volatility);
            var sharpeRatio = (expectedReturn - riskFreeRate) / (expectedVolatility > 0 ? expectedVolatility : 1.0);

            return new OptimizedPortfolio
            {
                Id = Guid.NewGuid().ToString(),
                Weights = weights,
                ExpectedReturn = expectedReturn,
                ExpectedVolatility = expectedVolatility,
                SharpeRatio = sharpeRatio,
                Status = "Success",
                Warnings = new List<string> { "Using naive equal-weight allocation. Run Python optimizer for production results." }
            };
        });
    }
}
