import { Component } from '@angular/core';
import { PageHeader } from "../../layouts/page-header/page-header";
import { TabsModule } from 'primeng/tabs';

@Component({
  selector: 'app-dashboard',
  imports: [PageHeader, TabsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {}
