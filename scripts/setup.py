import subprocess
import sys
import time
import logging
from typing import Optional, List
from enum import Enum

from dataclasses import dataclass
from dot_net_requirement import validate_dotnet_version
from input import prompt_with_default
from utils import load_animation, section, success, warning, error, step, divider

class SetupStatus(Enum):
    PENDING = "pending"
    RUNNING = "running"
    SUCCESS = "success"
    FAILED = "failed"
    SKIPPED = "skipped"

@dataclass
class SetupStep:
    name: str
    description: str
    function: callable
    critical: bool = True
    status: SetupStatus = SetupStatus.PENDING
    error: Optional[str] = None
    duration: float = 0.0
    required: bool = False


class BuildHubSetup:
    def __init__(self, verbose: bool = False):
        self.verbose = verbose
        self.steps: List[SetupStep] = []
        self.setup_logging()
        self._register_steps()
        
    def setup_logging(self):
        log_file = 'setup.log'
        
        file_handler = logging.FileHandler(log_file, mode='a')
        file_handler.setFormatter(
            logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
        )
        
        logging.basicConfig(
            level=logging.DEBUG if self.verbose else logging.INFO,
            handlers=[file_handler],
            force=True
        )
        
        self.logger = logging.getLogger(__name__)
        self.logger.info("\n" + "="*60)
        self.logger.info(f"Build-Hub Setup Session Started - {time.strftime('%Y-%m-%d %H:%M:%S')}")
        self.logger.info("="*60)
    
    def _register_steps(self):
        self.steps = [
            SetupStep(
                name="dotnet_validation",
                description="Validating .NET installation",
                function=self._validate_dotnet,
                critical=True,
                required = True
            ),
             SetupStep(
                name="install_or_update_sql_package",
                description="Create or update databases",
                function=self._create_or_update_databases,
                critical=True
            ),
        ]
    
    def _validate_dotnet(self) -> bool:
        try:
            self.logger.info("Validating .NET version")
            result = validate_dotnet_version()
            
            if result:
                self.logger.info(".NET version validated")
                return True
            else:
                raise Exception(".NET version does not meet requirements")
                
        except Exception as e:
            self.logger.error(f".NET validation failed: {str(e)}")
            raise
            
        except KeyboardInterrupt:
            self.logger.warning("Setup interrupted by user")
            raise
        except Exception as e:
            self.logger.error(f"Failed to setup database: {str(e)}")
            raise
        
    def _create_or_update_databases(self) -> bool:
            load_animation("Creating/Updating databases...")
            
            
            return True

    def _run_step(self, step: SetupStep) -> bool:
        
        step.status = SetupStatus.RUNNING
        start_time = time.time()
        
        if not step.required: 
            confirmation = prompt_with_default( "Do you wish to proceed with this step? (Y/n)", "y")
            if confirmation is not "Y" or not "y":
                step.status = SetupStatus.SKIPPED
                return True;
    
        try:
            self.logger.debug(f"Starting step: {step.name}")
            result = step.function()
            
            step.duration = time.time() - start_time
            step.status = SetupStatus.SUCCESS
            
            return result
            
        except KeyboardInterrupt:
            step.duration = time.time() - start_time
            step.status = SetupStatus.FAILED
            step.error = "Interrupted by user"
            raise
            
        except Exception as e:
            step.duration = time.time() - start_time
            step.status = SetupStatus.FAILED
            step.error = str(e)
            self.logger.error(f"Step '{step.name}' failed: {str(e)}")
            
            if step.critical:
                return False
            else:
                warning(f"Non-critical step '{step.name}' failed, continuing...")
                step.status = SetupStatus.SKIPPED
                return True
    
    def _print_summary(self):
        total_time = sum(step.duration for step in self.steps)
        successful = sum(1 for step in self.steps if step.status == SetupStatus.SUCCESS)
        failed = sum(1 for step in self.steps if step.status == SetupStatus.FAILED)
        skipped = sum(1 for step in self.steps if step.status == SetupStatus.SKIPPED)
        
        print("\n" + "=" * 60)
        print("📊 SETUP SUMMARY")
        print("=" * 60)
        print(f"Total Steps:  {len(self.steps)}")
        print(f"✅ Successful: {successful}")
        if failed > 0:
            print(f"❌ Failed:     {failed}")
        print(f"⏱️  Total Time: {total_time:.2f}s")
        print("=" * 60)
        
        for i, step in enumerate(self.steps, 1):
            status_symbol = {
                SetupStatus.SUCCESS: "✅",
                SetupStatus.FAILED: "❌",
                SetupStatus.SKIPPED: "⊘"
            }.get(step.status, "?")
            
            print(f"{status_symbol} [{i}] {step.description} ({step.duration:.2f}s)")
            
            if step.error and self.verbose:
                print(f"    └─ Error: {step.error}")
        
        print("=" * 60)
    
    def run(self) -> bool:
        section("Build-Hub Setup Starting")
        
        setup_successful = True
        interrupted = False 
        
        try:
            for i, step_obj in enumerate(self.steps, 1):
                print()
                step(i, len(self.steps), step_obj.description)
                divider()
                
                if not self._run_step(step_obj):
                    if step_obj.critical:
                        setup_successful = False
                        error(f"Critical step failed: {step_obj.description}")
                        break
                else:
                    success(f"Completed in {step_obj.duration:.2f}s")
                    
        except KeyboardInterrupt:
            print("\n")
            warning("Setup interrupted by user (Ctrl+C)")
            setup_successful = False
            interrupted = True
        
        self._print_summary()
        
        print()
        if setup_successful:
            success("Build-Hub setup completed successfully!")
            print("ℹ️  You may need to restart your terminal for environment changes to take effect")
            self.logger.info("Setup completed successfully")
        else:
            error("Build-Hub setup failed!")
            print("📝 Check buildhub_setup.log for detailed error information")
            self.logger.error("Setup failed")
        
        self.logger.info("="*60 + "\n")
        
        return setup_successful


def main():
    import argparse
    
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description="Build-Hub Setup Script")
    parser.add_argument('-v', '--verbose', action='store_true', help='Enable verbose output')
    args = parser.parse_args()
    
    try:
        setup = BuildHubSetup(verbose=args.verbose)
        success = setup.run()
        sys.exit(0 if success else 1)
    except Exception as e:
        error(f"Unexpected error: {str(e)}")
        sys.exit(1)

if __name__ == "__main__":
    main()
