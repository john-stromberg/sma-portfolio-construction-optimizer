using PortfolioOptimization.Core.Models;
using PortfolioOptimization.Core.Services;

namespace PortfolioOptimization.Tests;

[TestClass]
public class NaivePortfolioOptimizerTests
{
    private IPortfolioOptimizer _optimizer = null!;

    [TestInitialize]
    public void Setup()
    {
        _optimizer = new NaivePortfolioOptimizer();
    }

    [TestMethod]
    public async Task Optimize_WithValidAssets_ReturnsEqualWeights()
    {
        // Arrange
        var assets = new List<Asset>
        {
            new Asset { Id = "STOCK1", Name = "Stock 1", AssetClass = "Equity", ExpectedReturn = 0.10, Volatility = 0.15, CurrentPrice = 100 },
            new Asset { Id = "STOCK2", Name = "Stock 2", AssetClass = "Equity", ExpectedReturn = 0.12, Volatility = 0.18, CurrentPrice = 50 },
            new Asset { Id = "BOND1", Name = "Bond 1", AssetClass = "Fixed Income", ExpectedReturn = 0.04, Volatility = 0.05, CurrentPrice = 1000 }
        };

        var constraints = new PortfolioConstraints();

        // Act
        var result = await _optimizer.OptimizeAsync(assets, constraints);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Weights.Count);
        Assert.AreEqual(1.0 / 3.0, result.Weights["STOCK1"], 0.0001);
        Assert.AreEqual(1.0 / 3.0, result.Weights["STOCK2"], 0.0001);
        Assert.AreEqual(1.0 / 3.0, result.Weights["BOND1"], 0.0001);
        Assert.AreEqual("Success", result.Status);
        Assert.IsTrue(result.Warnings.Any());
    }

    [TestMethod]
    public async Task Optimize_WithNoAssets_ThrowsException()
    {
        // Arrange
        var assets = new List<Asset>();
        var constraints = new PortfolioConstraints();

        // Act & Assert
        try
        {
            await _optimizer.OptimizeAsync(assets, constraints);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException ex)
        {
            Assert.IsTrue(ex.Message.Contains("No assets"));
        }
    }

    [TestMethod]
    public async Task Optimize_CalculatesSharpeRatio()
    {
        // Arrange
        var assets = new List<Asset>
        {
            new Asset { Id = "STOCK1", Name = "Stock 1", AssetClass = "Equity", ExpectedReturn = 0.10, Volatility = 0.15, CurrentPrice = 100 }
        };

        var constraints = new PortfolioConstraints();

        // Act
        var result = await _optimizer.OptimizeAsync(assets, constraints, riskFreeRate: 0.02);

        // Assert
        var expectedSharpeRatio = (0.10 - 0.02) / 0.15;
        Assert.AreEqual(expectedSharpeRatio, result.SharpeRatio, 0.0001);
    }
}
