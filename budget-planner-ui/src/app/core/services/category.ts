
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/categories`;

  // GET: Get all categories
  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  // GET: Get category by ID
  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  // POST: Create category
  createCategory(
    category: CreateCategoryRequest
  ): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category);
  }

  // PUT: Update category
  updateCategory(
    id: number,
    category: UpdateCategoryRequest
  ): Observable<Category> {
    return this.http.put<Category>(
      `${this.apiUrl}/${id}`,
      category
    );
  }

  // DELETE: Delete category
  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
