import { Component, inject, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css',
})
export class TaskFormComponent {
  private readonly tasks = inject(TaskService);
  private readonly users = inject(UserService);

  readonly onClose = input<() => void>(() => {});
  readonly isFormOnly = input<boolean>(false);
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string>('');

  readonly form = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    description: new FormControl<string | null>(null),
    userId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    additionalData: new FormControl<string | null>(null),
  });

  get usersList() {
    return this.users.users();
  }

  submit() {
    if (this.form.invalid || this.isSubmitting()) return;
    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const raw = this.form.getRawValue();
    this.tasks
      .createTask({
        title: raw.title,
        description: raw.description,
        userId: raw.userId ?? 0,
        additionalData: raw.additionalData,
      })
      .subscribe({
        next: () => {
          this.form.reset({ title: '', description: null, userId: null, additionalData: null });
          this.isSubmitting.set(false);
          this.onClose()();
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const errorMsg = err.error?.error || err.message || 'An error occurred';
          this.errorMessage.set(errorMsg);
        },
      });
  }
}

