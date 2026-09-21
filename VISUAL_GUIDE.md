# Visual Guide - Client Onboarding Flow

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         Client Application                      │
│                    (Web/Mobile/Desktop)                         │
└────────────────────────────────┬────────────────────────────────┘
								 │
					GET /api/examples/client-onboarding
					[?riskTolerance=1]
								 │
								 ▼
┌─────────────────────────────────────────────────────────────────┐
│                   PortfolioOptimization.API                     │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │         ExamplesController                              │  │
│  │                                                           │  │
│  │  GenerateClientOnboardingExample(int? riskTolerance)    │  │
│  │                                                           │  │
│  │  IF riskTolerance is provided:                          │  │
│  │      param = (RiskTolerance)riskTolerance              │  │
│  │  ELSE:                                                  │  │
│  │      param = null (random)                              │  │
│  │                                                           │  │
│  │  ┌─────────────────────────────────────────────────┐     │  │
│  │  │ Step 1: Generate                               │     │  │
│  │  │                                                 │     │  │
│  │  │ example = _exampleDataGenerator                │     │  │
│  │  │   .GenerateClientOnboardingExample(param)      │     │  │
│  │  └─────────────────────────────────────────────────┘     │  │
│  │                     │                                     │  │
│  │                     ▼                                     │  │
│  │  ┌─────────────────────────────────────────────────┐     │  │
│  │  │ Step 2: Optimize                               │     │  │
│  │  │                                                 │     │  │
│  │  │ optimizedPortfolio = await _optimizer          │     │  │
│  │  │   .OptimizeAsync(                              │     │  │
│  │  │     example.AvailableAssets,                   │     │  │
│  │  │     example.ConstraintsDerived,                │     │  │
│  │  │     example.CorrelationMatrix,                 │     │  │
│  │  │     example.RiskFreeRate)                      │     │  │
│  │  └─────────────────────────────────────────────────┘     │  │
│  │                     │                                     │  │
│  │                     ▼                                     │  │
│  │  ┌─────────────────────────────────────────────────┐     │  │
│  │  │ Step 3: Attach & Return                        │     │  │
│  │  │                                                 │     │  │
│  │  │ example.OptimizedPortfolio = optimizedPortfolio│     │  │
│  │  │ return Ok(example)                              │     │  │
│  │  └─────────────────────────────────────────────────┘     │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
								 │
								 ▼
