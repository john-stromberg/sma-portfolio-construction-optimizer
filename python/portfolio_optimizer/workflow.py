"""
End-to-end portfolio optimization workflow.
Demonstrates: data ingestion → validation → optimization → results output.
"""

import json
from datetime import datetime
from pathlib import Path

from portfolio_optimizer.models import Asset, PortfolioConstraints
from portfolio_optimizer.optimizer import PortfolioOptimizer


def load_assets_from_json(file_path: str) -> list[Asset]:
    """Load assets from JSON file."""
    with open(file_path) as f:
        data = json.load(f)
    return [
        Asset(
            id=a["id"],
            name=a["name"],
            asset_class=a["asset_class"],
            expected_return=a["expected_return"],
            volatility=a["volatility"],
            current_price=a["current_price"],
        )
        for a in data.get("assets", [])
    ]


def load_constraints_from_json(file_path: str) -> PortfolioConstraints:
    """Load constraints from JSON file."""
    with open(file_path) as f:
        data = json.load(f)

    return PortfolioConstraints(
        min_weights=data.get("min_weights", {}),
        max_weights=data.get("max_weights", {}),
        minimum_return=data.get("minimum_return"),
        maximum_volatility=data.get("maximum_volatility"),
        budget_constraint=data.get("budget_constraint", 1.0),
    )


def validate_assets(assets: list[Asset]) -> list[str]:
    """Validate asset data."""
    warnings = []

    if not assets:
        warnings.append("No assets provided.")

    for asset in assets:
        if asset.expected_return < 0:
            warnings.append(f"{asset.name}: Expected return is negative.")
        if asset.volatility < 0:
            warnings.append(f"{asset.name}: Volatility cannot be negative.")
        if asset.current_price <= 0:
            warnings.append(f"{asset.name}: Price must be positive.")

    return warnings


def run_workflow(
    assets_file: str,
    constraints_file: str,
    output_dir: str = "output",
    risk_free_rate: float = 0.02,
) -> dict:
    """
    Run complete portfolio optimization workflow.

    Args:
        assets_file: Path to JSON file with assets.
        constraints_file: Path to JSON file with constraints.
        output_dir: Directory to save results.
        risk_free_rate: Risk-free rate for calculations.

    Returns:
        Dictionary with workflow results.
    """

    # Create output directory
    Path(output_dir).mkdir(exist_ok=True)

    print("[1/5] Loading data...")
    assets = load_assets_from_json(assets_file)
    constraints = load_constraints_from_json(constraints_file)
    print(f"  Loaded {len(assets)} assets")

    print("[2/5] Validating input...")
    validation_warnings = validate_assets(assets)
    if validation_warnings:
        for w in validation_warnings:
            print(f"  WARNING: {w}")
    else:
        print("  All validations passed")

    print("[3/5] Running optimization...")
    optimizer = PortfolioOptimizer(risk_free_rate=risk_free_rate)
    portfolio = optimizer.optimize(assets, constraints)
    print(f"  Portfolio return: {portfolio.expected_return:.2%}")
    print(f"  Portfolio volatility: {portfolio.expected_volatility:.2%}")
    print(f"  Sharpe ratio: {portfolio.sharpe_ratio:.4f}")

    print("[4/5] Generating allocation report...")
    report = {
        "timestamp": datetime.utcnow().isoformat(),
        "optimization_id": portfolio.id,
        "portfolio_metrics": {
            "expected_return": portfolio.expected_return,
            "expected_volatility": portfolio.expected_volatility,
            "sharpe_ratio": portfolio.sharpe_ratio,
            "status": portfolio.status,
        },
        "allocation": portfolio.weights,
        "warnings": validation_warnings + portfolio.warnings,
    }

    # Save results
    output_file = Path(output_dir) / f"optimization_{datetime.now().strftime('%Y%m%d_%H%M%S')}.json"
    with open(output_file, "w") as f:
        json.dump(report, f, indent=2)
    print(f"  Results saved to {output_file}")

    print("[5/5] Workflow complete!")
    print(f"\nAllocation Summary:")
    for asset_id, weight in sorted(portfolio.weights.items(), key=lambda x: x[1], reverse=True):
        print(f"  {asset_id}: {weight:.2%}")

    return report


if __name__ == "__main__":
    # Example usage (requires sample data files)
    import sys

    if len(sys.argv) < 3:
        print("Usage: python workflow.py <assets.json> <constraints.json> [output_dir]")
        print("\nExample:")
        print("  python workflow.py data/sample_assets.json data/sample_constraints.json output/")
        sys.exit(1)

    assets_file = sys.argv[1]
    constraints_file = sys.argv[2]
    output_dir = sys.argv[3] if len(sys.argv) > 3 else "output"

    run_workflow(assets_file, constraints_file, output_dir)
