import { DatePipe } from '@angular/common';
import { Component, DestroyRef, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Post, PostCard } from './post-card/post-card';

interface PostDraft {
  title: string;
  excerpt: string;
  imageDataUrls: string[];
}

const STORAGE_KEY = 'angular-post-app.posts';

const STARTER_POSTS: Post[] = [
  {
    id: 'starter-1',
    title: 'Male ideje koje mijenjaju svakodnevicu',
    excerpt:
      'Nekoliko jednostavnih navika može potpuno promijeniti način na koji planiramo dan i čuvamo vrijeme za važne stvari.',
    publishedAt: '2026-10-03T09:30:00.000Z',
    readingTime: 2,
    accent: 'coral',
    imageDataUrls: [],
  },
  {
    id: 'starter-2',
    title: 'Bilješke s jednog sporog nedjeljnog jutra',
    excerpt:
      'Kava, otvoren prozor i popis knjiga koje već dugo čekaju. Ponekad je sporiji ritam upravo ono što nam treba.',
    publishedAt: '2026-09-28T08:15:00.000Z',
    readingTime: 3,
    accent: 'sage',
    imageDataUrls: [],
  },
];

@Component({
  selector: 'app-root',
  imports: [DatePipe, FormsModule, PostCard],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild('imageInput') private imageInput?: ElementRef<HTMLInputElement>;
  @ViewChild('postDialog') private postDialog?: ElementRef<HTMLDialogElement>;

  protected readonly posts = signal<Post[]>(this.loadPosts());
  protected readonly selectedPost = signal<Post | null>(null);
  protected readonly currentTime = signal(new Date());
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly isProcessingImage = signal(false);

  protected draft: PostDraft = this.emptyDraft();

  constructor() {
    const clockTimer = globalThis.setInterval(() => this.currentTime.set(new Date()), 1000);
    this.destroyRef.onDestroy(() => globalThis.clearInterval(clockTimer));
  }

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
      publishedAt: new Date().toISOString(),
      readingTime: this.calculateReadingTime(`${title} ${excerpt}`),
      accent: this.nextAccent(),
      imageDataUrls: [...this.draft.imageDataUrls],
    };

    const nextPosts = [newPost, ...this.posts()];

    if (!this.savePosts(nextPosts)) {
      return;
    }

    this.posts.set(nextPosts);
    this.draft = this.emptyDraft();
    if (this.imageInput) {
      this.imageInput.nativeElement.value = '';
    }
    this.errorMessage.set('');
    this.successMessage.set('Post je dodan.');
  }

  protected removePost(id: string): void {
    const nextPosts = this.posts().filter((post) => post.id !== id);

    if (!this.savePosts(nextPosts)) {
      return;
    }

    this.posts.set(nextPosts);
    this.successMessage.set('Post je uklonjen.');
  }

  protected openPost(post: Post): void {
    this.selectedPost.set(post);

    if (this.postDialog && !this.postDialog.nativeElement.open) {
      this.postDialog.nativeElement.showModal();
    }
  }

  protected closePost(): void {
    this.postDialog?.nativeElement.close();
  }

  protected closePostFromBackdrop(event: MouseEvent): void {
    if (event.target === this.postDialog?.nativeElement) {
      this.closePost();
    }
  }

  protected async selectImages(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);

    if (!files.length) {
      return;
    }

    this.clearMessage();

    if (this.draft.imageDataUrls.length + files.length > 8) {
      this.errorMessage.set('Jedan post može imati najviše 8 slika.');
      input.value = '';
      return;
    }

    if (files.some((file) => !['image/jpeg', 'image/png', 'image/webp'].includes(file.type))) {
      this.errorMessage.set('Odaberi JPG, PNG ili WebP sliku.');
      input.value = '';
      return;
    }

    if (files.some((file) => file.size > 12 * 1024 * 1024)) {
      this.errorMessage.set('Svaka slika može imati najviše 12 MB.');
      input.value = '';
      return;
    }

    this.isProcessingImage.set(true);

    try {
      const compressedImages: string[] = [];

      for (const file of files) {
        compressedImages.push(await this.compressImage(file));
      }

      this.draft.imageDataUrls = [...this.draft.imageDataUrls, ...compressedImages];
    } catch {
      this.errorMessage.set('Neke slike nisu se mogle obraditi. Pokušaj ponovno.');
    } finally {
      input.value = '';
      this.isProcessingImage.set(false);
    }
  }

  protected removeSelectedImage(index: number): void {
    this.draft.imageDataUrls = this.draft.imageDataUrls.filter((_, imageIndex) => imageIndex !== index);
    this.clearMessage();
  }

  protected clearMessage(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  private emptyDraft(): PostDraft {
    return {
      title: '',
      excerpt: '',
      imageDataUrls: [],
    };
  }

  private compressImage(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const objectUrl = URL.createObjectURL(file);
      const image = new Image();

      image.onload = () => {
        const maxWidth = 1400;
        const maxHeight = 900;
        const scale = Math.min(1, maxWidth / image.width, maxHeight / image.height);
        const width = Math.round(image.width * scale);
        const height = Math.round(image.height * scale);
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');

        canvas.width = width;
        canvas.height = height;

        if (!context) {
          URL.revokeObjectURL(objectUrl);
          reject(new Error('Canvas nije dostupan.'));
          return;
        }

        context.drawImage(image, 0, 0, width, height);
        URL.revokeObjectURL(objectUrl);
        resolve(canvas.toDataURL('image/webp', 0.82));
      };

      image.onerror = () => {
        URL.revokeObjectURL(objectUrl);
        reject(new Error('Slika se nije mogla učitati.'));
      };

      image.src = objectUrl;
    });
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
      const parsedPosts: LegacyPost[] = savedPosts === null ? STARTER_POSTS : JSON.parse(savedPosts);

      if (!Array.isArray(parsedPosts)) {
        return STARTER_POSTS;
      }

      return parsedPosts.map((post) => ({
        id: post.id,
        title: post.title,
        excerpt: post.excerpt,
        publishedAt: post.publishedAt,
        readingTime: post.readingTime,
        accent: post.accent,
        imageDataUrls: post.imageDataUrls ?? (post.imageDataUrl ? [post.imageDataUrl] : []),
      }));
    } catch {
      return STARTER_POSTS;
    }
  }

  private savePosts(posts: Post[]): boolean {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(posts));
      return true;
    } catch {
      this.errorMessage.set('Nema dovoljno prostora za spremanje. Pokušaj s manjom slikom ili ukloni neki post.');
      this.successMessage.set('');
      return false;
    }
  }
}

type LegacyPost = Omit<Post, 'imageDataUrls'> & {
  imageDataUrls?: string[];
  imageDataUrl?: string;
};
