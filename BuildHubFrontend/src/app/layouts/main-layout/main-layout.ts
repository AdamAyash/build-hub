import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menubar } from '../menubar/menubar';
import { Sidebar } from '../sidebar/sidebar';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-main-layout',
  imports: [Menubar, RouterOutlet, Sidebar, BreadcrumbModule],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
  public items: MenuItem[] = [
    { label: 'Electronics' },
    { label: 'Computer' },
    { label: 'Accessories' },
    { label: 'Keyboard' },
    { label: 'Wireless' },
  ];

  public home: MenuItem = { label: 'Build Hub', icon: '', routerLink: '/' };
}
