"""
Portfolio Optimization Engine

A hybrid Python/C# module for multi-asset portfolio construction and optimization.
Supports constrained optimization with cvxpy and Sharpe ratio maximization.
"""

__version__ = "0.1.0"
__author__ = "Portfolio Analytics"

from .optimizer import PortfolioOptimizer
from .models import (
    Asset,
    PortfolioConstraints,
    OptimizedPortfolio,
)

__all__ = [
    "PortfolioOptimizer",
    "Asset",
    "PortfolioConstraints",
    "OptimizedPortfolio",
]
