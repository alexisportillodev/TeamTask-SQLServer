import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserPanelComponent } from './components/user-panel/user-panel.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskService } from './services/task.service';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, UserPanelComponent, TaskFormComponent, TaskListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private readonly users = inject(UserService);
  private readonly tasks = inject(TaskService);

  readonly activeTab = signal<'users' | 'tasks'>('users');
  readonly showFormModal = signal(false);

  ngOnInit() {
    this.users.loadUsers().subscribe(() => {
      this.tasks.loadTasks().subscribe();
    });
  }

  switchTab(tab: 'users' | 'tasks') {
    this.activeTab.set(tab);
  }

  openFormModal() {
    this.showFormModal.set(true);
  }

  closeFormModal() {
    this.showFormModal.set(false);
  }
}
