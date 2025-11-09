@echo off
echo ============================================================
echo Build-Hub Setup Launcher
echo ============================================================
echo.

echo [1/2] Validating Python requirements...
python validate_python_requirements.py
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to validate Python requirements
    echo Please fix the errors above and try again.
    pause
    exit /b 1
)

echo.
echo [2/2] Starting Build-Hub setup...
echo.
python setup.py

if %errorlevel% equ 0 (
    echo.
    echo ============================================================
    echo Setup completed successfully!
    echo ============================================================
) else (
    echo.
    echo ============================================================
    echo Setup failed - check buildhub_setup.log for details
    echo ============================================================
)

echo.
pause