┌─────────────────────────────────────────────────────────────────┐
│               PortfolioOptimization.Core                        │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │   ExampleDataGenerator (Services)                       │  │
│  │                                                           │  │
│  │   GenerateClientOnboardingExample(RiskTolerance?)       │  │
│  │                                                           │  │
│  │   ┌──────────────────────────────────────────────────┐   │  │
│  │   │ GenerateRandomClientProfile()                   │   │  │
│  │   │ • Random name                                   │   │  │
│  │   │ • Random income ($50k-$500k)                   │   │  │
│  │   │ • Random investment ($10k-$500k)               │   │  │
│  │   │ • Random horizon (5-40 years)                  │   │  │
│  │   │ • Given or random risk tolerance               │   │  │
│  │   │ • Derive target return & volatility            │   │  │
│  │   │ Returns: ClientProfile                         │   │  │
│  │   └──────────────────────────────────────────────────┘   │  │
│  │                     │                                     │  │
│  │                     ▼                                     │  │
│  │   ┌──────────────────────────────────────────────────┐   │  │
│  │   │ GenerateConstraintsFromProfile()                │   │  │
│  │   │ • Set MinimumReturn (from profile)              │   │  │
│  │   │ • Set MaximumVolatility (from profile)          │   │  │
│  │   │ • For each asset:                               │   │  │
│  │   │   - If Fixed Income: higher bounds              │   │  │
│  │   │   - If Equity: standard bounds                  │   │  │
│  │   │   - If Alternative: lower bounds                │   │  │
│  │   │ Returns: PortfolioConstraints                   │   │  │
│  │   └──────────────────────────────────────────────────┘   │  │
│  │                     │                                     │  │
│  │                     ▼                                     │  │
│  │   Returns: ClientOnboardingExample                        │  │
│  │   • ClientProfile                                         │  │
│  │   • List<Asset> (5 assets)                                │  │
│  │   • PortfolioConstraints                                  │  │
│  │   • Correlation matrix 5x5                                │  │
│  │   • RiskFreeRate = 0.02                                   │  │
│  │   • OptimizedPortfolio = null (filled later)              │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │   IPortfolioOptimizer (Services)                        │  │
│  │                                                           │  │
│  │   OptimizeAsync(assets, constraints, matrix, rate)      │  │
│  │   • Considers all constraints                            │  │
│  │   • Analyzes asset returns and volatility                │  │
│  │   • Uses correlation matrix                              │  │
│  │   • Optimizes for maximum Sharpe ratio                   │  │
│  │   Returns: OptimizedPortfolio                            │  │
│  │   • Id                                                    │  │
│  │   • Weights (Dictionary<AssetId, Weight>)                │  │
│  │   • ExpectedReturn                                        │  │
│  │   • ExpectedVolatility                                    │  │
│  │   • SharpeRatio                                           │  │
│  │   • Status & Warnings                                     │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │   Models                                                 │  │
│  │                                                           │  │
│  │   ClientProfile                PortfolioConstraints       │  │
│  │   • ClientId                   • MinWeights               │  │
│  │   • Name                        • MaxWeights              │  │
│  │   • AnnualIncome                • MinimumReturn           │  │
│  │   • RiskTolerance (enum)        • MaximumVolatility       │  │
│  │   • InvestmentAmount            • BudgetConstraint        │  │
│  │   • TimeHorizonYears                                       │  │
│  │   • TargetReturnRate            RiskTolerance (enum)      │  │
│  │   • MaxVolatilityTolerance      • Conservative = 0        │  │
│  │   • CreatedAt                   • Moderate = 1            │  │
│  │                                 • Aggressive = 2          │  │
│  │   ClientOnboardingExample                                  │  │
│  │   • ClientProfile                                          │  │
│  │   • AvailableAssets                                        │  │
│  │   • ConstraintsDerived                                     │  │
│  │   • CorrelationMatrix                                      │  │
│  │   • RiskFreeRate                                           │  │
│  │   • OptimizedPortfolio                                     │  │
│  │   • GeneratedAt                                            │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
								 │
								 ▼
					200 OK - ClientOnboardingExample
							  (JSON formatted)
```

---

## Risk Tolerance Decision Tree

```
						Is Risk Tolerance Specified?
							   /        \
							Yes          No
							/              \
						   ▼                ▼
					Use Provided         Generate Random
											  ▼
					┌────────────────────────────────────┐
					│   Random(0, 1, 2)                 │
					│   (1/3 each)                       │
					└────────────────────────────────────┘
							   │
				┌──────────────┼──────────────┐
				▼              ▼              ▼
			┌───────┐    ┌───────────┐   ┌──────────┐
			│   0   │    │     1     │   │    2     │
			│  CON  │    │    MOD    │   │   AGG    │
			└───────┘    └───────────┘   └──────────┘
				│              │              │
				▼              ▼              ▼
		 Return:5%      Return:7%       Return:9%
		 Vol: 8%        Vol: 12%        Vol: 18%
		 Min Eq: 5%     Min Eq: 5%      Min Eq: 5%
		 Max Eq: 40%    Max Eq: 50%     Max Eq: 60%
		 Min FI: 10%    Min FI: 10%     Min FI: 10%
		 Max FI: 80%    Max FI: 100%    Max FI: 120%
				│              │              │
				└──────────────┴──────────────┘
							   │
							   ▼
					Create ClientProfile
					Derive Constraints
					Assemble Example
