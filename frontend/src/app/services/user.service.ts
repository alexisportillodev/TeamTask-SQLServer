import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { User } from '../models/user';

export type CreateUserDto = {
  name: string;
  email: string;
};

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);

  readonly users = signal<User[]>([]);
  readonly usersById = computed(() => new Map(this.users().map((u) => [u.id, u])));

  loadUsers() {
    return this.http.get<User[]>(`${API_BASE_URL}/api/users`).pipe(
      tap((users) => this.users.set(users)),
      catchError((err) => {
        console.error('Failed to load users', err);
        return of([] as User[]);
      }),
    );
  }

  createUser(dto: CreateUserDto) {
    return this.http.post<User>(`${API_BASE_URL}/api/users`, dto).pipe(
      tap((created) => this.users.update((prev) => [created, ...prev])),
      catchError((err) => {
        console.error('Failed to create user', err);
        throw err;
      }),
    );
  }
}

