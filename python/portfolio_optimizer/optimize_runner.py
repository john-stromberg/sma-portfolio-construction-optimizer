#!/usr/bin/env python
"""
Standalone optimization runner for subprocess calls from C#.
Reads JSON request from stdin, runs optimization, outputs JSON result to stdout.
"""

import json
import sys
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from portfolio_optimizer.optimizer import PortfolioOptimizer
from portfolio_optimizer.models import Asset, PortfolioConstraints


def run_optimization():
    """Read request from stdin, run optimization, write result to stdout."""
    try:
        # Read JSON request from stdin
        request_json = sys.stdin.read()
        request = json.loads(request_json)

        # Parse request
        assets_data = request.get("assets", [])
        assets = [
            Asset(
                id=a["id"],
                name=a["name"],
                asset_class=a["asset_class"],
                expected_return=a["expected_return"],
                volatility=a["volatility"],
                current_price=a["current_price"],
            )
            for a in assets_data
        ]

        constraints_data = request.get("constraints", {})
        constraints = PortfolioConstraints(
            min_weights=constraints_data.get("min_weights", {}),
            max_weights=constraints_data.get("max_weights", {}),
            minimum_return=constraints_data.get("minimum_return"),
            maximum_volatility=constraints_data.get("maximum_volatility"),
            budget_constraint=constraints_data.get("budget_constraint", 1.0),
        )

        risk_free_rate = request.get("risk_free_rate", 0.02)

        # Run optimization
        optimizer = PortfolioOptimizer(risk_free_rate=risk_free_rate)
        result = optimizer.optimize(assets, constraints)

        # Output JSON result
        output = {
            "id": result.id,
            "weights": result.weights,
            "expected_return": result.expected_return,
            "expected_volatility": result.expected_volatility,
            "sharpe_ratio": result.sharpe_ratio,
            "status": result.status,
            "warnings": result.warnings,
            "created_at": result.created_at,
        }

        print(json.dumps(output))

    except Exception as e:
        # Output error as JSON
        error_output = {
            "status": "Error",
            "error": str(e),
        }
        print(json.dumps(error_output), file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    run_optimization()
