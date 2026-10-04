import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Post, PostCard } from './post-card/post-card';

interface PostDraft {
  title: string;
  excerpt: string;
  category: string;
}

const STORAGE_KEY = 'angular-post-app.posts';

const STARTER_POSTS: Post[] = [
  {
    id: 'starter-1',
    title: 'Male ideje koje mijenjaju svakodnevicu',
    excerpt:
      'Nekoliko jednostavnih navika može potpuno promijeniti način na koji planiramo dan i čuvamo vrijeme za važne stvari.',
    category: 'Ideje',
    publishedAt: '2026-10-03T09:30:00.000Z',
    readingTime: 2,
    accent: 'coral',
  },
  {
    id: 'starter-2',
    title: 'Bilješke s jednog sporog nedjeljnog jutra',
    excerpt:
      'Kava, otvoren prozor i popis knjiga koje već dugo čekaju. Ponekad je sporiji ritam upravo ono što nam treba.',
    category: 'Život',
    publishedAt: '2026-09-28T08:15:00.000Z',
    readingTime: 3,
    accent: 'sage',
  },
];

@Component({
  selector: 'app-root',
  imports: [FormsModule, PostCard],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly posts = signal<Post[]>(this.loadPosts());
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');

  protected draft: PostDraft = this.emptyDraft();

  protected addPost(): void {
    const title = this.draft.title.trim();
    const excerpt = this.draft.excerpt.trim();

    if (!title || !excerpt) {
      this.errorMessage.set('Upiši naslov i kratki tekst posta.');
      this.successMessage.set('');
      return;
    }

    const newPost: Post = {
      id: globalThis.crypto?.randomUUID?.() ?? `${Date.now()}`,
      title,
      excerpt,
      category: this.draft.category,
      publishedAt: new Date().toISOString(),
      readingTime: this.calculateReadingTime(`${title} ${excerpt}`),
      accent: this.nextAccent(),
    };

    this.posts.update((posts) => [newPost, ...posts]);
    this.savePosts();
    this.draft = this.emptyDraft();
    this.errorMessage.set('');
    this.successMessage.set('Post je dodan.');
  }

  protected removePost(id: string): void {
    this.posts.update((posts) => posts.filter((post) => post.id !== id));
    this.savePosts();
    this.successMessage.set('Post je uklonjen.');
  }

  protected clearMessage(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  private emptyDraft(): PostDraft {
    return {
      title: '',
      excerpt: '',
      category: 'Ideje',
    };
  }

  private calculateReadingTime(text: string): number {
    const wordCount = text.split(/\s+/).filter(Boolean).length;
    return Math.max(1, Math.ceil(wordCount / 180));
  }

  private nextAccent(): Post['accent'] {
    const accents: Post['accent'][] = ['coral', 'sage', 'sky', 'sand'];
    return accents[this.posts().length % accents.length];
  }

  private loadPosts(): Post[] {
    try {
      const savedPosts = localStorage.getItem(STORAGE_KEY);
      return savedPosts === null ? STARTER_POSTS : JSON.parse(savedPosts);
    } catch {
      return STARTER_POSTS;
    }
  }

  private savePosts(): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(this.posts()));
  }
}
