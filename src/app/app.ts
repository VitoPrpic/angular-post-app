import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ApiPost, BlogApiService, SavePostRequest } from './blog-api.service';
import { Post, PostCard } from './post-card/post-card';

interface PostDraft {
  title: string;
  excerpt: string;
  imageDataUrls: string[];
}

@Component({
  selector: 'app-root',
  imports: [DatePipe, FormsModule, PostCard],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly api = inject(BlogApiService);
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild('imageInput') private imageInput?: ElementRef<HTMLInputElement>;
  @ViewChild('postDialog') private postDialog?: ElementRef<HTMLDialogElement>;

  protected readonly posts = signal<Post[]>([]);
  protected readonly selectedPost = signal<Post | null>(null);
  protected readonly currentTime = signal(new Date());
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly isProcessingImage = signal(false);
  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly pendingDeleteId = signal<number | null>(null);
  protected readonly editingPostId = signal<number | null>(null);

  private readonly ownerId = signal<number | null>(null);

  protected draft: PostDraft = this.emptyDraft();

  constructor() {
    const clockTimer = globalThis.setInterval(() => this.currentTime.set(new Date()), 1000);
    this.destroyRef.onDestroy(() => globalThis.clearInterval(clockTimer));
  }

  ngOnInit(): void {
    void this.loadPosts();
  }

  protected async loadPosts(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set('');

    try {
      const [owner, apiPosts] = await Promise.all([
        firstValueFrom(this.api.ensureOwner()),
        firstValueFrom(this.api.getPosts()),
      ]);

      this.ownerId.set(owner.id);
      this.posts.set(apiPosts.map((post) => this.toPost(post)));
    } catch (error) {
      this.errorMessage.set(
        this.getErrorMessage(
          error,
          'Ne mogu se spojiti na bazu. Provjeri je li .NET API pokrenut na portu 5180.',
        ),
      );
    } finally {
      this.isLoading.set(false);
    }
  }

  protected async savePost(): Promise<void> {
    const title = this.draft.title.trim();
    const excerpt = this.draft.excerpt.trim();
    const ownerId = this.ownerId();

    if (!title || !excerpt) {
      this.errorMessage.set('Upiši naslov i tekst posta.');
      this.successMessage.set('');
      return;
    }

    if (ownerId === null) {
      this.errorMessage.set('Veza s bazom još nije spremna. Pokušaj ponovno.');
      return;
    }

    const editingId = this.editingPostId();
    const existingPost = this.posts().find((post) => post.id === editingId);
    const request: SavePostRequest = {
      adminUserId: existingPost?.adminUserId ?? ownerId,
      categoryId: existingPost?.categoryId ?? null,
      title,
      content: excerpt,
      imageUrls: [...this.draft.imageDataUrls],
    };

    this.isSaving.set(true);
    this.clearMessage();

    try {
      const savedPost =
        editingId === null
          ? await firstValueFrom(this.api.createPost(request))
          : await firstValueFrom(this.api.updatePost(editingId, request));
      const post = this.toPost(savedPost);

      if (editingId === null) {
        this.posts.update((posts) => [post, ...posts]);
        this.successMessage.set('Post je spremljen u bazu.');
      } else {
        this.posts.update((posts) =>
          posts.map((currentPost) => (currentPost.id === editingId ? post : currentPost)),
        );
        if (this.selectedPost()?.id === editingId) {
          this.selectedPost.set(post);
        }
        this.successMessage.set('Promjene su spremljene u bazu.');
      }

      this.resetComposer();
    } catch (error) {
      this.errorMessage.set(this.getErrorMessage(error, 'Post se nije mogao spremiti.'));
    } finally {
      this.isSaving.set(false);
    }
  }

  protected async removePost(id: number): Promise<void> {
    this.pendingDeleteId.set(id);
    this.clearMessage();

    try {
      await firstValueFrom(this.api.deletePost(id));
      this.posts.update((posts) => posts.filter((post) => post.id !== id));

      if (this.editingPostId() === id) {
        this.resetComposer();
      }

      if (this.selectedPost()?.id === id) {
        this.closePost();
      }

      this.successMessage.set('Post je uklonjen iz baze.');
    } catch (error) {
      this.errorMessage.set(this.getErrorMessage(error, 'Post se nije mogao ukloniti.'));
    } finally {
      this.pendingDeleteId.set(null);
    }
  }

  protected startEditing(post: Post): void {
    this.editingPostId.set(post.id);
    this.draft = {
      title: post.title,
      excerpt: post.excerpt,
      imageDataUrls: [...post.imageDataUrls],
    };
    this.clearMessage();

    if (this.imageInput) {
      this.imageInput.nativeElement.value = '';
    }

    globalThis.requestAnimationFrame(() => {
      document.getElementById('novi-post')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  protected cancelEditing(): void {
    this.resetComposer();
    this.clearMessage();
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
    this.draft.imageDataUrls = this.draft.imageDataUrls.filter(
      (_, imageIndex) => imageIndex !== index,
    );
    this.clearMessage();
  }

  protected clearMessage(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  private resetComposer(): void {
    this.editingPostId.set(null);
    this.draft = this.emptyDraft();

    if (this.imageInput) {
      this.imageInput.nativeElement.value = '';
    }
  }

  private emptyDraft(): PostDraft {
    return {
      title: '',
      excerpt: '',
      imageDataUrls: [],
    };
  }

  private toPost(post: ApiPost): Post {
    return {
      id: post.id,
      adminUserId: post.adminUserId,
      categoryId: post.categoryId,
      title: post.title,
      excerpt: post.content,
      publishedAt: post.createdAt,
      readingTime: this.calculateReadingTime(`${post.title} ${post.content}`),
      accent: this.accentForId(post.id),
      imageDataUrls: post.imageUrls,
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

  private accentForId(id: number): Post['accent'] {
    const accents: Post['accent'][] = ['coral', 'sage', 'sky', 'sand'];
    return accents[Math.abs(id) % accents.length];
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse && typeof error.error?.message === 'string') {
      return error.error.message;
    }

    return fallback;
  }
}
