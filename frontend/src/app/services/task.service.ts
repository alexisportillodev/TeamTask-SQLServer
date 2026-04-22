import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { TaskItem, TaskStatus } from '../models/task';

export type CreateTaskDto = {
  title: string;
  description?: string | null;
  userId: number;
  additionalData?: string | null;
};

export type UpdateTaskStatusDto = {
  status: TaskStatus;
};

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);

  readonly tasks = signal<TaskItem[]>([]);
  readonly statusFilter = signal<TaskStatus | ''>('');

  readonly filteredTasks = computed(() => {
    const filter = this.statusFilter();
    if (!filter) return this.tasks();
    return this.tasks().filter((t) => t.status === filter);
  });

  loadTasks() {
    return this.http.get<TaskItem[]>(`${API_BASE_URL}/api/tasks`).pipe(
      tap((tasks) => this.tasks.set(tasks)),
      catchError((err) => {
        console.error('Failed to load tasks', err);
        return of([] as TaskItem[]);
      }),
    );
  }

  createTask(dto: CreateTaskDto) {
    return this.http.post<TaskItem>(`${API_BASE_URL}/api/tasks`, dto).pipe(
      tap((created) => this.tasks.update((prev) => [created, ...prev])),
      catchError((err) => {
        console.error('Failed to create task', err);
        throw err;
      }),
    );
  }

  updateStatus(id: number, dto: UpdateTaskStatusDto) {
    return this.http.put<TaskItem>(`${API_BASE_URL}/api/tasks/${id}/status`, dto).pipe(
      tap((updated) => {
        this.tasks.update((prev) => prev.map((t) => (t.id === updated.id ? updated : t)));
      }),
      catchError((err) => {
        console.error('Failed to update status', err);
        throw err;
      }),
    );
  }
}

