import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Category, ServiceInput, ServiceOffering } from './models';

const API_URL = 'http://localhost:5148/api/services';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly http = inject(HttpClient);

  getAll(search = '', categoryId?: number) {
    let params = new HttpParams();
    if (search.trim()) params = params.set('search', search.trim());
    if (categoryId) params = params.set('categoryId', categoryId);
    return this.http.get<ServiceOffering[]>(API_URL, { params });
  }

  getAllForAdmin() {
    return this.http.get<ServiceOffering[]>(`${API_URL}/admin`);
  }

  getCategories() {
    return this.http.get<Category[]>(`${API_URL}/categories`);
  }

  create(input: ServiceInput) {
    return this.http.post<{ id: number }>(API_URL, input);
  }

  update(id: number, input: ServiceInput) {
    return this.http.put<void>(`${API_URL}/${id}`, input);
  }

  delete(id: number) {
    return this.http.delete<void>(`${API_URL}/${id}`);
  }
}
