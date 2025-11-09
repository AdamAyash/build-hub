import sys
import time
import shutil
from typing import Optional, Callable
from colorama import Fore, Style, init

init(autoreset=True)


def load_animation(message: str, duration: int = 2) -> None:
    """
    Display a loading animation with a message.
    
    Args:
        message: Message to display during loading
        duration: Duration in seconds
    """
    print(message, end="")
    sys.stdout.flush()
    
    spinner_chars = "|/-\\"
    iterations = duration * 4
    
    for i in range(iterations):
        ch = spinner_chars[i % len(spinner_chars)]
        sys.stdout.write(f"\r{message} {ch}")
        sys.stdout.flush()
        time.sleep(0.25)
    
    print("\r" + message + " ✅" + " " * 10)


def progress_bar(
    current: int,
    total: int,
    prefix: str = "",
    suffix: str = "",
    length: int = 50,
    fill: str = "█"
) -> None:
    """
    Display a progress bar.
    
    Args:
        current: Current progress value
        total: Total/max value
        prefix: Prefix string
        suffix: Suffix string
        length: Character length of the bar
        fill: Fill character
    """
    percent = 100 * (current / float(total))
    filled_length = int(length * current // total)
    bar = fill * filled_length + '-' * (length - filled_length)
    
    sys.stdout.write(f'\r{prefix} |{bar}| {percent:.1f}% {suffix}')
    sys.stdout.flush()
    
    if current == total:
        print()


def section(title: str, width: Optional[int] = None) -> None:
    """
    Print a formatted section header.
    
    Args:
        title: Section title
        width: Width of the section (auto-detected if None)
    """
    if width is None:
        terminal_width = shutil.get_terminal_size((80, 20)).columns
        width = min(terminal_width, 80)
    
    title_with_icon = f"🏗️  {title}"
    
    print("\n" + Fore.CYAN + "=" * width)
    print(Fore.CYAN + title_with_icon)
    print(Fore.CYAN + "=" * width + "\n")


def success(msg: str, icon: str = "✅") -> None:
    """
    Print a success message.
    
    Args:
        msg: Success message
        icon: Icon to display
    """
    print(Fore.GREEN + f"{icon} {msg}")


def error(msg: str, icon: str = "❌") -> None:
    """
    Print an error message.
    
    Args:
        msg: Error message
        icon: Icon to display
    """
    print(Fore.RED + f"{icon} {msg}")


def warning(msg: str, icon: str = "⚠️") -> None:
    """
    Print a warning message.
    
    Args:
        msg: Warning message
        icon: Icon to display
    """
    print(Fore.YELLOW + f"{icon}  {msg}")


def info(msg: str, icon: str = "ℹ️") -> None:
    """
    Print an info message.
    
    Args:
        msg: Info message
        icon: Icon to display
    """
    print(Fore.CYAN + f"{icon}  {msg}")


def debug(msg: str, icon: str = "🔍") -> None:
    """
    Print a debug message.
    
    Args:
        msg: Debug message
        icon: Icon to display
    """
    print(Fore.MAGENTA + f"{icon} {msg}")


def step(number: int, total: int, description: str) -> None:
    """
    Print a step indicator.
    
    Args:
        number: Current step number
        total: Total number of steps
        description: Step description
    """
    print(f"\n{Fore.CYAN}[{number}/{total}]{Style.RESET_ALL} {description}")


def confirm(message: str, default: bool = True) -> bool:
    """
    Ask user for confirmation.
    
    Args:
        message: Confirmation message
        default: Default value if user just presses Enter
        
    Returns:
        True if confirmed, False otherwise
    """
    suffix = " (Y/n)" if default else " (y/N)"
    response = input(f"{message}{suffix}: ").strip().lower()
    
    if not response:
        return default
    
    return response in ['y', 'yes']


def box(content: str, title: Optional[str] = None, color: str = Fore.CYAN) -> None:
    """
    Print content in a box.
    
    Args:
        content: Content to display
        title: Optional title for the box
        color: Color for the box border
    """
    lines = content.split('\n')
    max_length = max(len(line) for line in lines)
    width = max_length + 4
    
    # Top border
    if title:
        title_padded = f" {title} "
        padding = (width - len(title_padded)) // 2
        print(color + "╔" + "═" * padding + title_padded + "═" * (width - padding - len(title_padded)) + "╗")
    else:
        print(color + "╔" + "═" * width + "╗")
    
    # Content
    for line in lines:
        padding = width - len(line) - 2
        print(color + "║ " + Style.RESET_ALL + line + " " * padding + color + " ║")
    
    # Bottom border
    print(color + "╚" + "═" * width + "╝" + Style.RESET_ALL)


def divider(char: str = "─", length: Optional[int] = None, color: str = Fore.CYAN) -> None:
    """
    Print a divider line.
    
    Args:
        char: Character to use for the divider
        length: Length of the divider (auto-detected if None)
        color: Color of the divider
    """
    if length is None:
        terminal_width = shutil.get_terminal_size((80, 20)).columns
        length = min(terminal_width, 80)
    
    print(color + char * length + Style.RESET_ALL)


def timer(func: Callable) -> Callable:
    """
    Decorator to time function execution.
    
    Args:
        func: Function to time
        
    Returns:
        Wrapped function
    """
    def wrapper(*args, **kwargs):
        start = time.time()
        result = func(*args, **kwargs)
        duration = time.time() - start
        print(f"⏱️  {func.__name__} completed in {duration:.2f}s")
        return result
    return wrapper


def clear_line() -> None:
    """Clear the current line in the terminal."""
    sys.stdout.write('\r' + ' ' * 80 + '\r')
    sys.stdout.flush()


def pause(message: str = "Press Enter to continue...") -> None:
    """
    Pause execution until user presses Enter.
    
    Args:
        message: Message to display
    """
    input(f"\n{Fore.YELLOW}⏸️  {message}{Style.RESET_ALL}")


def print_list(items: list, prefix: str = "  •", color: str = Fore.WHITE) -> None:
    """
    Print a formatted list.
    
    Args:
        items: List of items to print
        prefix: Prefix for each item
        color: Color for the items
    """
    for item in items:
        print(color + f"{prefix} {item}" + Style.RESET_ALL)


def print_dict(data: dict, indent: int = 0, color: str = Fore.WHITE) -> None:
    """
    Print a dictionary in a formatted way.
    
    Args:
        data: Dictionary to print
        indent: Indentation level
        color: Color for the output
    """
    for key, value in data.items():
        spacing = "  " * indent
        if isinstance(value, dict):
            print(color + f"{spacing}{key}:" + Style.RESET_ALL)
            print_dict(value, indent + 1, color)
        else:
            print(color + f"{spacing}{key}: {value}" + Style.RESET_ALL)


# Testing when run directly
if __name__ == "__main__":
    print("Testing utility functions...\n")
    
    section("Section Example")
    success("This is a success message")
    error("This is an error message")
    warning("This is a warning message")
    info("This is an info message")
    
    print()
    box("This is content in a box\nWith multiple lines", "Example Box")
    
    print()
    divider()
    
    print("\nProgress bar example:")
    for i in range(101):
        progress_bar(i, 100, prefix="Progress:", suffix="Complete")
        time.sleep(0.02)
    
    print("\nAll tests completed!")