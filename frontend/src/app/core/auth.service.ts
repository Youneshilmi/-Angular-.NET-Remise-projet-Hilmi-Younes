import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { AuthSession } from './models';

const API_URL = 'http://localhost:5148/api';
const STORAGE_KEY = 'service-market-session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly sessionState = signal<AuthSession | null>(this.readSession());

  readonly session = this.sessionState.asReadonly();
  readonly user = computed(() => this.sessionState()?.user ?? null);
  readonly token = computed(() => this.sessionState()?.token ?? null);
  readonly isAuthenticated = computed(() => this.sessionState() !== null);
  readonly isAdmin = computed(() => this.sessionState()?.user.role === 'Admin');

  login(email: string, password: string) {
    return this.http.post<AuthSession>(`${API_URL}/auth/login`, { email, password }).pipe(
      tap(session => this.saveSession(session))
    );
  }

  register(fullName: string, email: string, password: string) {
    return this.http.post<AuthSession>(`${API_URL}/auth/register`, { fullName, email, password }).pipe(
      tap(session => this.saveSession(session))
    );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.sessionState.set(null);
  }

  private saveSession(session: AuthSession): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    this.sessionState.set(session);
  }

  private readSession(): AuthSession | null {
    const value = localStorage.getItem(STORAGE_KEY);
    if (!value) return null;
    try {
      return JSON.parse(value) as AuthSession;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}
