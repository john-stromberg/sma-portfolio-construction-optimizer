using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PortfolioOptimization.Core.Models;

namespace PortfolioOptimization.Core.Services;

/// <summary>
/// Portfolio optimizer that calls Python engine via subprocess.
/// Used for production optimization with cvxpy support.
/// </summary>
public class PythonPortfolioOptimizer : IPortfolioOptimizer
{
    private readonly string _pythonExePath;
    private readonly string _optimizerScriptPath;
    private readonly ILogger<PythonPortfolioOptimizer> _logger;

    public PythonPortfolioOptimizer(
        ILogger<PythonPortfolioOptimizer> logger,
        string? pythonExePath = null,
        string? optimizerScriptPath = null)
    {
        _logger = logger;
        _pythonExePath = pythonExePath ?? "python";
        _optimizerScriptPath = optimizerScriptPath ?? 
            FindOptimizeRunner() ?? 
            throw new InvalidOperationException("Could not locate Python optimizer script.");
    }

    /// <summary>
    /// Find the optimize_runner.py script in the Python package.
    /// </summary>
    private static string? FindOptimizeRunner()
    {
        var searchPaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "..", "..", "python", "portfolio_optimizer", "optimize_runner.py"),
            "python/portfolio_optimizer/optimize_runner.py",
            "../python/portfolio_optimizer/optimize_runner.py",
            "../../python/portfolio_optimizer/optimize_runner.py",
        };

        foreach (var path in searchPaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    public async Task<OptimizedPortfolio> OptimizeAsync(
        List<Asset> assets,
        PortfolioConstraints constraints,
        double[,]? correlationMatrix = null,
        double riskFreeRate = 0.02)
    {
        return await Task.Run(() => Optimize(assets, constraints, correlationMatrix, riskFreeRate));
    }

    private OptimizedPortfolio Optimize(
        List<Asset> assets,
        PortfolioConstraints constraints,
        double[,]? correlationMatrix,
        double riskFreeRate)
    {
        if (assets.Count == 0)
            throw new ArgumentException("No assets provided for optimization.");

        try
        {
            // Build request JSON
            var request = BuildOptimizationRequest(assets, constraints, correlationMatrix, riskFreeRate);
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions 
            { 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            });

            _logger.LogInformation("Calling Python optimizer with {AssetCount} assets.", assets.Count);

            // Call Python subprocess
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _pythonExePath,
                    Arguments = $"\"{_optimizerScriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.StandardInput.Write(requestJson);
            process.StandardInput.Close();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!process.WaitForExit(300000)) // 5 minute timeout
            {
                process.Kill();
                throw new TimeoutException("Python optimizer exceeded 5-minute timeout.");
            }

            if (process.ExitCode != 0)
            {
                _logger.LogError("Python optimizer failed: {Error}", error);
                throw new InvalidOperationException($"Python optimizer failed: {error}");
            }

            // Parse response
            var result = JsonSerializer.Deserialize<PythonOptimizerResponse>(output)
                ?? throw new InvalidOperationException("Invalid optimizer response.");

            if (result.Status == "Error")
                throw new InvalidOperationException($"Optimization error: {result.Error}");

            // Map to OptimizedPortfolio
            return new OptimizedPortfolio
            {
                Id = result.Id,
                Weights = result.Weights,
                ExpectedReturn = result.ExpectedReturn,
                ExpectedVolatility = result.ExpectedVolatility,
                SharpeRatio = result.SharpeRatio,
                Status = result.Status,
                Warnings = result.Warnings ?? new List<string>(),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Python optimizer call failed.");
            throw;
        }
    }

    private object BuildOptimizationRequest(
        List<Asset> assets,
        PortfolioConstraints constraints,
        double[,]? correlationMatrix,
        double riskFreeRate)
    {
        var assetsList = assets.Select(a => new
        {
            a.Id,
            a.Name,
            a.AssetClass,
            a.ExpectedReturn,
            a.Volatility,
            a.CurrentPrice,
        }).ToList();

        var constraintsObj = new
        {
            constraints.MinWeights,
            constraints.MaxWeights,
            constraints.MinimumReturn,
            constraints.MaximumVolatility,
            constraints.BudgetConstraint,
        };

        return new
        {
            assets = assetsList,
            constraints = constraintsObj,
            correlation_matrix = correlationMatrix,
            risk_free_rate = riskFreeRate,
        };
    }
}

/// <summary>
/// Response model from Python optimizer.
/// </summary>
internal class PythonOptimizerResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("weights")]
    public Dictionary<string, double> Weights { get; set; } = new();

    [JsonPropertyName("expected_return")]
    public double ExpectedReturn { get; set; }

    [JsonPropertyName("expected_volatility")]
    public double ExpectedVolatility { get; set; }

    [JsonPropertyName("sharpe_ratio")]
    public double SharpeRatio { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("warnings")]
    public List<string>? Warnings { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
