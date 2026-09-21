namespace PortfolioOptimization.Core.Models;

/// <summary>
/// Represents a complete client onboarding example with profile, assets, constraints, and optimized portfolio.
/// </summary>
public class ClientOnboardingExample
{
    /// <summary>
    /// The client profile.
    /// </summary>
    public required ClientProfile ClientProfile { get; set; }

    /// <summary>
    /// Available assets for the portfolio.
    /// </summary>
    public required List<Asset> AvailableAssets { get; set; }

    /// <summary>
    /// Portfolio constraints derived from client profile.
    /// </summary>
    public required PortfolioConstraints ConstraintsDerived { get; set; }

    /// <summary>
    /// Correlation matrix for the assets (optional).
    /// </summary>
    public double[,]? CorrelationMatrix { get; set; }

    /// <summary>
    /// Risk-free rate used for optimization.
    /// </summary>
    public double RiskFreeRate { get; set; } = 0.02;

    /// <summary>
    /// The optimized portfolio result.
    /// </summary>
    public OptimizedPortfolio? OptimizedPortfolio { get; set; }

    /// <summary>
    /// Timestamp when this example was generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
