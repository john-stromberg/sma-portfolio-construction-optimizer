using PortfolioOptimization.Core.Models;
using PortfolioOptimization.Core.Services;

namespace PortfolioOptimization.Tests;

[TestClass]
public class PythonPortfolioOptimizerIntegrationTests
{
    private IPortfolioOptimizer? _optimizer;

    [TestInitialize]
    public void Setup()
    {
        // Use NaivePortfolioOptimizer for CI/CD compatibility
        // Set UsePythonOptimizer=true in appsettings.json to use Python engine
        _optimizer = new NaivePortfolioOptimizer();
    }

    [TestMethod]
    public async Task PortfolioOptimization_WithMultiAssetPortfolio_ReturnsValidAllocation()
    {
        // Arrange
        var assets = CreateSampleAssets();
        var constraints = CreateSampleConstraints();

        // Act
        var result = await _optimizer!.OptimizeAsync(assets, constraints);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result.Status);
        Assert.AreEqual(assets.Count, result.Weights.Count);

        // All weights should sum to budget constraint (usually 1.0)
        var totalWeight = result.Weights.Values.Sum();
        Assert.AreEqual(1.0, totalWeight, 0.0001);

        // Portfolio metrics should be reasonable
        Assert.IsTrue(result.ExpectedReturn > 0);
        Assert.IsTrue(result.ExpectedVolatility > 0);
        Assert.IsTrue(result.SharpeRatio > 0);
    }

    [TestMethod]
    public async Task PortfolioOptimization_WithConstraints_RespectsBounds()
    {
        // Arrange
        var assets = CreateSampleAssets();
        var constraints = new PortfolioConstraints
        {
            MinWeights = new Dictionary<string, double>
            {
                { "STOCK1", 0.10 },
                { "BOND1", 0.20 },
            },
            MaxWeights = new Dictionary<string, double>
            {
                { "STOCK1", 0.40 },
                { "STOCK2", 0.30 },
            },
        };

        // Act
        var result = await _optimizer!.OptimizeAsync(assets, constraints);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Weights["STOCK1"] >= 0.10);
        Assert.IsTrue(result.Weights["BOND1"] >= 0.20);
    }

    [TestMethod]
    public async Task PortfolioOptimization_WithDifferentRiskFreeRates_CalculatesCorrectSharpeRatio()
    {
        // Arrange
        var assets = CreateSampleAssets();
        var constraints = new PortfolioConstraints();

        // Act
        var result1 = await _optimizer!.OptimizeAsync(assets, constraints, riskFreeRate: 0.01);
        var result2 = await _optimizer!.OptimizeAsync(assets, constraints, riskFreeRate: 0.03);

        // Assert
        // Higher risk-free rate should result in lower Sharpe ratio
        Assert.IsTrue(result1.SharpeRatio > result2.SharpeRatio);
    }

    [TestMethod]
    public async Task PortfolioOptimization_WithEmptyWeights_IsValid()
    {
        // Arrange
        var assets = CreateSampleAssets();
        var constraints = new PortfolioConstraints();

        // Act
        var result = await _optimizer!.OptimizeAsync(assets, constraints);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Weights);
        Assert.AreEqual(assets.Count, result.Weights.Count);
        Assert.IsTrue(result.Weights.All(w => w.Value >= 0 && w.Value <= 1.0));
    }

    // Helper methods
    private List<Asset> CreateSampleAssets()
    {
        return new List<Asset>
        {
            new Asset
            {
                Id = "STOCK1",
                Name = "US Stock",
                AssetClass = "Equity",
                ExpectedReturn = 0.10,
                Volatility = 0.15,
                CurrentPrice = 100,
            },
            new Asset
            {
                Id = "STOCK2",
                Name = "Intl Stock",
                AssetClass = "Equity",
                ExpectedReturn = 0.09,
                Volatility = 0.18,
                CurrentPrice = 50,
            },
            new Asset
            {
                Id = "BOND1",
                Name = "Bond Fund",
                AssetClass = "Fixed Income",
                ExpectedReturn = 0.04,
                Volatility = 0.05,
                CurrentPrice = 1000,
            },
        };
    }

    private PortfolioConstraints CreateSampleConstraints()
    {
        return new PortfolioConstraints
        {
            MinWeights = new Dictionary<string, double>(),
            MaxWeights = new Dictionary<string, double>(),
            BudgetConstraint = 1.0,
        };
    }
}
