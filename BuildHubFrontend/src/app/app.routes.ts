import { Routes } from '@angular/router';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Dashboard } from './features/dashboard/dashboard';
import { Notifications } from './features/notifications/notifications';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'notifications',
        component: Notifications,
      },
    ],
  },
];
