import { Component, inject } from '@angular/core';
import { Ticker } from '../../../../core/services/ticker/ticker';

@Component({
  selector: 'app-news-ticker',
  imports: [],
  templateUrl: './news-ticker.html',
  styleUrl: './news-ticker.css',
})
export class NewsTicker {
  public thickerService = inject(Ticker);
}
