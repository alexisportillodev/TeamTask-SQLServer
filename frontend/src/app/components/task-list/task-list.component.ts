import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';
import { TaskStatus } from '../../models/task';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent {
  private readonly tasks = inject(TaskService);
  private readonly users = inject(UserService);

  readonly statuses: (TaskStatus | '')[] = ['', 'Pending', 'InProgress', 'Done'];

  get statusFilter() {
    return this.tasks.statusFilter();
  }

  setFilter(value: TaskStatus | '') {
    this.tasks.statusFilter.set(value);
  }

  onFilterChange(event: Event) {
    const value = (event.target as HTMLSelectElement | null)?.value ?? '';
    this.setFilter((value as TaskStatus | '') ?? '');
  }

  get list() {
    return this.tasks.filteredTasks();
  }

  userLabel(userId: number) {
    const u = this.users.usersById().get(userId);
    return u ? `${u.name} (${u.email})` : `User #${userId}`;
  }

  refresh() {
    this.tasks.loadTasks().subscribe();
  }

  changeStatus(id: number, status: TaskStatus) {
    this.tasks.updateStatus(id, { status }).subscribe();
  }

  onStatusChange(id: number, event: Event) {
    const value = (event.target as HTMLSelectElement | null)?.value ?? 'Pending';
    this.changeStatus(id, value as TaskStatus);
  }
}

