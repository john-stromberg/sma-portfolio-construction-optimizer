using PortfolioOptimization.Core.Models;

namespace PortfolioOptimization.Core.Services;

/// <summary>
/// Interface for portfolio optimization engine.
/// Implementations can use Python (via subprocess) or direct C# algorithms.
/// </summary>
public interface IPortfolioOptimizer
{
    /// <summary>
    /// Optimize a portfolio given assets and constraints.
    /// </summary>
    /// <param name="assets">List of candidate assets.</param>
    /// <param name="constraints">Portfolio constraints.</param>
    /// <param name="correlationMatrix">Optional correlation matrix between assets.</param>
    /// <param name="riskFreeRate">Risk-free rate for Sharpe ratio calculation.</param>
    /// <returns>Optimized portfolio solution.</returns>
    Task<OptimizedPortfolio> OptimizeAsync(
        List<Asset> assets,
        PortfolioConstraints constraints,
        double[,]? correlationMatrix = null,
        double riskFreeRate = 0.02);
}
