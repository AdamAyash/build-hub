#!/usr/bin/env python3
"""
Python Requirements Validator
Validates and installs required Python packages with enhanced error handling and user experience.
"""

import sys
import subprocess
import importlib
import logging
from typing import List, Tuple, Optional, Dict
from dataclasses import dataclass
from enum import Enum


class InstallStatus(Enum):
    """Status of package installation."""
    ALREADY_INSTALLED = "already_installed"
    INSTALLED = "installed"
    SKIPPED = "skipped"
    FAILED = "failed"


@dataclass
class PackageRequirement:
    """Represents a Python package requirement."""
    name: str
    import_name: Optional[str] = None  # If different from package name
    min_version: Optional[str] = None
    required: bool = True
    description: Optional[str] = None
    
    def __post_init__(self):
        if self.import_name is None:
            self.import_name = self.name


class PythonRequirementsValidator:
    """
    Enhanced validator for Python package requirements.
    Handles installation, version checking, and provides detailed feedback.
    """
    
    def __init__(self, requirements: Optional[List[PackageRequirement]] = None, 
                 auto_install: bool = False,
                 verbose: bool = False):
        """
        Initialize the validator.
        
        Args:
            requirements: List of package requirements
            auto_install: Skip confirmation prompts
            verbose: Enable verbose output
        """
        self.requirements = requirements or self._default_requirements()
        self.auto_install = auto_install
        self.verbose = verbose
        self.results: Dict[str, InstallStatus] = {}
        self.logger = self._setup_logging()
    
    def _setup_logging(self) -> logging.Logger:
        """Setup logging configuration."""
        logger = logging.getLogger(__name__)
        
        if not logger.handlers:
            handler = logging.StreamHandler()
            formatter = logging.Formatter('%(levelname)s - %(message)s')
            handler.setFormatter(formatter)
            logger.addHandler(handler)
            logger.setLevel(logging.DEBUG if self.verbose else logging.INFO)
        
        return logger
    
    def _default_requirements(self) -> List[PackageRequirement]:
        """Return default package requirements for Build-Hub."""
        return [
            PackageRequirement(
                name="colorama",
                description="Terminal colors and formatting",
                required=True
            ),
            PackageRequirement(
                name="rich",
                description="Enhanced terminal output (optional)",
                required=False
            ),
        ]
    
    def _print_header(self):
        """Print validation header."""
        print("\n" + "=" * 60)
        print("🐍 PYTHON REQUIREMENTS VALIDATION")
        print("=" * 60)
        print("Checking required Python packages...\n")
    
    def _print_summary(self):
        """Print validation summary."""
        already_installed = sum(1 for s in self.results.values() if s == InstallStatus.ALREADY_INSTALLED)
        installed = sum(1 for s in self.results.values() if s == InstallStatus.INSTALLED)
        skipped = sum(1 for s in self.results.values() if s == InstallStatus.SKIPPED)
        failed = sum(1 for s in self.results.values() if s == InstallStatus.FAILED)
        
        print("\n" + "=" * 60)
        print("📊 VALIDATION SUMMARY")
        print("=" * 60)
        print(f"Total packages:     {len(self.requirements)}")
        print(f"✅ Already installed: {already_installed}")
        if installed > 0:
            print(f"📦 Newly installed:   {installed}")
        if skipped > 0:
            print(f"⊘  Skipped:          {skipped}")
        if failed > 0:
            print(f"❌ Failed:           {failed}")
        print("=" * 60)
    
    def _check_python_version(self) -> bool:
        """Check if Python version meets requirements."""
        min_version = (3, 7)
        current_version = sys.version_info[:2]
        
        self.logger.debug(f"Python version: {sys.version}")
        
        if current_version < min_version:
            print(f"❌ Python {min_version[0]}.{min_version[1]}+ required, but {current_version[0]}.{current_version[1]} found")
            return False
        
        if self.verbose:
            print(f"✓ Python version: {current_version[0]}.{current_version[1]}")
        
        return True
    
    def _check_pip_available(self) -> bool:
        """Check if pip is available."""
        try:
            result = subprocess.run(
                [sys.executable, "-m", "pip", "--version"],
                capture_output=True,
                text=True,
                timeout=10,
                check=True
            )
            
            if self.verbose:
                print(f"✓ pip available: {result.stdout.strip()}")
            
            return True
            
        except subprocess.CalledProcessError:
            print("❌ pip is not available or not working correctly")
            return False
        except subprocess.TimeoutExpired:
            print("❌ pip check timed out")
            return False
        except Exception as e:
            print(f"❌ Error checking pip: {str(e)}")
            return False
    
    def _is_package_installed(self, package: PackageRequirement) -> Tuple[bool, Optional[str]]:
        """
        Check if a package is installed.
        
        Args:
            package: Package requirement to check
            
        Returns:
            Tuple of (is_installed, version)
        """
        try:
            module = importlib.import_module(package.import_name)
            
            # Try to get version
            version = None
            for attr in ['__version__', 'version', 'VERSION']:
                if hasattr(module, attr):
                    version = getattr(module, attr)
                    if callable(version):
                        version = version()
                    break
            
            return True, version
            
        except ImportError:
            return False, None
        except Exception as e:
            self.logger.debug(f"Error checking {package.name}: {str(e)}")
            return False, None
    
    def _install_package(self, package: PackageRequirement) -> bool:
        """
        Install a package using pip.
        
        Args:
            package: Package to install
            
        Returns:
            True if installation successful, False otherwise
        """
        package_spec = package.name
        if package.min_version:
            package_spec = f"{package.name}>={package.min_version}"
        
        print(f"📦 Installing {package.name}...", end="", flush=True)
        
        try:
            result = subprocess.run(
                [sys.executable, "-m", "pip", "install", package_spec],
                capture_output=True,
                text=True,
                timeout=120,  # 2 minutes timeout
                check=True
            )
            
            if self.verbose:
                print(f"\n{result.stdout}")
            else:
                print(" ✅")
            
            self.logger.info(f"Successfully installed {package.name}")
            return True
            
        except subprocess.CalledProcessError as e:
            print(" ❌")
            print(f"   Error: {e.stderr.strip() if e.stderr else 'Installation failed'}")
            self.logger.error(f"Failed to install {package.name}: {e.stderr}")
            return False
            
        except subprocess.TimeoutExpired:
            print(" ❌")
            print(f"   Error: Installation timed out (>120s)")
            self.logger.error(f"Installation of {package.name} timed out")
            return False
            
        except Exception as e:
            print(" ❌")
            print(f"   Error: {str(e)}")
            self.logger.error(f"Unexpected error installing {package.name}: {str(e)}")
            return False
    
    def _prompt_install(self, package: PackageRequirement) -> bool:
        """
        Prompt user to install a package.
        
        Args:
            package: Package to install
            
        Returns:
            True if user wants to install, False otherwise
        """
        if self.auto_install:
            return True
        
        # Simple prompt without external dependencies
        suffix = " (Y/n)" if package.required else " (y/N)"
        default = "y" if package.required else "n"
        
        while True:
            response = input(f"   Install {package.name}?{suffix}: ").strip().lower()
            
            if not response:
                response = default
            
            if response in ['y', 'yes']:
                return True
            elif response in ['n', 'no']:
                return False
            else:
                print("   Please answer 'y' or 'n'")
    
    def _validate_package(self, package: PackageRequirement) -> InstallStatus:
        """
        Validate a single package requirement.
        
        Args:
            package: Package to validate
            
        Returns:
            InstallStatus enum value
        """
        # Check if already installed
        is_installed, version = self._is_package_installed(package)
        
        if is_installed:
            version_str = f" (v{version})" if version else ""
            status_marker = "✅" if package.required else "✓"
            print(f"{status_marker} {package.name}{version_str} - already installed")
            
            if package.description and self.verbose:
                print(f"   {package.description}")
            
            return InstallStatus.ALREADY_INSTALLED
        
        # Package not installed
        required_marker = "⚠️  Required" if package.required else "ℹ️  Optional"
        print(f"\n{required_marker}: {package.name}")
        
        if package.description:
            print(f"   Description: {package.description}")
        
        # Ask if user wants to install
        if not self._prompt_install(package):
            if package.required:
                print(f"   ⚠️  Skipping required package - setup may fail")
            return InstallStatus.SKIPPED
        
        # Install the package
        if self._install_package(package):
            # Verify installation
            is_installed, version = self._is_package_installed(package)
            if is_installed:
                version_str = f" (v{version})" if version else ""
                print(f"   ✅ {package.name}{version_str} installed successfully")
                return InstallStatus.INSTALLED
            else:
                print(f"   ⚠️  Package installed but could not be imported")
                return InstallStatus.FAILED
        else:
            return InstallStatus.FAILED
    
    def validate_all(self) -> bool:
        """
        Validate all requirements.
        
        Returns:
            True if all required packages are available, False otherwise
        """
        self._print_header()
        
        # Pre-flight checks
        if not self._check_python_version():
            print("\n❌ Python version requirement not met")
            return False
        
        if not self._check_pip_available():
            print("\n❌ pip is not available")
            print("   Install pip: https://pip.pypa.io/en/stable/installation/")
            return False
        
        print()
        
        # Validate each package
        all_required_met = True
        
        for package in self.requirements:
            status = self._validate_package(package)
            self.results[package.name] = status
            
            if package.required and status in [InstallStatus.SKIPPED, InstallStatus.FAILED]:
                all_required_met = False
        
        # Print summary
        self._print_summary()
        
        if not all_required_met:
            print("\n❌ Some required packages are missing")
            print("   Run this script again to install them")
            return False
        
        print("\n✅ All required packages are available")
        return all_required_met
    
    def validate_and_exit(self):
        """Validate requirements and exit if any required packages are missing."""
        if not self.validate_all():
            sys.exit(1)


