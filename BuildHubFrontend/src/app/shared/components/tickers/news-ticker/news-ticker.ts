import { Component, inject } from '@angular/core';
import { TickerService } from '../../../../core/services/ticker/ticker.service';

@Component({
  selector: 'app-news-ticker',
  imports: [],
  templateUrl: './news-ticker.html',
  styleUrl: './news-ticker.css',
})
export class NewsTicker {
  public thickerService = inject(TickerService);
}
