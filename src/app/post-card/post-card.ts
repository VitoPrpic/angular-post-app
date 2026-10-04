import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface Post {
  id: string;
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
  @Output() openPost = new EventEmitter<Post>();
  @Output() removePost = new EventEmitter<string>();
}
