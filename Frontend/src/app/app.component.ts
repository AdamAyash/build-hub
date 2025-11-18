import { Component } from '@angular/core';
import { HasPermission } from "./core/directives/has-permission";

@Component({
  selector: 'bh-root',
  imports: [HasPermission],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'BuildHub Frontend';
}
