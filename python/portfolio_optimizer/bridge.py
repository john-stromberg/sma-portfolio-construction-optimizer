"""
Bridge between C# API and Python optimization engine via subprocess.
Serializes requests to JSON and deserializes results.
"""

import json
import subprocess
import sys
from pathlib import Path
from typing import Dict, Any

from .models import OptimizedPortfolio


class PythonOptimizerBridge:
    """
    Calls the Python optimizer from C# via subprocess.
    Handles JSON serialization/deserialization.
    """

    def __init__(self, python_script_path: str = None):
        """
        Initialize the bridge.

        Args:
            python_script_path: Path to the Python optimization script.
                                If None, uses embedded script.
        """
        self.python_script_path = python_script_path or self._get_default_script()

    @staticmethod
    def _get_default_script() -> str:
        """Get the default Python optimization script path."""
        script_dir = Path(__file__).parent
        return str(script_dir / "optimize_runner.py")

    def call_optimizer(self, request_json: str) -> Dict[str, Any]:
        """
        Call the Python optimizer via subprocess.

        Args:
            request_json: JSON string containing optimization request.

        Returns:
            Dictionary with optimization result.

        Raises:
            RuntimeError: If subprocess call fails.
        """
        try:
            result = subprocess.run(
                [sys.executable, self.python_script_path],
                input=request_json,
                capture_output=True,
                text=True,
                timeout=300,  # 5 minute timeout
            )

            if result.returncode != 0:
                raise RuntimeError(
                    f"Python optimizer failed: {result.stderr}"
                )

            return json.loads(result.stdout)
        except json.JSONDecodeError as e:
            raise RuntimeError(f"Failed to parse optimizer output: {e}")
        except subprocess.TimeoutExpired:
            raise RuntimeError("Python optimizer exceeded 5-minute timeout.")
        except Exception as e:
            raise RuntimeError(f"Subprocess error: {e}")
