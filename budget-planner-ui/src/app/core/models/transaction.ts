
export type TransactionType = 'Income' | 'Expense';

export interface Transaction {
  transactionId: number;
  categoryId: number;
  categoryName: string;
  amount: number;
  type: TransactionType;
  description: string | null;
  transactionDate: string;
  createdAt: string;
}

export interface CreateTransactionRequest {
  categoryId: number;
  amount: number;
  type: TransactionType;
  description: string;
  transactionDate: string;
}

export interface UpdateTransactionRequest {
  categoryId: number;
  amount: number;
  type: TransactionType;
  description: string;
  transactionDate: string;
}
