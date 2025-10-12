import { Component, Input } from '@angular/core';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';

@Component({
  selector: 'app-icon',
  imports: [LucideAngularModule],
  templateUrl: './icon.html',
  styleUrl: './icon.css',
})
export class Icon {
  @Input() public icon?: LucideIconData;
  @Input() public size = 4;

  public getSize(): number {
    return (this.size * 0.25 * 16);
  }
}
