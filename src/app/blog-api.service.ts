import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';

export interface ApiPost {
  id: number;
  adminUserId: number;
  categoryId: number | null;
  title: string;
  content: string;
  imageUrls: string[];
  createdAt: string;
}

export interface SavePostRequest {
  adminUserId: number;
  categoryId: number | null;
  title: string;
  content: string;
  imageUrls: string[];
}

interface AdminUser {
  id: number;
  username: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class BlogApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';

  getPosts(): Observable<ApiPost[]> {
    return this.http.get<ApiPost[]>(`${this.apiUrl}/posts`);
  }

  createPost(request: SavePostRequest, apiKey: string): Observable<ApiPost> {
    return this.http.post<ApiPost>(`${this.apiUrl}/posts`, request, this.authOptions(apiKey));
  }

  updatePost(id: number, request: SavePostRequest, apiKey: string): Observable<ApiPost> {
    return this.http.put<ApiPost>(
      `${this.apiUrl}/posts/${id}`,
      request,
      this.authOptions(apiKey),
    );
  }

  deletePost(id: number, apiKey: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/posts/${id}`, this.authOptions(apiKey));
  }

  ensureOwner(apiKey: string): Observable<AdminUser> {
    const options = this.authOptions(apiKey);

    return this.http.get<AdminUser[]>(`${this.apiUrl}/admin-users`, options).pipe(
      switchMap((users) => {
        if (users.length) {
          return of(users[0]);
        }

        return this.http.post<AdminUser>(
          `${this.apiUrl}/admin-users`,
          {
            username: 'blog-admin',
            email: 'admin@blog.local',
            password: globalThis.crypto?.randomUUID?.() ?? `local-${Date.now()}`,
          },
          options,
        );
      }),
    );
  }

  private authOptions(apiKey: string): { headers: HttpHeaders } {
    return {
      headers: new HttpHeaders({ Authorization: apiKey }),
    };
  }
}
