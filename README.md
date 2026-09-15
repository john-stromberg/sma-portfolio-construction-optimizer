# sma-portfolio-construction-optimizer

Track: **SMA Portfolio Management**

## Project purpose

This project is designed to be run independently as a workflow module for multi-asset SMA portfolio management across **equities, bonds, alternatives, ETFs, and mutual funds**.

## Local development workflow

1. Create environment and install dependencies
   - `python -m venv .venv`
   - `.venv\\Scripts\\Activate.ps1`
   - `pip install -e .`
2. Run tests
   - `pytest -q`
3. Run pipeline task (placeholder)
   - `python pipelines/job.py`
4. Run API service (if used)
   - `uvicorn api.main:app --reload`
5. Run dashboard (if used)
   - `streamlit run apps/dashboard.py`

## Repository structure

- `src/` analytics and business logic
- `api/` service endpoints for integration
- `apps/` visualization and UI layer
- `pipelines/` scheduled/batch workflow tasks
- `sql/` schema and query assets
- `tests/` automated validation
- `reports/` presentation and decision memo outputs
- `notebooks/` exploratory analysis and prototypes
- `data/` local data artifacts

## Expected outputs

- Visual diagnostics for portfolio/risk decisions
- Reproducible table exports for downstream workflow use
- API/app components for professional integration

## Decision memo template

- **Question:** What decision is this run evaluating?
- **Evidence:** Which charts/tables support the conclusion?
- **Interpretation:** What changed and why does it matter?
- **Action:** Rebalance, hedge, monitor, or defer.

## Notes

- Use this repo as an independent module; dependencies on other repositories should be explicit.
- Keep assumptions, constraints, and known limitations documented with each change.

