# sma-portfolio-construction-optimizer

Track: **SMA Portfolio Management**

## Project purpose

Multi-asset portfolio construction and optimization engine with a **hybrid C#/Python architecture**. Provides:
- Constrained portfolio optimization (equal-weight naive baseline, cvxpy for production)
- Sharpe ratio maximization and risk metrics
- ASP.NET Core API for integration with portfolio workflows
- Real-time and batch optimization capabilities

Supports **equities, bonds, alternatives, ETFs, and mutual funds**.

## Quick Start

### Option 1: Run Python Workflow (Fastest)
```bash
cd python
python -m venv .venv
.venv\Scripts\Activate.ps1
pip install -e .
python -m portfolio_optimizer.workflow data/sample_assets.json data/sample_constraints.json output/
```

### Option 2: Run C# API
```bash
cd csharp
dotnet build
dotnet run --project PortfolioOptimization.API/

# In another terminal, test the API:
curl http://localhost:5000/api/optimization/health
```

### Option 3: Run Tests
```bash
# C# tests
cd csharp && dotnet test

# Python tests
cd python && pytest tests/ -v
```

## Architecture

### C# Backend (ASP.NET Core Web API)
- **Location**: `csharp/PortfolioOptimization.API`
- **Projects**:
  - `PortfolioOptimization.Core`: Domain models, interfaces, business logic
  - `PortfolioOptimization.API`: REST API controllers, service registration
  - `PortfolioOptimization.Tests`: Unit and integration tests (MSTest)

- **Key Components**:
  - `IPortfolioOptimizer` interface with two implementations:
    - `NaivePortfolioOptimizer`: Equal-weight baseline (for testing/fallback)
    - `PythonPortfolioOptimizer`: Production optimizer via Python subprocess
  - REST endpoints:
    - `/api/optimization/health` - Health check
    - `/api/optimization/optimize` - Submit optimization request
    - `/api/examples/sample-assets` - Get example assets
    - `/api/examples/sample-constraints` - Get example constraints
    - `/api/examples/sample-correlation-matrix` - Get correlation matrix

### Python Analytics Engine
- **Location**: `python/portfolio_optimizer`
- **Package**: `portfolio_optimizer`
- **Key Modules**:
  - `models.py`: Data structures (Asset, PortfolioConstraints, OptimizedPortfolio)
  - `optimizer.py`: PortfolioOptimizer class with covariance-based optimization
  - `optimize_runner.py`: Subprocess entry point (reads JSON from stdin, outputs JSON)
  - `bridge.py`: C#-to-Python IPC layer (optional for future use)

- **Features**:
  - Naive equal-weight allocation (current)
  - Support for correlation matrices
  - Sharpe ratio calculation
  - Extensible for cvxpy constraints (planned)

### Integration Strategy
1. C# API receives optimization request
2. Serializes request to JSON
3. Spawns Python subprocess via `PythonPortfolioOptimizer`
4. Python `optimize_runner.py` receives JSON on stdin
5. Python optimizer runs, outputs result JSON on stdout
6. C# deserializes response, returns to client

## Local development workflow

### C# API Setup
```bash
cd csharp
dotnet build
dotnet run --project PortfolioOptimization.API/

# In another terminal:
curl http://localhost:5000/api/optimization/health
```

### Python Engine Setup
```bash
cd python
python -m venv .venv
.venv\Scripts\Activate.ps1
pip install -e .
pytest tests/  # Run tests
python -m portfolio_optimizer.optimize_runner < sample_request.json
```

### Run Full Integration Test
```bash
# 1. Start C# API
cd csharp
dotnet run --project PortfolioOptimization.API/

# 2. Send optimization request (with UsePythonOptimizer=true in appsettings.json)
curl -X POST http://localhost:5000/api/optimization/optimize \
  -H "Content-Type: application/json" \
  -d @sample_request.json
```

## Repository structure

- `csharp/` - Visual Studio solution with C# projects
  - `PortfolioOptimization.slnx` - Solution file
  - `PortfolioOptimization.Core/` - Core business logic
  - `PortfolioOptimization.API/` - ASP.NET Core Web API
  - `PortfolioOptimization.Tests/` - Unit tests
- `python/` - Python package for optimization
  - `portfolio_optimizer/` - Main package
  - `tests/` - pytest test suite
  - `setup.py` - Package configuration
  - `requirements.txt` - Python dependencies
- `src/` - Original Python analytics
- `api/` - Python FastAPI/Flask service (placeholder)
- `apps/` - Streamlit dashboard (placeholder)
- `data/` - Sample datasets
- `notebooks/` - Exploratory analysis
- `reports/` - Decision memos and outputs

## Expected outputs

From optimization:
- **Weights**: Recommended asset allocation (dictionary of asset_id -> weight)
- **Metrics**: Expected return, volatility, Sharpe ratio
- **Status**: Success/Error status with warnings
- **Visualization**: (Planned) Dashboard charts and risk heatmaps

## Building and Testing

### C# Build
```bash
cd csharp
dotnet build
dotnet test
```

### Python Test
```bash
cd python
pytest tests/ -v
```

### Solution Build (both layers)
```bash
dotnet build csharp
pytest python/tests/
```

## Test Results

### C# Tests (MSTest)
```
PortfolioOptimization.Tests.dll
  NaivePortfolioOptimizerTests
    ✓ Optimize_WithValidAssets_ReturnsEqualWeights
    ✓ Optimize_WithNoAssets_ThrowsException
    ✓ Optimize_CalculatesSharpeRatio

  PythonPortfolioOptimizerIntegrationTests
    ✓ PortfolioOptimization_WithMultiAssetPortfolio_ReturnsValidAllocation
    ✓ PortfolioOptimization_WithConstraints_RespectsBounds
    ✓ PortfolioOptimization_WithDifferentRiskFreeRates_CalculatesCorrectSharpeRatio
    ✓ PortfolioOptimization_WithEmptyWeights_IsValid
    ✓ OptimizationController_Health_ReturnsOk

Total: 8 passed, 0 failed
```

### Python Tests (pytest)
```
portfolio_optimizer/tests/test_optimizer.py
  ✓ test_optimizer_with_valid_assets
  ✓ test_optimizer_equal_weight_allocation
  ✓ test_optimizer_with_no_assets
  ✓ test_optimizer_sharpe_ratio_calculation
  ✓ test_optimizer_with_correlation_matrix
  ✓ test_optimizer_includes_warnings

Total: 6 passed, 0 failed
```

## Configuration

### Enable Python Optimizer
Edit `csharp/PortfolioOptimization.API/appsettings.json`:
```json
{
  "UsePythonOptimizer": true
}
```

Default: `false` (uses NaivePortfolioOptimizer)

## Decision memo template

- **Question:** What portfolio construction is being optimized?
- **Evidence:** Which metrics (Sharpe, return, volatility) drive the decision?
- **Interpretation:** How does allocation differ from benchmarks? What risks are managed?
- **Action:** Adopt allocation, backtest further, or adjust constraints.

## Next Steps

- [ ] Implement cvxpy-based constrained optimization in Python
- [ ] Add correlation matrix parsing and validation
- [ ] Create Streamlit dashboard for visualization
- [ ] Add batch processing pipeline
- [ ] Implement performance attribution
- [ ] Add live market data feeds


