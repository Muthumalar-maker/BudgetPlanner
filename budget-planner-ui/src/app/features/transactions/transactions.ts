
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { TransactionService } from '../../core/services/transaction';
import { CategoryService } from '../../core/services/category';

import {
  Transaction,
  TransactionType,
  CreateTransactionRequest,
  UpdateTransactionRequest
} from '../../core/models/transaction';

import { Category } from '../../core/models/category';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transactions.html',
  styleUrl: './transactions.css'
})
export class Transactions implements OnInit {

  private transactionService = inject(TransactionService);
  private categoryService = inject(CategoryService);

  transactions: Transaction[] = [];
  categories: Category[] = [];

  loading = false;
  saving = false;

  errorMessage = '';
  successMessage = '';

  searchText = '';
  selectedFilter: 'All' | 'Income' | 'Expense' = 'All';

  showForm = false;
  isEditMode = false;
  editingTransactionId: number | null = null;

  formData = {
    categoryId: 0,
    amount: 0,
    type: 'Expense' as TransactionType,
    description: '',
    transactionDate: new Date().toISOString().slice(0, 16)
  };

  ngOnInit(): void {
    this.loadCategories();
    this.loadTransactions();
  }

  // Load categories from backend
  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (data) => {
        this.categories = data;
      },
      error: (error) => {
        console.error('Category loading error:', error);
        this.errorMessage = 'Unable to load categories.';
      }
    });
  }

  // Load transactions from backend
  loadTransactions(): void {
    this.loading = true;
    this.errorMessage = '';

    this.transactionService.getAllTransactions().subscribe({
      next: (data) => {
        this.transactions = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Transaction loading error:', error);
        this.errorMessage =
          error.error?.message || 'Unable to load transactions.';
        this.loading = false;
      }
    });
  }

  // Get categories based on transaction type
  get filteredCategories(): Category[] {
    return this.categories.filter(
      category => category.type === this.formData.type
    );
  }

  // Get filtered transactions
  get filteredTransactions(): Transaction[] {
    return this.transactions.filter(transaction => {

      const matchesType =
        this.selectedFilter === 'All' ||
        transaction.type === this.selectedFilter;

      const matchesSearch =
        transaction.categoryName
          .toLowerCase()
          .includes(this.searchText.toLowerCase()) ||
        (transaction.description || '')
          .toLowerCase()
          .includes(this.searchText.toLowerCase());

      return matchesType && matchesSearch;
    });
  }

  // Calculate total income
  get totalIncome(): number {
    return this.transactions
      .filter(t => t.type === 'Income')
      .reduce((total, t) => total + t.amount, 0);
  }

  // Calculate total expense
  get totalExpense(): number {
    return this.transactions
      .filter(t => t.type === 'Expense')
      .reduce((total, t) => total + t.amount, 0);
  }

  // Calculate balance
  get balance(): number {
    return this.totalIncome - this.totalExpense;
  }

  // Change transaction type
  onTypeChange(): void {
    this.formData.categoryId = 0;
  }

  // Open add form
  openAddForm(): void {
    this.resetForm();
    this.showForm = true;
  }

  // Save transaction
  saveTransaction(): void {

    this.errorMessage = '';
    this.successMessage = '';

    if (this.formData.categoryId <= 0) {
      this.errorMessage = 'Please select a category.';
      return;
    }

    if (this.formData.amount <= 0) {
      this.errorMessage = 'Amount must be greater than zero.';
      return;
    }

    if (!this.formData.transactionDate) {
      this.errorMessage = 'Please select a transaction date.';
      return;
    }

    const request = {
      categoryId: Number(this.formData.categoryId),
      amount: Number(this.formData.amount),
      type: this.formData.type,
      description: this.formData.description.trim(),
      transactionDate: new Date(
        this.formData.transactionDate
      ).toISOString()
    };

    this.saving = true;

    if (this.isEditMode && this.editingTransactionId !== null) {

      const updateRequest: UpdateTransactionRequest = request;

      this.transactionService.updateTransaction(
        this.editingTransactionId,
        updateRequest
      ).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Transaction updated successfully!';
          this.showForm = false;
          this.resetForm();
          this.loadTransactions();
        },
        error: (error) => {
          this.saving = false;
          this.errorMessage =
            error.error?.message || 'Unable to update transaction.';
        }
      });

    } else {

      const createRequest: CreateTransactionRequest = request;

      this.transactionService.createTransaction(
        createRequest
      ).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Transaction added successfully!';
          this.showForm = false;
          this.resetForm();
          this.loadTransactions();
        },
        error: (error) => {
          this.saving = false;
          this.errorMessage =
            error.error?.message || 'Unable to create transaction.';
        }
      });
    }
  }

  // Edit transaction
  editTransaction(transaction: Transaction): void {

    this.isEditMode = true;
    this.editingTransactionId = transaction.transactionId;

    this.formData = {
      categoryId: transaction.categoryId,
      amount: transaction.amount,
      type: transaction.type,
      description: transaction.description || '',
      transactionDate: this.toLocalDateTime(transaction.transactionDate)
    };

    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  // Delete transaction
  deleteTransaction(transaction: Transaction): void {

    const confirmed = confirm(
      `Are you sure you want to delete "${transaction.categoryName}" transaction?`
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.transactionService.deleteTransaction(
      transaction.transactionId
    ).subscribe({
      next: () => {
        this.successMessage = 'Transaction deleted successfully!';
        this.loadTransactions();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Unable to delete transaction.';
      }
    });
  }

  // Reset form
  resetForm(): void {
    this.isEditMode = false;
    this.editingTransactionId = null;

    this.formData = {
      categoryId: 0,
      amount: 0,
      type: 'Expense',
      description: '',
      transactionDate: new Date().toISOString().slice(0, 16)
    };

    this.errorMessage = '';
  }

  // Close form
  closeForm(): void {
    this.showForm = false;
    this.resetForm();
  }

  // Convert API date to local datetime input value
  private toLocalDateTime(date: string): string {
    const value = new Date(date);
    const offset = value.getTimezoneOffset() * 60000;

    return new Date(value.getTime() - offset)
      .toISOString()
      .slice(0, 16);
  }
}
