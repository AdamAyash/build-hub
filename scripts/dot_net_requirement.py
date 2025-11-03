import subprocess
from utils import load_animation

def __get_dotnet_version():

    load_animation("Validating .NET version...")
    
    try:
        result = subprocess.run(['dotnet', '--version'], 
                              capture_output=True, 
                              text=True, 
                              check=True)
        return result.stdout.strip()
    except subprocess.CalledProcessError:
        return "dotnet command failed"
    except FileNotFoundError:
        return ".NET is not installed or not in PATH"

def validate_dotnet_version():
    version = __get_dotnet_version()
    print(f"Current .NET version: {version}")
    if float(version[:3]) >= 4.8:
       return True
    else:
         print("Build-Hub requires .NET 4.8 or higher.")
         return False
