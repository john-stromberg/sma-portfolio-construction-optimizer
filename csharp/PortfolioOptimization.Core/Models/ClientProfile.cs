namespace PortfolioOptimization.Core.Models;

/// <summary>
/// Represents a client profile for onboarding.
/// </summary>
public class ClientProfile
{
    /// <summary>
    /// Unique identifier for this client profile.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Client name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Annual income in dollars.
    /// </summary>
    public required double AnnualIncome { get; set; }

    /// <summary>
    /// Risk tolerance level.
    /// </summary>
    public required RiskTolerance RiskTolerance { get; set; }

    /// <summary>
    /// Initial investment amount (budget) in dollars.
    /// </summary>
    public required double InvestmentAmount { get; set; }

    /// <summary>
    /// Investment time horizon in years.
    /// </summary>
    public required int TimeHorizonYears { get; set; }

    /// <summary>
    /// Target annual return (as decimal, e.g., 0.07 for 7%).
    /// </summary>
    public double? TargetReturnRate { get; set; }

    /// <summary>
    /// Maximum acceptable volatility/risk (as decimal, e.g., 0.12 for 12%).
    /// </summary>
    public double? MaxVolatilityTolerance { get; set; }

    /// <summary>
    /// Profile creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enumeration of risk tolerance levels.
/// </summary>
public enum RiskTolerance
{
    /// <summary>
    /// Conservative investor - prioritizes capital preservation.
    /// </summary>
    Conservative = 0,

    /// <summary>
    /// Moderate investor - balanced approach.
    /// </summary>
    Moderate = 1,

    /// <summary>
    /// Aggressive investor - prioritizes growth.
    /// </summary>
    Aggressive = 2,
}
