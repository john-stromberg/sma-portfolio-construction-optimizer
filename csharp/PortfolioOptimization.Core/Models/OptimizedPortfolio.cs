namespace PortfolioOptimization.Core.Models;

/// <summary>
/// Represents an optimized portfolio solution.
/// </summary>
public class OptimizedPortfolio
{
    /// <summary>
    /// Unique identifier for this optimization run.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Timestamp of the optimization.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Asset weights in the optimized portfolio (asset id -> weight).
    /// </summary>
    public required Dictionary<string, double> Weights { get; set; }

    /// <summary>
    /// Expected portfolio return.
    /// </summary>
    public required double ExpectedReturn { get; set; }

    /// <summary>
    /// Expected portfolio volatility (risk).
    /// </summary>
    public required double ExpectedVolatility { get; set; }

    /// <summary>
    /// Sharpe ratio (return per unit of risk).
    /// </summary>
    public required double SharpeRatio { get; set; }

    /// <summary>
    /// Optimization status.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Constraint violations or warnings, if any.
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}