def validate_python_requirements(
    requirements: Optional[List[PackageRequirement]] = None,
    auto_install: bool = False,
    verbose: bool = False
) -> bool:
    """
    Convenience function to validate Python requirements.
    
    Args:
        requirements: List of package requirements (uses defaults if None)
        auto_install: Skip confirmation prompts
        verbose: Enable verbose output
        
    Returns:
        True if all required packages are available, False otherwise
    """
    validator = PythonRequirementsValidator(
        requirements=requirements,
        auto_install=auto_install,
        verbose=verbose
    )
    return validator.validate_all()


def main():
    """Main entry point when run as a script."""
    import argparse
    
    parser = argparse.ArgumentParser(
        description="Validate and install Python package requirements",
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    
    parser.add_argument(
        '-y', '--yes',
        action='store_true',
        help='Automatically install all packages without prompting'
    )
    
    parser.add_argument(
        '-v', '--verbose',
        action='store_true',
        help='Enable verbose output'
    )
    
    parser.add_argument(
        '--package',
        action='append',
        dest='packages',
        help='Add additional package to check (can be used multiple times)'
    )
    
    args = parser.parse_args()
    
    requirements = [
        PackageRequirement(name="colorama", description="Terminal colors", required=True),
        PackageRequirement(name="rich", description="Enhanced output", required=True),
    ]
    
    if args.packages:
        for pkg in args.packages:
            requirements.append(PackageRequirement(name=pkg, required=False))
    
    validator = PythonRequirementsValidator(
        requirements=requirements,
        auto_install=args.yes,
        verbose=args.verbose
    )
    
    success = validator.validate_all()
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()