import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface Post {
  id: string;
  title: string;
  excerpt: string;
  category: string;
  publishedAt: string;
  readingTime: number;
  accent: 'coral' | 'sage' | 'sky' | 'sand';
  imageDataUrl?: string;
}

@Component({
  selector: 'app-post-card',
  imports: [DatePipe],
  templateUrl: './post-card.html',
  styleUrl: './post-card.scss',
})
export class PostCard {
  @Input({ required: true }) post!: Post;
  @Output() removePost = new EventEmitter<string>();
}
