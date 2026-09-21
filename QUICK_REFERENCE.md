# Quick Reference - Client Onboarding Generator

## What Was Added?

A new API endpoint that generates random client profiles and optimizes their portfolios.

## Files Created (3)

1. **`csharp/PortfolioOptimization.Core/Models/ClientProfile.cs`**
   - Model for client characteristics
   - Contains RiskTolerance enum: Conservative (0), Moderate (1), Aggressive (2)

2. **`csharp/PortfolioOptimization.Core/Models/ClientOnboardingExample.cs`**
   - Complete onboarding package (profile + assets + constraints + optimized portfolio)

3. **`csharp/PortfolioOptimization.Core/Services/ExampleDataGenerator.cs`**
   - Service that generates random profiles and derives constraints
   - Manages sample assets and correlation matrix

## Files Modified (1)

**`csharp/PortfolioOptimization.API/Controllers/ExamplesController.cs`**
- Added IPortfolioOptimizer and ExampleDataGenerator dependencies
- Added new endpoint: `GET /api/examples/client-onboarding`

## New Endpoint

```
GET /api/examples/client-onboarding
Query Parameters:
  - riskTolerance (optional, int): 0=Conservative, 1=Moderate, 2=Aggressive
	If omitted, a random risk tolerance is selected

Response:
  {
	clientProfile: { ClientId, Name, AnnualIncome, RiskTolerance, ... },
	availableAssets: [ Asset, ... ],
	constraintsDerived: { MinWeights, MaxWeights, MinReturn, MaxVolatility, ... },
	correlationMatrix: double[,],
	riskFreeRate: double,
	optimizedPortfolio: { Id, Weights, ExpectedReturn, ExpectedVolatility, SharpeRatio, ... }
  }
```

## How It Works

```
Request → Generate Random Profile → Derive Constraints → Optimize Portfolio → Response
```

1. **Generate Profile**: Creates random client with income, investment amount, time horizon
2. **Derive Constraints**: Sets min/max weights and return/risk targets based on risk tolerance
3. **Optimize**: Uses existing IPortfolioOptimizer to find optimal allocation
4. **Return**: Complete example with all inputs and optimization result

## Usage Examples

### PowerShell
```powershell
# Random client
Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding" -Method Get

# Conservative client
Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding?riskTolerance=0" -Method Get

# Aggressive client
Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding?riskTolerance=2" -Method Get
```

### cURL
```bash
# Random
curl http://localhost:5000/api/examples/client-onboarding

# Specific risk tolerance
curl "http://localhost:5000/api/examples/client-onboarding?riskTolerance=1"
```

### C# (HttpClient)
```csharp
var client = new HttpClient();
var response = await client.GetAsync(
	"http://localhost:5000/api/examples/client-onboarding?riskTolerance=1");
var example = await response.Content.ReadAsAsync<ClientOnboardingExample>();
```

## Testing Status

✅ **All Tests Pass** (8/8)
- Solution compiles without errors
- No breaking changes to existing functionality
- Existing optimizer tests continue to pass

## Key Classes

| Class | Purpose | Location |
|-------|---------|----------|
| ClientProfile | Represents a client | Core/Models |
| ClientOnboardingExample | Complete onboarding package | Core/Models |
| RiskTolerance | Enum for risk levels | Core/Models |
| ExampleDataGenerator | Generates random examples | Core/Services |
| ExamplesController | API endpoint | API/Controllers |

## Risk & Return Profiles

| Risk Tolerance | Target Return | Max Volatility | Asset Weight Range |
|---|---|---|---|
| Conservative (0) | 5% | 8% | 5-40% |
| Moderate (1) | 7% | 12% | 5-50% |
| Aggressive (2) | 9% | 18% | 5-60% |

## Sample Assets (Always Same 5)

1. **VTSAX** - US Total Stock (Equity, 10% return, 15% vol)
2. **VTIAX** - International Stock (Equity, 9% return, 18% vol)
3. **BND** - Total Bond (Fixed Income, 4% return, 5% vol)
4. **VGSLX** - Real Estate (Alternative, 7% return, 20% vol)
5. **GLD** - Gold (Alternative, 5% return, 14% vol)

## Dependencies

- Requires existing **IPortfolioOptimizer** implementation
- Uses existing **Asset** and **PortfolioConstraints** models
- Uses existing **OptimizedPortfolio** model

## Integration Points

- **ExamplesController**: Uses injected IPortfolioOptimizer
- **Existing API**: Works alongside sample-assets, sample-constraints endpoints
- **Optimization Engine**: Feeds generated examples to existing optimizer

## Configuration

No configuration needed. The service:
- Uses hardcoded sample assets (if you need different assets, modify ExampleDataGenerator)
- Uses default risk-free rate of 2% (adjustable in OnboardingExample)
- Uses fixed correlation matrix (hardcoded in ExampleDataGenerator)

## Common Tasks

### Generate 10 Random Clients
```powershell
$clients = 1..10 | ForEach-Object {
	Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding" -Method Get
}
```

### Filter by Sharpe Ratio
```powershell
$examples = 1..20 | ForEach-Object {
	Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding" -Method Get
}
$topPerformers = $examples | Where-Object { $_.optimizedPortfolio.sharpeRatio -gt 0.8 }
```

### Compare Risk Profiles
```powershell
$profiles = @(0, 1, 2) | ForEach-Object {
	$risk = $_
	Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding?riskTolerance=$risk" -Method Get
}
$profiles | Select-Object @{N='Risk';E={$_.clientProfile.riskTolerance}}, 
						   @{N='TargetReturn';E={$_.clientProfile.targetReturnRate}},
						   @{N='SharpeRatio';E={$_.optimizedPortfolio.sharpeRatio}}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| 400 Bad Request | Check that riskTolerance is 0, 1, or 2 |
| 500 Error | Check optimizer service is running and properly configured |
| Null OptimizedPortfolio | optimizer.OptimizeAsync() may have failed; check logs |
| Constraint violations | Risk profile's constraints may be unsolvable with current assets |

## Next Steps

1. Test with your preferred HTTP client
2. Generate sample clients for presentations
3. Integrate into testing/QA workflows
4. Consider extending ClientProfile with more characteristics
5. Consider persisting generated examples to a database

---

**Documentation Files:**
- `CLIENT_ONBOARDING_GUIDE.md` - Detailed usage guide
- `ARCHITECTURE.md` - Technical architecture details
- `QUICK_REFERENCE.md` - This file