```

---

## Optimization Process

```
				  ClientOnboardingExample
						 │
		┌────────────────┼────────────────┐
		│                │                │
		▼                ▼                ▼
	Assets         Constraints       Correlation
	-------        -----------       -----------
	VTSAX          MinWeights        1.00  0.85 -0.10 0.60 0.05
	VTIAX          MaxWeights        0.85  1.00 -0.05 0.50 0.10
	BND            MinReturn         -0.10 -0.05 1.00 0.20 0.30
	VGSLX          MaxVolatility     0.60  0.50  0.20 1.00 0.15
	GLD            BudgetConstraint  0.05  0.10  0.30 0.15 1.00
		│
		└──────────────────────┬──────────────────────┘
							   │
							   ▼
				  ┌─────────────────────────┐
				  │   Optimization Engine   │
				  │   (IPortfolioOptimizer) │
				  │                         │
				  │  Objective: Maximize    │
				  │  (Return - RiskFree) /  │
				  │     Volatility          │
				  │  (Sharpe Ratio)         │
				  │                         │
				  │  Subject to:            │
				  │  • Min/Max weights      │
				  │  • Min return           │
				  │  • Max volatility       │
				  │  • Sum weights = 1.0    │
				  └─────────────────────────┘
							   │
							   ▼
				  ┌─────────────────────────┐
				  │  OptimizedPortfolio     │
				  │  ─────────────────────  │
				  │  Weights:               │
				  │    VTSAX: 32%           │
				  │    VTIAX: 18%           │
				  │    BND:   35%           │
				  │    VGSLX: 10%           │
				  │    GLD:   5%            │
				  │                         │
				  │  Return: 7.1%           │
				  │  Volatility: 11.0%      │
				  │  Sharpe: 0.779          │
				  │  Status: Optimal        │
				  └─────────────────────────┘
							   │
							   ▼
				  ┌─────────────────────────┐
				  │ ClientOnboardingExample │
				  │ (with portfolio result) │
				  └─────────────────────────┘
```

---

## Constraint Derivation Examples

### Example 1: Conservative Client
```
Input Client Profile:
  • Risk Tolerance: Conservative (0)
  • Annual Income: $120,000
  • Investment: $200,000
  • Time Horizon: 20 years

Derived Constraints:
  ┌──────────────────────────────────────────┐
  │ Target Return: 5%                        │
  │ Max Volatility: 8%                       │
  │                                          │
  │ Asset    │ Min Weight │ Max Weight │      │
  │──────────┼────────────┼────────────┤      │
  │ VTSAX    │    5%      │    40%     │      │
  │ VTIAX    │    5%      │    40%     │      │
  │ BND      │   10%      │    80%     │      │
  │ VGSLX    │   2.5%     │    20%     │      │
  │ GLD      │   2.5%     │    20%     │      │
  └──────────────────────────────────────────┘

Expected Outcome:
  • Heavy bond allocation (conservative)
  • Lower equity exposure
  • Emphasis on capital preservation
```

### Example 2: Moderate Client
```
Input Client Profile:
  • Risk Tolerance: Moderate (1)
  • Annual Income: $185,000
  • Investment: $350,000
  • Time Horizon: 25 years

Derived Constraints:
  ┌──────────────────────────────────────────┐
  │ Target Return: 7%                        │
  │ Max Volatility: 12%                      │
  │                                          │
  │ Asset    │ Min Weight │ Max Weight │      │
  │──────────┼────────────┼────────────┤      │
  │ VTSAX    │    5%      │    50%     │      │
  │ VTIAX    │    5%      │    50%     │      │
  │ BND      │   10%      │   100%     │      │
  │ VGSLX    │   2.5%     │    25%     │      │
  │ GLD      │   2.5%     │    25%     │      │
  └──────────────────────────────────────────┘

Expected Outcome:
  • Balanced equity/bond split
  • Diversified allocation
  • Moderate growth orientation
```

### Example 3: Aggressive Client
```
Input Client Profile:
  • Risk Tolerance: Aggressive (2)
  • Annual Income: $250,000
  • Investment: $500,000
  • Time Horizon: 30 years

