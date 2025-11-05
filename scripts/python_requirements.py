from utils import load_animation
from install_python_modules import try_install_python_module

def validate_python_requirements():
	load_animation("Validating python requirements...")
	try_install_python_module("colorama")


