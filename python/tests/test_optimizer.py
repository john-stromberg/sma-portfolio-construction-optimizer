"""
Unit tests for portfolio optimization engine.
"""

import pytest
import numpy as np

from portfolio_optimizer.models import Asset, PortfolioConstraints
from portfolio_optimizer.optimizer import PortfolioOptimizer


@pytest.fixture
def optimizer():
    """Create an optimizer instance."""
    return PortfolioOptimizer(risk_free_rate=0.02)


@pytest.fixture
def sample_assets():
    """Create sample assets for testing."""
    return [
        Asset(
            id="STOCK1",
            name="Stock 1",
            asset_class="Equity",
            expected_return=0.10,
            volatility=0.15,
            current_price=100.0,
        ),
        Asset(
            id="STOCK2",
            name="Stock 2",
            asset_class="Equity",
            expected_return=0.12,
            volatility=0.18,
            current_price=50.0,
        ),
        Asset(
            id="BOND1",
            name="Bond 1",
            asset_class="Fixed Income",
            expected_return=0.04,
            volatility=0.05,
            current_price=1000.0,
        ),
    ]


def test_optimizer_with_valid_assets(optimizer, sample_assets):
    """Test optimization with valid assets."""
    constraints = PortfolioConstraints()
    result = optimizer.optimize(sample_assets, constraints)

    assert result.id is not None
    assert len(result.weights) == 3
    assert abs(sum(result.weights.values()) - 1.0) < 1e-6
    assert result.status == "Success"
    assert result.expected_return > 0
    assert result.expected_volatility > 0
    assert result.sharpe_ratio is not None


def test_optimizer_equal_weight_allocation(optimizer, sample_assets):
    """Test that naive optimizer returns equal weights."""
    constraints = PortfolioConstraints()
    result = optimizer.optimize(sample_assets, constraints)

    for weight in result.weights.values():
        assert abs(weight - 1.0 / 3.0) < 1e-6


def test_optimizer_with_no_assets(optimizer):
    """Test optimization with no assets raises error."""
    constraints = PortfolioConstraints()

    with pytest.raises(ValueError, match="No assets"):
        optimizer.optimize([], constraints)


def test_optimizer_sharpe_ratio_calculation(optimizer, sample_assets):
    """Test Sharpe ratio calculation."""
    constraints = PortfolioConstraints()
    result = optimizer.optimize(sample_assets, constraints)

    # For equal-weight portfolio
    avg_return = np.mean([a.expected_return for a in sample_assets])
    expected_sharpe = (avg_return - 0.02) / result.expected_volatility

    assert abs(result.sharpe_ratio - expected_sharpe) < 0.01


def test_optimizer_with_correlation_matrix(optimizer, sample_assets):
    """Test optimization with correlation matrix."""
    # Simple correlation matrix (identity = independence)
    corr_matrix = np.eye(3)

    constraints = PortfolioConstraints()
    result = optimizer.optimize(sample_assets, constraints, corr_matrix)

    assert result.id is not None
    assert len(result.weights) == 3
    assert result.status == "Success"


def test_optimizer_includes_warnings(optimizer, sample_assets):
    """Test that optimizer includes implementation warnings."""
    constraints = PortfolioConstraints()
    result = optimizer.optimize(sample_assets, constraints)

    assert len(result.warnings) > 0
    assert any("naive" in w.lower() for w in result.warnings)
