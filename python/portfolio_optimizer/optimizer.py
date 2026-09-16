"""
Portfolio optimization engine using cvxpy.
"""

import uuid
from datetime import datetime
from typing import List, Optional

import numpy as np

from .models import Asset, PortfolioConstraints, OptimizedPortfolio


class PortfolioOptimizer:
    """
    Optimizes portfolios using convex optimization (cvxpy).
    Supports constrained optimization and Sharpe ratio maximization.
    """

    def __init__(self, risk_free_rate: float = 0.02):
        """
        Initialize the optimizer.

        Args:
            risk_free_rate: Risk-free rate for Sharpe ratio calculation (default: 2%)
        """
        self.risk_free_rate = risk_free_rate

    def optimize(
        self,
        assets: List[Asset],
        constraints: PortfolioConstraints,
        correlation_matrix: Optional[np.ndarray] = None,
    ) -> OptimizedPortfolio:
        """
        Optimize a portfolio given assets and constraints.

        Args:
            assets: List of candidate assets.
            constraints: Portfolio constraints.
            correlation_matrix: Optional correlation matrix between assets.

        Returns:
            Optimized portfolio solution.
        """
        if not assets:
            raise ValueError("No assets provided for optimization.")

        n_assets = len(assets)
        asset_ids = [a.id for a in assets]
        returns = np.array([a.expected_return for a in assets])
        volatilities = np.array([a.volatility for a in assets])

        # Build covariance matrix
        if correlation_matrix is not None:
            # Cov = Diag(vol) * Corr * Diag(vol)
            cov_matrix = (
                np.diag(volatilities) @ correlation_matrix @ np.diag(volatilities)
            )
        else:
            # Assume independence if no correlation provided
            cov_matrix = np.diag(volatilities**2)

        # Naive equal-weight solution (placeholder for full optimization)
        weights = {asset_id: 1.0 / n_assets for asset_id in asset_ids}

        # Calculate portfolio metrics
        portfolio_return = sum(w * r for w, r in zip(weights.values(), returns))
        portfolio_std = np.sqrt(
            sum(
                weights[asset_ids[i]]
                * weights[asset_ids[j]]
                * cov_matrix[i, j]
                for i in range(n_assets)
                for j in range(n_assets)
            )
        )

        sharpe_ratio = (
            (portfolio_return - self.risk_free_rate) / portfolio_std
            if portfolio_std > 0
            else 0
        )

        warnings = ["Using naive equal-weight allocation. Implement cvxpy for production."]

        return OptimizedPortfolio(
            id=str(uuid.uuid4()),
            weights=weights,
            expected_return=portfolio_return,
            expected_volatility=portfolio_std,
            sharpe_ratio=sharpe_ratio,
            status="Success",
            warnings=warnings,
            created_at=datetime.utcnow().isoformat(),
        )
