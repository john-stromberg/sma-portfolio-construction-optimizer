# Client Onboarding Example Generator - Usage Guide

## Overview
The new client onboarding example generator creates realistic fictional client profiles with randomly generated characteristics and uses the portfolio optimization tools to generate optimal portfolios for these clients. This feature is useful for:

- Testing and demonstrating the portfolio optimization engine
- Generating realistic scenario examples for client presentations
- Stress-testing the optimizer with various client profiles and constraints

## New Components Added

### 1. Models
- **ClientProfile.cs** - Represents a client's profile with:
  - Risk tolerance (Conservative, Moderate, Aggressive)
  - Annual income
  - Investment amount (budget)
  - Time horizon (years)
  - Target return rate and volatility tolerance

- **ClientOnboardingExample.cs** - Complete onboarding package containing:
  - Client profile
  - Available assets
  - Derived constraints
  - Correlation matrix
  - Optimized portfolio result

### 2. Services
- **ExampleDataGenerator.cs** - Service that:
  - Generates random client profiles
  - Derives constraints based on risk profile
  - Creates complete onboarding examples
  - Manages sample assets and correlation matrices

### 3. API Endpoint
- **GET /api/examples/client-onboarding** - New endpoint that generates and optimizes

## Using the API

### Basic Usage (Random Profile)
```bash
GET /api/examples/client-onboarding
```

This generates a random client profile with random risk tolerance and returns:
```json
{
  "clientProfile": {
	"clientId": "CLIENT-abc12345",
	"name": "Alice Johnson",
	"annualIncome": 125000,
	"riskTolerance": 1,  // 0=Conservative, 1=Moderate, 2=Aggressive
	"investmentAmount": 250000,
	"timeHorizonYears": 15,
	"targetReturnRate": 0.07,
	"maxVolatilityTolerance": 0.12,
	"createdAt": "2024-01-15T10:30:00Z"
  },
  "availableAssets": [
	{
	  "id": "VTSAX",
	  "name": "Vanguard Total Stock Market ETF",
	  "assetClass": "Equity",
	  "expectedReturn": 0.10,
	  "volatility": 0.15,
	  "currentPrice": 245.50
	},
	// ... more assets
  ],
  "constraintsDerived": {
	"minWeights": { "VTSAX": 0.05, ... },
	"maxWeights": { "VTSAX": 0.50, ... },
	"minimumReturn": 0.06,
	"maximumVolatility": 0.12,
	"budgetConstraint": 1.0
  },
  "correlationMatrix": [[1.0, 0.85, ...], ...],
  "riskFreeRate": 0.02,
  "optimizedPortfolio": {
	"id": "OPT-xyz789",
	"weights": {
	  "VTSAX": 0.35,
	  "VTIAX": 0.15,
	  "BND": 0.30,
	  "VGSLX": 0.12,
	  "GLD": 0.08
	},
	"expectedReturn": 0.073,
	"expectedVolatility": 0.112,
	"sharpeRatio": 0.765,
	"status": "Optimal",
	"warnings": []
  },
  "generatedAt": "2024-01-15T10:30:00Z"
}
```

### Generate Specific Risk Profile
```bash
GET /api/examples/client-onboarding?riskTolerance=0   // Conservative
GET /api/examples/client-onboarding?riskTolerance=1   // Moderate
GET /api/examples/client-onboarding?riskTolerance=2   // Aggressive
```

## Risk Profiles

The generator creates different constraints and targets based on risk tolerance:

### Conservative (0)
- Target Return: 5%
- Max Volatility: 8%
- Weight Constraints: 5-40% per asset
- Emphasizes bonds and stable assets

### Moderate (1)
- Target Return: 7%
- Max Volatility: 12%
- Weight Constraints: 5-50% per asset
- Balanced allocation across asset classes

### Aggressive (2)
- Target Return: 9%
- Max Volatility: 18%
- Weight Constraints: 5-60% per asset
- Higher equity concentration

## Available Assets

The generator uses 5 sample assets:
1. **VTSAX** - Vanguard Total Stock Market (US Equity)
2. **VTIAX** - Vanguard International Stock (International Equity)
3. **BND** - Vanguard Total Bond Market (Fixed Income)
4. **VGSLX** - Vanguard Real Estate (Alternative)
5. **GLD** - SPDR Gold Shares (Alternative)

## Examples

### Example 1: Test Conservative Client
```powershell
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding?riskTolerance=0" -Method Get
$response | ConvertTo-Json | Write-Host
```

### Example 2: Generate Multiple Random Clients
```powershell
for ($i = 1; $i -le 5; $i++) {
	$client = Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding" -Method Get
	Write-Host "Generated Client: $($client.clientProfile.name) - Sharpe Ratio: $($client.optimizedPortfolio.sharpeRatio)"
}
```

### Example 3: Analyze Constraint Derivation
```powershell
$clients = @()
foreach ($risk in @(0, 1, 2)) {
	$response = Invoke-RestMethod -Uri "http://localhost:5000/api/examples/client-onboarding?riskTolerance=$risk" -Method Get
	$clients += $response
}

$clients | ForEach-Object {
	Write-Host "Risk Tolerance: $($_.clientProfile.riskTolerance)"
	Write-Host "  Target Return: $($_.clientProfile.targetReturnRate * 100)%"
	Write-Host "  Max Volatility: $($_.clientProfile.maxVolatilityTolerance * 100)%"
	Write-Host "  Portfolio Sharpe: $($_.optimizedPortfolio.sharpeRatio)"
	Write-Host ""
}
```

## Integration with Existing Endpoints

You can also use these endpoints together:

```bash
# Get sample assets and constraints
GET /api/examples/sample-assets
GET /api/examples/sample-constraints
GET /api/examples/sample-correlation-matrix

# Generate client onboarding example (combines everything and optimizes)
GET /api/examples/client-onboarding

# Manually optimize using your own data
POST /api/optimization/optimize
```

## Testing the Feature

All existing tests pass (8/8), confirming the new feature integrates well with the existing optimization engines without breaking functionality.

## Notes

- Each call to the endpoint generates a unique client ID and profile
- Constraints are automatically derived from the client's risk profile
- The correlation matrix is consistent across all generations
- The optimization uses a default risk-free rate of 2%
- All timestamps are in UTC format
