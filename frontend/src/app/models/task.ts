export type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export type TaskItem = {
  id: number;
  title: string;
  description?: string | null;
  userId: number;
  status: TaskStatus;
  additionalData?: string | null;
  createdAt?: string;
};

