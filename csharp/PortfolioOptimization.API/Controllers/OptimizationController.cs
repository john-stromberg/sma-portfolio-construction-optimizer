using Microsoft.AspNetCore.Mvc;
using PortfolioOptimization.Core.Models;
using PortfolioOptimization.Core.Services;

namespace PortfolioOptimization.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptimizationController : ControllerBase
{
    private readonly IPortfolioOptimizer _optimizer;
    private readonly ILogger<OptimizationController> _logger;

    public OptimizationController(
        IPortfolioOptimizer optimizer,
        ILogger<OptimizationController> logger)
    {
        _optimizer = optimizer;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Optimize a portfolio given assets and constraints.
    /// </summary>
    /// <param name="request">Optimization request containing assets and constraints.</param>
    /// <returns>Optimized portfolio solution.</returns>
    [HttpPost("optimize")]
    public async Task<ActionResult<OptimizedPortfolio>> Optimize(
        [FromBody] OptimizationRequest request)
    {
        try
        {
            _logger.LogInformation("Received optimization request with {AssetCount} assets.", request.Assets.Count);

            var result = await _optimizer.OptimizeAsync(
                request.Assets,
                request.Constraints,
                request.CorrelationMatrix,
                request.RiskFreeRate ?? 0.02);

            _logger.LogInformation("Optimization completed. Portfolio Sharpe Ratio: {SharpeRatio:F4}", result.SharpeRatio);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Optimization failed.");
            return StatusCode(500, new { error = "Optimization failed.", details = ex.Message });
        }
    }
}

/// <summary>
/// Request model for portfolio optimization.
/// </summary>
public class OptimizationRequest
{
    public required List<Asset> Assets { get; set; }
    public required PortfolioConstraints Constraints { get; set; }
    public double[,]? CorrelationMatrix { get; set; }
    public double? RiskFreeRate { get; set; }
}
