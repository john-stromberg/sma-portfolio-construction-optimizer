namespace PortfolioOptimization.Core.Models;

/// <summary>
/// Represents portfolio constraints for optimization.
/// </summary>
public class PortfolioConstraints
{
    /// <summary>
    /// Minimum weight for each asset (0.0 - 1.0).
    /// </summary>
    public Dictionary<string, double> MinWeights { get; set; } = new();

    /// <summary>
    /// Maximum weight for each asset (0.0 - 1.0).
    /// </summary>
    public Dictionary<string, double> MaxWeights { get; set; } = new();

    /// <summary>
    /// Minimum target return for the portfolio.
    /// </summary>
    public double? MinimumReturn { get; set; }

    /// <summary>
    /// Maximum target volatility (standard deviation) for the portfolio.
    /// </summary>
    public double? MaximumVolatility { get; set; }

    /// <summary>
    /// Budget constraint (sum of weights = 1.0 by default).
    /// </summary>
    public double BudgetConstraint { get; set; } = 1.0;
}
