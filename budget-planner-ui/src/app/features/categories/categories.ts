
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Category, CategoryType } from '../../core/models/category';
import { CategoryService } from '../../core/services/category';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class Categories implements OnInit {

  private categoryService = inject(CategoryService);

  categories: Category[] = [];

  categoryName = '';
  type: CategoryType = 'Expense';

  selectedCategoryId: number | null = null;

  filterType = 'All';

  loading = false;
  saving = false;

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadCategories();
  }

  get filteredCategories(): Category[] {
    if (this.filterType === 'All') {
      return this.categories;
    }

    return this.categories.filter(
      category => category.type === this.filterType
    );
  }

  get incomeCount(): number {
    return this.categories.filter(
      category => category.type === 'Income'
    ).length;
  }

  get expenseCount(): number {
    return this.categories.filter(
      category => category.type === 'Expense'
    ).length;
  }

  loadCategories(): void {
    this.loading = true;
    this.errorMessage = '';

    this.categoryService.getAllCategories().subscribe({
      next: (response) => {
        this.categories = response;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage =
          error.error?.message || 'Unable to load categories.';
      }
    });
  }

  saveCategory(): void {
    this.errorMessage = '';
    this.successMessage = '';

    const name = this.categoryName.trim();

    if (!name) {
      this.errorMessage = 'Please enter category name.';
      return;
    }

    if (name.length < 2 || name.length > 50) {
      this.errorMessage = 'Category name must be between 2 and 50 characters.';
      return;
    }

    const categoryData = {
      categoryName: name,
      type: this.type
    };

    this.saving = true;

    if (this.selectedCategoryId !== null) {

      this.categoryService.updateCategory(
        this.selectedCategoryId,
        categoryData
      ).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Category updated successfully!';
          this.resetForm();
          this.loadCategories();
        },
        error: (error) => {
          this.saving = false;
          this.errorMessage =
            error.error?.message || 'Unable to update category.';
        }
      });

    } else {

      this.categoryService.createCategory(categoryData).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Category created successfully!';
          this.resetForm();
          this.loadCategories();
        },
        error: (error) => {
          this.saving = false;
          this.errorMessage =
            error.error?.message || 'Unable to create category.';
        }
      });

    }
  }

  editCategory(category: Category): void {
    this.selectedCategoryId = category.categoryId;
    this.categoryName = category.categoryName;
    this.type = category.type;

    this.errorMessage = '';
    this.successMessage = '';

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteCategory(category: Category): void {
    const confirmed = window.confirm(
      `Are you sure you want to delete "${category.categoryName}"?`
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.categoryService.deleteCategory(category.categoryId).subscribe({
      next: () => {
        this.successMessage = 'Category deleted successfully!';
        this.loadCategories();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Unable to delete category.';
      }
    });
  }

  resetForm(): void {
    this.categoryName = '';
    this.type = 'Expense';
    this.selectedCategoryId = null;
  }

}
