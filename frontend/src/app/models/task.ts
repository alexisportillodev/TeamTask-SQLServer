export type TaskStatus = 0 | 1 | 2;

export type TaskItem = {
  id: number;
  title: string;
  description?: string | null;
  userId: number;
  status: TaskStatus;
  additionalData?: string | null;
  createdAt?: string;
};

