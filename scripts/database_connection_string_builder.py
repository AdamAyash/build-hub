import getpass
import os
import subprocess
import logging
from typing import Optional
from input import prompt_with_default


class DatabaseConnectionStringBuilder:
    """
    Enhanced database connection string builder with validation,
    security features, and better error handling.
    """
    
    connectionStringEnvironmentVariablePrefix = "ConnectionStrings__"
    connectionString = ""
    
    def __init__(self, databaseKey: str, databaseName: str):
        """
        Initialize the connection string builder.
        
        Args:
            databaseKey: Key for the environment variable
            databaseName: Human-readable database name
        """
        self.databaseKey = databaseKey
        self.databaseName = databaseName
        self.logger = logging.getLogger(__name__)
        
    def build(self) -> str:
        """
        Build the connection string through user input.
        
        Returns:
            The built connection string
            
        Raises:
            ValueError: If validation fails
            KeyboardInterrupt: If user cancels
        """
        print(f"🔧 Building connection string for {self.databaseName}...")
        print("─" * 60)
        
        try:
            # Get connection parameters with validation
            dataSource = self._get_data_source()
            initialCatalog = self._get_catalog()
            userID = self._get_user_id()
            password = self._get_password()
            
            # Offer Windows Authentication as alternative
            use_windows_auth = self._prompt_windows_auth()
            
            if use_windows_auth:
                self.connectionString = (
                    f"Data Source={dataSource};"
                    f"Initial Catalog={initialCatalog};"
                    f"Integrated Security=True;"
                )
                print("✓ Using Windows Authentication")
            else:
                self.connectionString = (
                    f"Data Source={dataSource};"
                    f"Initial Catalog={initialCatalog};"
                    f"User ID={userID};"
                    f"Password={password};"
                )
            
            # Add optional parameters
            self._add_optional_parameters()
            
            print("\n✅ Connection string built successfully")
            self.logger.info(f"Connection string built for {self.databaseName}")
            
            # Save to environment variable
            self._save_connection_string()
            
            return self.connectionString
            
        except KeyboardInterrupt:
            print("\n\n⚠️  Connection string setup cancelled by user")
            raise
        except Exception as e:
            self.logger.error(f"Failed to build connection string: {str(e)}")
            raise
    
    def _get_data_source(self) -> str:
        """Get and validate data source."""
        while True:
            dataSource = input("📍 Data source (e.g., localhost, server.domain.com): ").strip()
            
            if not dataSource:
                print("❌ Data source cannot be empty")
                continue
                
            # Check if includes instance name
            if "\\" not in dataSource and "," not in dataSource:
                has_instance = prompt_with_default(
                    "   Include SQL Server instance? (y/N)", "n"
                )
                if has_instance.lower() == 'y':
                    instance = input("   Instance name: ").strip()
                    if instance:
                        dataSource = f"{dataSource}\\{instance}"
            
            return dataSource
    
    def _get_catalog(self) -> str:
        """Get and validate catalog/database name."""
        while True:
            catalog = input("📊 Database catalog/name: ").strip()
            
            if not catalog:
                print("❌ Catalog cannot be empty")
                continue
                
            return catalog
    
    def _get_user_id(self) -> str:
        """Get and validate user ID."""
        while True:
            userId = input("👤 User ID: ").strip()
            
            if not userId:
                print("❌ User ID cannot be empty")
                continue
                
            return userId
    
    def _get_password(self) -> str:
        """Get and validate password."""
        while True:
            password = getpass.getpass("🔑 Password: ")
            
            if not password:
                print("❌ Password cannot be empty")
                continue
            
            password_confirm = getpass.getpass("🔑 Confirm password: ")
            if password != password_confirm:
                print("❌ Passwords don't match, please try again")
                continue
            
            return password
    
    def _prompt_windows_auth(self) -> bool:
        """Ask if user wants to use Windows Authentication."""
        if os.name != 'nt':
            return False
            
        response = prompt_with_default(
            "\n🪟 Use Windows Authentication instead? (y/N)", "n"
        )
        return response.lower() == 'y'
    
    def _add_optional_parameters(self):
        """Add optional connection string parameters."""
        add_params = prompt_with_default(
            "\n⚙️  Add optional parameters? (y/N)", "n"
        )
        
        if add_params.lower() != 'y':
            return
        
        print("\nOptional parameters:")
        
        # Connection timeout
        timeout = prompt_with_default(
            "  Connection Timeout in seconds (default: 30)", "30"
        )
        if timeout and timeout.isdigit():
            self.connectionString += f"Connection Timeout={timeout};"
        
        # Encrypt connection
        encrypt = prompt_with_default(
            "  Encrypt connection? (Y/n)", "y"
        )
        if encrypt.lower() != 'n':
            self.connectionString += "Encrypt=True;"
            
            # Trust server certificate
            trust_cert = prompt_with_default(
                "  Trust Server Certificate? (y/N)", "n"
            )
            if trust_cert.lower() == 'y':
                self.connectionString += "TrustServerCertificate=True;"
        
        # MultipleActiveResultSets
        mars = prompt_with_default(
            "  Enable Multiple Active Result Sets (MARS)? (y/N)", "n"
        )
        if mars.lower() == 'y':
            self.connectionString += "MultipleActiveResultSets=True;"
    
    def _save_connection_string(self):
        """Save connection string to environment variable with enhanced prompts."""
        print("\n" + "─" * 60)
        print("💾 SAVE TO ENVIRONMENT VARIABLE")
        print("─" * 60)
        
        confirmation = prompt_with_default(
            "Save the connection string to an environment variable? (Y/n)", "y"
        )
        
        if confirmation.lower() == 'n':
            print("⚠️  Connection string not saved. Setup incomplete.")
            self.logger.warning("User declined to save connection string")
            
            show_string = prompt_with_default(
                "Display connection string for manual setup? (y/N)", "n"
            )
            
            if show_string.lower() == 'y':
                print(f"\nConnection String:\n{self.connectionString}\n")
                print(f"Environment Variable Name:")
                print(f"{self.connectionStringEnvironmentVariablePrefix}{self.databaseKey}\n")
            
            raise Exception("Setup cancelled: Connection string not saved")
        
        connectionStringEnvironmentVariableName = (
            self.connectionStringEnvironmentVariablePrefix + self.databaseKey
        )
        
        try:
            if os.name == 'nt':  # Windows
                # Use setx to persist the environment variable
                result = subprocess.run(
                    ['setx', connectionStringEnvironmentVariableName, self.connectionString],
                    capture_output=True,
                    text=True,
                    check=True
                )
                
                print(f"✅ Saved to: {connectionStringEnvironmentVariableName}")
                print("ℹ️  Restart your terminal for the change to take effect")
                
                self.logger.info(
                    f"Connection string saved to environment variable: "
                    f"{connectionStringEnvironmentVariableName}"
                )
                
            else:  # Unix-like systems
                print(f"\n⚠️  Manual setup required for Unix-like systems")
                print(f"Add this to your ~/.bashrc or ~/.zshrc:")
                print(f'export {connectionStringEnvironmentVariableName}="{self.connectionString}"')
                
                self.logger.warning("Unix system detected, provided manual setup instructions")
                
        except subprocess.CalledProcessError as e:
            error_msg = f"Failed to set environment variable: {e.stderr}"
            print(f"❌ {error_msg}")
            self.logger.error(error_msg)
            
            print("\n📋 Manual setup required:")
            print(f"Variable name: {connectionStringEnvironmentVariableName}")
            print(f"Value: {self.connectionString}")
            
            raise Exception(error_msg)
        except Exception as e:
            self.logger.error(f"Unexpected error saving connection string: {str(e)}")
            raise
    
    def test_connection(self) -> bool:
        """
        Test the database connection (optional method for future use).
        
        Returns:
            True if connection successful, False otherwise
        """
        # This could be implemented with pyodbc or other database drivers
        # Left as a placeholder for future enhancement
        self.logger.info("Connection test not implemented yet")
        return True