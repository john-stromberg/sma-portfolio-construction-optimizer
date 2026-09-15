from src.core import run_placeholder_analysis

def test_placeholder():
    out = run_placeholder_analysis()
    assert out['status'] == 'ok'