Derived Constraints:
  ┌──────────────────────────────────────────┐
  │ Target Return: 9%                        │
  │ Max Volatility: 18%                      │
  │                                          │
  │ Asset    │ Min Weight │ Max Weight │      │
  │──────────┼────────────┼────────────┤      │
  │ VTSAX    │    5%      │    60%     │      │
  │ VTIAX    │    5%      │    60%     │      │
  │ BND      │   10%      │   120%*    │      │
  │ VGSLX    │   2.5%     │    30%     │      │
  │ GLD      │   2.5%     │    30%     │      │
  └──────────────────────────────────────────┘
  * Note: Bond max >100% allows some flexibility
		 in meeting overall constraint

Expected Outcome:
  • High equity concentration (50%+)
  • Lower bond allocation (< 30%)
  • Emphasis on growth
```

---

## Data Flow Journey

```
User sends request
  ↓
Request reaches API with optional riskTolerance parameter
  ↓
Controller validates parameter (0, 1, 2, or null)
  ↓
Generator creates random profile
  ├─ If riskTolerance specified → use it
  └─ If null → generate random (0, 1, or 2)
  ↓
Profile includes:
  ├─ Random name, income, investment, horizon
  └─ Derived target return & volatility from risk level
  ↓
Generator derives constraints
  ├─ Set min returns based on risk profile
  ├─ Set max volatility based on risk profile
  └─ Allocate min/max weights by asset class
  ↓
Complete ClientOnboardingExample assembled
  ├─ Profile
  ├─ Assets (always same 5)
  ├─ Constraints
  ├─ Correlation matrix (always same)
  └─ Risk-free rate = 0.02
  ↓
Example passed to Optimizer
  ├─ Optimizer receives constraints
  ├─ Optimizer analyzes correlations
  └─ Optimizer finds best weights
  ↓
Optimizer returns OptimizedPortfolio
  ├─ Weight allocation
  ├─ Expected return & volatility
  ├─ Sharpe ratio
  └─ Status & warnings
  ↓
Result attached to example
  └─ example.OptimizedPortfolio = result
  ↓
Complete example returned to API caller
  └─ JSON response with all data
```

---

## File Organization

```
PortfolioOptimization/
│
├── csharp/
│   │
│   ├── PortfolioOptimization.API/
│   │   ├── Controllers/
│   │   │   ├── ExamplesController.cs (MODIFIED)
│   │   │   │   • GenerateClientOnboardingExample() ← NEW
│   │   │   │
│   │   │   └── OptimizationController.cs (unchanged)
│   │   │
│   │   ├── Program.cs (unchanged)
│   │   └── appsettings.json (unchanged)
│   │
│   ├── PortfolioOptimization.Core/
│   │   │
│   │   ├── Models/
│   │   │   ├── ClientProfile.cs ← NEW
│   │   │   ├── ClientOnboardingExample.cs ← NEW
│   │   │   ├── Asset.cs (unchanged)
│   │   │   ├── PortfolioConstraints.cs (unchanged)
│   │   │   └── OptimizedPortfolio.cs (unchanged)
│   │   │
│   │   └── Services/
│   │       ├── ExampleDataGenerator.cs ← NEW
│   │       ├── IPortfolioOptimizer.cs (unchanged)
│   │       ├── NaivePortfolioOptimizer.cs (unchanged)
│   │       └── PythonPortfolioOptimizer.cs (unchanged)
│   │
│   └── PortfolioOptimization.Tests/
│       ├── NaivePortfolioOptimizerTests.cs (all pass)
│       └── PythonPortfolioOptimizerIntegrationTests.cs (all pass)
│
├── CLIENT_ONBOARDING_GUIDE.md ← NEW
├── ARCHITECTURE.md ← NEW
├── QUICK_REFERENCE.md ← NEW
├── IMPLEMENTATION_SUMMARY.md ← NEW
└── VISUAL_GUIDE.md ← NEW (this file)
```

---

This visual guide should help you understand how all components work together!
