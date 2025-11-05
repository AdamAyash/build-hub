import sys
import time
from colorama import Fore, Style, init

init(autoreset=True)

def prompt_with_default(prompt, default):
    value = input(f"{prompt}: ").strip()
    return value if value else default

def load_animation(message, duration=2):
    print(message, end="")
    for _ in range(duration * 4):
        for ch in "|/-\\":
            sys.stdout.write(f"\r{message} {ch}")
            sys.stdout.flush()
            time.sleep(0.1)
    print("\r" + message + " ✅")
    

def section(title):
    print("\n" +Fore.CYAN + "=" * len(title))
    print(Fore.CYAN + f"🏗️  {title}")
    print(Fore.CYAN + "=" * len(title) + "\n")

def success(msg):
    print(Fore.GREEN + f"✅ {msg}")

def warning(msg):
    print(Fore.YELLOW + f"⚠️  {msg}")
    
def confirmation(message):
    confirm = input(f"{message} (y/n): ")
    if confirm.lower() == "y":
        return True
    else:
        return False
    
