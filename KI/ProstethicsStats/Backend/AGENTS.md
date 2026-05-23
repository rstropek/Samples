# Prosthetics Statistics

## Project Overview

Python 3.14 application for analyzing prosthetics patient data. Uses Poetry for dependency management, modern Python type checking with mypy, and linting/formatting with Black.

## Project Structure

```
src/                   # Source code directory
data/
  prosthetics_data.csv # Large dataset with patient prosthetics records
tests/                 # Test files
```

## Data Schema

The `prosthetics_data.csv` contains patient prosthetic fitting records with these columns:
- `patient_id`, `age`, `gender`, `amputation_level`, `amputation_side`
- `foot_type`, `knee_type`, `hip_type` (product names like "Ottobock Genium X3", "Ossur Proprio")
- `fitting_date`, `num_visits`, `outcome_score`, `satisfaction_rating`

**Important**: The CSV file is 84MB and excluded from git (in `.gitignore`). It won't be synced with extensions.

## Development Workflow

### Environment Setup
- **Python version**: 3.14 (strict requirement in `pyproject.toml`)
- **Package manager**: Poetry (not pip or conda)
- Virtual environment is in `.venv/` (git-ignored)

### Running Commands
Use **taskipy** shortcuts defined in `pyproject.toml`:
- `poetry run task start` - Run the application (executes `src/main.py`)
- `poetry run task format` - Format code with Black
- `poetry run task format-check` - Check formatting without modifying
- `poetry run task typecheck` - Run mypy type checking

**Never** use bare `python` command - it's not available. Use Poetry commands.

## Code Conventions

### Type Checking (mypy)
- **Strict mode enabled** - all mypy strict checks are enforced
- `ignore_missing_imports = true` - third-party libraries without stubs are allowed
- Always add type hints to functions and variables

### Code Formatting (Black)
- Line length: **150 characters** (wider than default 88)
- Target Python 3.13+ syntax
- String normalization enabled (use double quotes)

## Environment Variables

`.env` file contains sensitive API keys (git-ignored):
- `OPENAI_API_KEY` - OpenAI API key for AI features

**Never commit** `.env` files or expose API keys in code.

## Testing

- Place test files in `tests/` directory
- Follow naming convention: `test_*.py`
- Uses pytest
- Run tests with `poetry run pytest`
