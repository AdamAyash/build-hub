def try_install_python_module(module_name):
    try:
        import importlib
        importlib.import_module(module_name)
        print(f"{module_name} is already installed.")
    except ImportError:
        import subprocess
        print(f"{module_name} is not installed. Installing...")
        subprocess.run(["pip", "install", module_name])
        print(f"{module_name} has been installed.")