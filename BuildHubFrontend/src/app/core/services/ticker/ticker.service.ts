import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TickerService {
  public isTickerVisible = false;
  public tickerMessages = [
    'test1',
    'Lorem ipsum dolor, sit amet consectetur adipisicing elit. ',
  ];
}
