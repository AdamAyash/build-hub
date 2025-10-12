import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Icon } from '../icon/icon';
import { Sun, Moon } from 'lucide-angular';

@Component({
  selector: 'app-dark-mode-toggle',
  imports: [Icon, ButtonModule],
  templateUrl: './dark-mode-toggle.html',
  styleUrl: './dark-mode-toggle.css',
})
export class DarkModeToggle {
  public isDarkMode = false;
  public readonly SunIcon = Sun;
  public readonly MoonIcon = Moon;
}
