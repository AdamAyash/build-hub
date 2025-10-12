import { Component } from '@angular/core';
import { PageHeader } from '../../layouts/page-header/page-header';
import { TagModule } from 'primeng/tag';
import { TabsModule } from 'primeng/tabs';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Search, Funnel } from 'lucide-angular';
import { SelectModule } from 'primeng/select';
import { Icon } from '../../shared/components/icon/icon';


@Component({
  selector: 'app-notifications',
  imports: [
    PageHeader,
    TagModule,
    TabsModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    Icon,
    SelectModule,
  ],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css',
})
export class Notifications {
  public readonly SearchIcon = Search;
  public readonly FunnelIcon = Funnel;
  public readonly filterValues = [
    { code: 'all', name: 'All Types' },
    { code: 'error', name: 'Errors' },
    { code: 'warning', name: 'Warnings' },
    { code: 'success', name: 'Success' },
    { code: 'info', name: 'Info' },
  ];
}
