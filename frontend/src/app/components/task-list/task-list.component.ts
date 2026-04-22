import { Component, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';
import { TaskStatus } from '../../models/task';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent implements OnInit {
  private readonly tasks = inject(TaskService);
  private readonly users = inject(UserService);

  readonly filterOptions = computed(() => {
    const allStatuses = new Set<TaskStatus | ''>(['']);
    this.tasks.tasks().forEach(t => allStatuses.add(t.status));
    return Array.from(allStatuses).sort();
  });

  ngOnInit() {
    this.refresh();
  }

  get statusFilter() {
    return this.tasks.statusFilter();
  }

  setFilter(value: TaskStatus | '') {
    this.tasks.statusFilter.set(value);
  }

  onFilterChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.setFilter(value === '' ? '' : (+value as TaskStatus));
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
    const value = +(event.target as HTMLSelectElement).value;
    this.changeStatus(id, value as TaskStatus);
  }
}
