using Microsoft.AspNetCore.Mvc;
using PortfolioOptimization.Core.Models;

namespace PortfolioOptimization.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamplesController : ControllerBase
{
    private readonly ILogger<ExamplesController> _logger;

    public ExamplesController(ILogger<ExamplesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get example assets for testing.
    /// </summary>
    /// <returns>List of sample assets across different asset classes.</returns>
    [HttpGet("sample-assets")]
    public IActionResult GetSampleAssets()
    {
        var assets = new List<Asset>
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

        return Ok(assets);
    }

    /// <summary>
    /// Get example constraints for testing.
    /// </summary>
    /// <returns>Sample portfolio constraints.</returns>
    [HttpGet("sample-constraints")]
    public IActionResult GetSampleConstraints()
    {
        var constraints = new
        {
            minWeights = new Dictionary<string, double>
            {
                { "VTSAX", 0.10 },
                { "VTIAX", 0.05 },
                { "BND", 0.10 },
            },
            maxWeights = new Dictionary<string, double>
            {
                { "VTSAX", 0.40 },
                { "VTIAX", 0.30 },
                { "BND", 0.50 },
                { "VGSLX", 0.20 },
                { "GLD", 0.15 },
            },
            minimumReturn = 0.06,
            maximumVolatility = 0.12,
            budgetConstraint = 1.0,
        };

        return Ok(constraints);
    }

    /// <summary>
    /// Get example correlation matrix for multi-asset analysis.
    /// </summary>
    /// <returns>Sample correlation matrix.</returns>
    [HttpGet("sample-correlation-matrix")]
    public IActionResult GetSampleCorrelationMatrix()
    {
        var correlationMatrix = new double[,]
        {
            { 1.0, 0.85, -0.10, 0.60, 0.05 },      // VTSAX
            { 0.85, 1.0, -0.05, 0.50, 0.10 },      // VTIAX
            { -0.10, -0.05, 1.0, 0.20, 0.30 },     // BND
            { 0.60, 0.50, 0.20, 1.0, 0.15 },       // VGSLX
            { 0.05, 0.10, 0.30, 0.15, 1.0 },       // GLD
        };

        return Ok(new
        {
            description = "5x5 correlation matrix for sample assets",
            dimensions = new { rows = 5, columns = 5 },
            assetOrder = new[] { "VTSAX", "VTIAX", "BND", "VGSLX", "GLD" },
            matrix = correlationMatrix,
        });
    }
}
