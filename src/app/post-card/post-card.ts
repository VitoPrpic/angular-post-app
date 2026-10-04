import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface Post {
  id: number;
  adminUserId: number;
  categoryId: number | null;
  title: string;
  excerpt: string;
  publishedAt: string;
  readingTime: number;
  accent: 'coral' | 'sage' | 'sky' | 'sand';
  imageDataUrls: string[];
}

@Component({
  selector: 'app-post-card',
  imports: [DatePipe],
  templateUrl: './post-card.html',
  styleUrl: './post-card.scss',
})
export class PostCard {
  @Input({ required: true }) post!: Post;
  @Input() deleting = false;
  @Output() openPost = new EventEmitter<Post>();
  @Output() editPost = new EventEmitter<Post>();
  @Output() removePost = new EventEmitter<number>();
}
