import { Component } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import {
  ChevronDown,
  ChevronUp,
  Activity,
  House,
  CirclePlay,
  Bell,
  Settings,
  GitBranch,
  Calendar,
  Server,
  Cpu,
  Shield,
  Users,
  Pin,
} from 'lucide-angular';
import { Icon } from '../../shared/components/icon/icon';
import { NavGroup } from '../../shared/models/navigation/navigation-group';
import { Button } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { RouterLink } from '@angular/router';
import { ScrollPanelModule } from 'primeng/scrollpanel';

@Component({
  selector: 'app-sidebar',
  imports: [AccordionModule, ScrollPanelModule, Icon, Button, TagModule, RouterLink],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class Sidebar {
  public readonly ChevronDownIcon = ChevronDown;
  public readonly ChevronUpIcon = ChevronUp;
  public readonly PinIcon = Pin;

  public navGroups: NavGroup[] = [
    {
      id: 'core',
      label: 'Core Operations',
      icon: Activity,
      defaultOpen: true,
      items: [
        {
          id: 'dashboard',
          label: 'Dashboard',
          icon: House,
          page: 'dashboard',
          description: 'System overview and real-time metrics',
          shortcut: '⌘1',
        },
        {
          id: 'jobs',
          label: 'Active Jobs',
          icon: CirclePlay,
          page: 'jobs',
          // badge: {
          //   content: realTimeData.metrics.activeBuilds,
          //   variant: realTimeData.metrics.activeBuilds > 0 ? 'default' : 'secondary',
          //   pulse: realTimeData.metrics.activeBuilds > 0,
          // },
          description: 'Monitor running and queued build jobs',
          shortcut: '⌘2',
        },
        // {
        //   id: 'build-requests',
        //   label: 'Build Requests',
        //   icon: GitPullRequest,
        //   page: 'build-requests',
        //   badge: {
        //     content: '4',
        //     variant: 'secondary',
        //   },
        //   description: 'Manage and approve build requests from users',
        //   shortcut: '⌘3',
        // },
        {
          id: 'notifications',
          label: 'Notifications',
          icon: Bell,
          page: 'notifications',
          // badge: {
          //   content: realTimeData.notifications.filter(
          //     (n) => n.type === 'error' || n.type === 'warning'
          //   ).length,
          //   variant: 'destructive',
          //   pulse: true,
          // },
          description: 'System alerts and job notifications',
          shortcut: '⌘4',
        },
      ],
    },
    {
      id: 'configuration',
      label: 'Configuration',
      icon: Settings,
      defaultOpen: true,
      collapsible: true,
      items: [
        {
          id: 'builds',
          label: 'Build Definitions',
          icon: GitBranch,
          page: 'builds',
          description: 'Manage build configurations and pipelines',
          shortcut: '⌘5',
        },
        {
          id: 'schedules',
          label: 'Schedules',
          icon: Calendar,
          page: 'schedules',
          // badge: {
          //   content: '3',
          //   variant: 'outline',
          // },
          description: 'Automated build scheduling',
          shortcut: '⌘6',
        },
      ],
    },
    {
      id: 'infrastructure',
      label: 'Infrastructure',
      icon: Server,
      defaultOpen: false,
      collapsible: true,
      items: [
        {
          id: 'workers',
          label: 'Build Workers',
          icon: Cpu,
          page: 'workers',
          // badge: {
          //   content: `${realTimeData.metrics.workersOnline}/${realTimeData.metrics.totalWorkers}`,
          //   variant:
          //     realTimeData.metrics.workersOnline === realTimeData.metrics.totalWorkers
          //       ? 'default'
          //       : 'destructive',
          // },
          description: 'Worker nodes and resource management',
          shortcut: '⌘7',
          isNew: true
        },
        // {
        //   id: 'statistics',
        //   label: 'Analytics',
        //   icon: TrendingUp,
        //   page: 'statistics',
        //   description: 'Performance metrics and insights',
        //   shortcut: '⌘8',
        //   isNew: true,
        // },
        // {
        //   id: 'maintenance',
        //   label: 'Maintenance',
        //   icon: Wrench,
        //   page: 'maintenance',
        //   description: 'System maintenance windows and scheduling',
        //   shortcut: '⌘M',
        // },
        // {
        //   id: 'maintenance-notification',
        //   label: 'Maintenance Notice',
        //   icon: TriangleAlert,
        //   page: 'maintenance-notification',
        //   description: 'User maintenance notification page',
        //   shortcut: '⌘N',
        // },
        // {
        //   id: 'system-status',
        //   label: 'System Status',
        //   icon: Activity,
        //   page: 'system-status',
        //   description: 'Real-time system health and performance',
        //   shortcut: '⌘S',
        // },
      ],
    },
    {
      id: 'administration',
      label: 'Administration',
      icon: Shield,
      defaultOpen: false,
      collapsible: true,
      items: [
        {
          id: 'users',
          label: 'User Management',
          icon: Users,
          page: 'users',
          description: 'Manage users, roles, and permissions',
          shortcut: '⌘9',
        },
        {
          id: 'settings',
          label: 'System Settings',
          icon: Settings,
          page: 'settings',
          description: 'Global configuration and preferences',
          shortcut: '⌘0',
        },
      ],
    },
  ];
}
