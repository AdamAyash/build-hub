from utils import prompt_with_default

def try_install_python_module(module_name):
    try:
        import importlib
        importlib.import_module(module_name)
        print(f"{module_name} is already installed.")
    except ImportError:
        import subprocess
        print(f"{module_name} is not installed.")
        confirmation = prompt_with_default("Do you want to install it? (y/n)", "y")
        if(confirmation.lower() == "n"):
            exit()
                  
        subprocess.run(["pip", "install", module_name])
        print(f"{module_name} has been installed.")