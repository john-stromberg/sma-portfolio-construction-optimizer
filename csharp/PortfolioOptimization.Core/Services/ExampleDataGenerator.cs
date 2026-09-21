using PortfolioOptimization.Core.Models;

namespace PortfolioOptimization.Core.Services;

/// <summary>
/// Service for generating random client onboarding examples.
/// </summary>
public class ExampleDataGenerator
{
    private static readonly Random _random = new();
    private readonly List<Asset> _sampleAssets;
    private readonly double[,] _correlationMatrix;

    public ExampleDataGenerator()
    {
        _sampleAssets = InitializeSampleAssets();
        _correlationMatrix = InitializeCorrelationMatrix();
    }

    /// <summary>
    /// Generates a random client profile with a specified risk tolerance.
    /// </summary>
    public ClientProfile GenerateRandomClientProfile(RiskTolerance? riskTolerance = null)
    {
        var randomRiskTolerance = riskTolerance ?? (RiskTolerance)_random.Next(0, 3);
        var annualIncome = _random.Next(50000, 500000);
        var investmentAmount = _random.Next(10000, Math.Min(500000, annualIncome * 2));
        var timeHorizonYears = _random.Next(5, 40);

        var (targetReturn, maxVolatility) = DeriveReturnsAndVolatilityFromRiskTolerance(randomRiskTolerance);

        return new ClientProfile
        {
            ClientId = $"CLIENT-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Name = GenerateRandomClientName(),
            AnnualIncome = annualIncome,
            RiskTolerance = randomRiskTolerance,
            InvestmentAmount = investmentAmount,
            TimeHorizonYears = timeHorizonYears,
            TargetReturnRate = targetReturn,
            MaxVolatilityTolerance = maxVolatility,
            CreatedAt = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Generates portfolio constraints based on client profile.
    /// </summary>
    public PortfolioConstraints GenerateConstraintsFromProfile(ClientProfile profile)
    {
        var constraints = new PortfolioConstraints
        {
            MinimumReturn = profile.TargetReturnRate ?? DeriveRiskTolerance(profile.RiskTolerance).minReturn,
            MaximumVolatility = profile.MaxVolatilityTolerance ?? DeriveRiskTolerance(profile.RiskTolerance).maxVolatility,
            BudgetConstraint = 1.0,
        };

        // Set min and max weights based on risk tolerance
        var (minWeight, maxWeight) = GetWeightConstraintsByRiskTolerance(profile.RiskTolerance);

        // Apply conservative constraints for bond allocations
        foreach (var asset in _sampleAssets)
        {
            if (asset.AssetClass == "Fixed Income")
            {
                constraints.MinWeights[asset.Id] = minWeight * 2; // Higher minimum for bonds
                constraints.MaxWeights[asset.Id] = maxWeight * 2; // Higher maximum for bonds
            }
            else if (asset.AssetClass == "Equity")
            {
                constraints.MinWeights[asset.Id] = minWeight;
                constraints.MaxWeights[asset.Id] = maxWeight;
            }
            else
            {
                constraints.MinWeights[asset.Id] = minWeight * 0.5; // Lower allocation for alternatives
                constraints.MaxWeights[asset.Id] = maxWeight * 0.5;
            }
        }

        return constraints;
    }

    /// <summary>
    /// Generates a complete client onboarding example (profile + assets + constraints).
    /// </summary>
    public ClientOnboardingExample GenerateClientOnboardingExample(RiskTolerance? riskTolerance = null)
    {
        var profile = GenerateRandomClientProfile(riskTolerance);
        var constraints = GenerateConstraintsFromProfile(profile);

        return new ClientOnboardingExample
        {
            ClientProfile = profile,
            AvailableAssets = new List<Asset>(_sampleAssets),
            ConstraintsDerived = constraints,
            CorrelationMatrix = _correlationMatrix,
            RiskFreeRate = 0.02,
            GeneratedAt = DateTime.UtcNow,
        };
    }

    private List<Asset> InitializeSampleAssets()
    {
        return new List<Asset>
        {
            new Asset
            {
                Id = "VTSAX",
                Name = "Vanguard Total Stock Market ETF",
                AssetClass = "Equity",
                ExpectedReturn = 0.10,
                Volatility = 0.15,
                CurrentPrice = 245.50,
            },
            new Asset
            {
                Id = "VTIAX",
                Name = "Vanguard International Stock ETF",
                AssetClass = "Equity",
                ExpectedReturn = 0.09,
                Volatility = 0.18,
                CurrentPrice = 120.30,
            },
            new Asset
            {
                Id = "BND",
                Name = "Vanguard Total Bond Market ETF",
                AssetClass = "Fixed Income",
                ExpectedReturn = 0.04,
                Volatility = 0.05,
                CurrentPrice = 80.75,
            },
            new Asset
            {
                Id = "VGSLX",
                Name = "Vanguard Real Estate ETF",
                AssetClass = "Alternative",
                ExpectedReturn = 0.07,
                Volatility = 0.20,
                CurrentPrice = 150.20,
            },
            new Asset
            {
                Id = "GLD",
                Name = "SPDR Gold Shares",
                AssetClass = "Alternative",
                ExpectedReturn = 0.05,
                Volatility = 0.14,
                CurrentPrice = 195.80,
            },
        };
    }

    private double[,] InitializeCorrelationMatrix()
    {
        return new double[,]
        {
            { 1.0, 0.85, -0.10, 0.60, 0.05 },      // VTSAX
            { 0.85, 1.0, -0.05, 0.50, 0.10 },      // VTIAX
            { -0.10, -0.05, 1.0, 0.20, 0.30 },     // BND
            { 0.60, 0.50, 0.20, 1.0, 0.15 },       // VGSLX
            { 0.05, 0.10, 0.30, 0.15, 1.0 },       // GLD
        };
    }

    private (double targetReturn, double maxVolatility) DeriveReturnsAndVolatilityFromRiskTolerance(RiskTolerance riskTolerance)
    {
        return riskTolerance switch
        {
            RiskTolerance.Conservative => (0.05, 0.08),   // 5% return, 8% volatility
            RiskTolerance.Moderate => (0.07, 0.12),       // 7% return, 12% volatility
            RiskTolerance.Aggressive => (0.09, 0.18),     // 9% return, 18% volatility
            _ => (0.07, 0.12),
        };
    }

    private (double minReturn, double maxVolatility) DeriveRiskTolerance(RiskTolerance riskTolerance)
    {
        return riskTolerance switch
        {
            RiskTolerance.Conservative => (0.04, 0.08),
            RiskTolerance.Moderate => (0.06, 0.12),
            RiskTolerance.Aggressive => (0.08, 0.18),
            _ => (0.06, 0.12),
        };
    }

    private (double minWeight, double maxWeight) GetWeightConstraintsByRiskTolerance(RiskTolerance riskTolerance)
    {
        return riskTolerance switch
        {
            RiskTolerance.Conservative => (0.05, 0.40),   // More conservative allocations
            RiskTolerance.Moderate => (0.05, 0.50),       // Moderate allocations
            RiskTolerance.Aggressive => (0.05, 0.60),     // More aggressive allocations
            _ => (0.05, 0.50),
        };
    }

    private string GenerateRandomClientName()
    {
        var firstNames = new[] { "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis" };

        var firstName = firstNames[_random.Next(firstNames.Length)];
        var lastName = lastNames[_random.Next(lastNames.Length)];

        return $"{firstName} {lastName}";
    }
}
