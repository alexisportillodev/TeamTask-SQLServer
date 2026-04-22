import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-panel',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-panel.component.html',
  styleUrl: './user-panel.component.css',
})
export class UserPanelComponent {
  private readonly users = inject(UserService);

  readonly isSubmitting = signal(false);

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
  });

  get usersList() {
    return this.users.users();
  }

  refresh() {
    this.users.loadUsers().subscribe();
  }

  submit() {
    if (this.form.invalid || this.isSubmitting()) return;
    this.isSubmitting.set(true);

    const dto = this.form.getRawValue();
    this.users.createUser(dto).subscribe({
      next: () => {
        this.form.reset({ name: '', email: '' });
        this.isSubmitting.set(false);
      },
      error: () => this.isSubmitting.set(false),
    });
  }
}

