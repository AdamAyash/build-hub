import os
from dot_net_requirement import validate_dotnet_version
from database_connection_string_builder import DatabaseConnectionStringBuilder
from python_requirements import validate_python_requirements
from utils import section, success, warning, prompt_with_default

validate_python_requirements()
validate_dotnet_version()

section("Build-Hub Setup Starting...")
DatabaseConnectionStringBuilder("BuildHub", "UsersDatabase").build()
success("Build-Hub setup completed successfully!")
