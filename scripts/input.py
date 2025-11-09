def prompt_with_default(prompt, default):
    value = input(f"{prompt}: ").strip()
    return value if value else default