import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private token: string | null = 'dummy-token';

  getToken(): string | null {
    return this.token;
  }

  clear() {
    this.token = null;
  }
}
