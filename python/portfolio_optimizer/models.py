"""
Data models for portfolio optimization.
"""

from dataclasses import dataclass, field
from typing import Dict, Optional


@dataclass
class Asset:
    """Represents a single asset in the portfolio."""
    id: str
    name: str
    asset_class: str
    expected_return: float
    volatility: float
    current_price: float


@dataclass
class PortfolioConstraints:
    """Represents portfolio constraints for optimization."""
    min_weights: Dict[str, float] = field(default_factory=dict)
    max_weights: Dict[str, float] = field(default_factory=dict)
    minimum_return: Optional[float] = None
    maximum_volatility: Optional[float] = None
    budget_constraint: float = 1.0


@dataclass
class OptimizedPortfolio:
    """Represents an optimized portfolio solution."""
    id: str
    weights: Dict[str, float]
    expected_return: float
    expected_volatility: float
    sharpe_ratio: float
    status: str
    warnings: list = field(default_factory=list)
    created_at: Optional[str] = None
