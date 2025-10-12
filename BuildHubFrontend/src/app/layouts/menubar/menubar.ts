import { Component } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { MenubarModule } from 'primeng/menubar';
import { DarkModeToggle } from '../../shared/components/dark-mode-toggle/dark-mode-toggle';
import { NotificationBell } from '../../shared/components/notification-bell/notification-bell';
import { OverlayBadge } from 'primeng/overlaybadge';

@Component({
  selector: 'app-menubar',
  imports: [
    MenubarModule,
    BadgeModule,
    AvatarModule,
    DarkModeToggle,
    NotificationBell,
    OverlayBadge,
  ],
  templateUrl: './menubar.html',
  styleUrl: './menubar.css',
})
export class Menubar {}
