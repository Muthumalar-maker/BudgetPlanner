
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  Transaction,
  CreateTransactionRequest,
  UpdateTransactionRequest
} from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/transactions`;

  // Get all transactions
  getAllTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl);
  }

  // Get transaction by ID
  getTransactionById(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/${id}`);
  }

  // Create transaction
  createTransaction(
    transaction: CreateTransactionRequest
  ): Observable<Transaction> {
    return this.http.post<Transaction>(
      this.apiUrl,
      transaction
    );
  }

  // Update transaction
  updateTransaction(
    id: number,
    transaction: UpdateTransactionRequest
  ): Observable<Transaction> {
    return this.http.put<Transaction>(
      `${this.apiUrl}/${id}`,
      transaction
    );
  }

  // Delete transaction
  deleteTransaction(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
