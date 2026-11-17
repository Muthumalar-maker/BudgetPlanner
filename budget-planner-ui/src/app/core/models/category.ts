
export type CategoryType = 'Income' | 'Expense';

export interface Category {
  categoryId: number;
  categoryName: string;
  type: CategoryType;
  createdAt: string;
}

export interface CreateCategoryRequest {
  categoryName: string;
  type: CategoryType;
}

export interface UpdateCategoryRequest {
  categoryName: string;
  type: CategoryType;
}
