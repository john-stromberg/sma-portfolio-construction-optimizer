namespace PortfolioOptimization.Core.Models;

/// <summary>
/// Represents a single asset in the portfolio.
/// </summary>
public class Asset
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string AssetClass { get; set; }

    /// <summary>
    /// Expected return (annual).
    /// </summary>
    public required double ExpectedReturn { get; set; }

    /// <summary>
    /// Standard deviation of returns (annual).
    /// </summary>
    public required double Volatility { get; set; }

    /// <summary>
    /// Current market price.
    /// </summary>
    public required double CurrentPrice { get; set; }
}
