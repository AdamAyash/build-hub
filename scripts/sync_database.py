import subprocess

def validate_sqlpackage():
    try:
        result = subprocess.call("sqlpackage", "/version")

    except subprocess.CalledProcessError:
        return "sqlpackage command failed."
    except FileNotFoundError:
        return "sqlpackage is not installed"