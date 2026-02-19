import { DragDataBase } from './DragDataBase';

/**
 * Abstract base class for image-based drag components
 * Equivalent to C# DragImageSource
 */
export abstract class DragImageSource extends DragDataBase {
  public imageSource?: string[] = [];

  constructor() {
    super();
  }

  /**
   * Load images for the component
   */
  public loadImages(): void {
    // Base implementation - derived classes can override
    this.onImageSourceChanged();
  }

  /**
   * Set component size based on images
   */
  public abstract setSize(): void;

  /**
   * Get XML output (default implementation)
   */
  public getOutXml(): any {
    return {
      ImageSource: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_Width': this.width,
        '@_Height': this.height,
        Images: this.imageSource
      }
    };
  }

  /**
   * Handle image source changes
   */
  protected onImageSourceChanged(): void {
    this.loadImages();
    this.setSize();
    this.notifyPropertyChanged('imageSource');
  }

  /**
   * Get all images used by this component
   */
  public getAllImages(): (string | undefined)[] {
    return [...(this.imageSource || [])];
  }

  /**
   * Set image source and trigger updates
   */
  public setImageSource(images: string[]): void {
    this.imageSource = images;
    this.onImageSourceChanged();
  }

  /**
   * Add image to source
   */
  public addImage(imagePath: string): void {
    if (!this.imageSource) {
      this.imageSource = [];
    }
    this.imageSource.push(imagePath);
    this.onImageSourceChanged();
  }

  /**
   * Remove image from source
   */
  public removeImage(index: number): void {
    if (this.imageSource && index >= 0 && index < this.imageSource.length) {
      this.imageSource.splice(index, 1);
      this.onImageSourceChanged();
    }
  }

  /**
   * Clear all images
   */
  public clearImages(): void {
    this.imageSource = [];
    this.onImageSourceChanged();
  }

  /**
   * Get image count
   */
  public getImageCount(): number {
    return this.imageSource ? this.imageSource.length : 0;
  }

  /**
   * Check if has images
   */
  public hasImages(): boolean {
    return this.getImageCount() > 0;
  }

  /**
   * Get first image path
   */
  public getFirstImage(): string | undefined {
    return this.imageSource && this.imageSource.length > 0 ? this.imageSource[0] : undefined;
  }

  /**
   * Validate all image paths
   */
  public async validateImages(): Promise<boolean> {
    if (!this.imageSource) return true;
    
    for (const imagePath of this.imageSource) {
      try {
        const response = await fetch(imagePath, { method: 'HEAD' });
        if (!response.ok) {
          console.warn(`Image not found: ${imagePath}`);
          return false;
        }
      } catch (error) {
        console.warn(`Error validating image: ${imagePath}`, error);
        return false;
      }
    }
    return true;
  }

  /**
   * Get image dimensions (estimated for web)
   */
  public async getImageDimensions(imagePath: string): Promise<{ width: number; height: number }> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        resolve({ width: img.naturalWidth, height: img.naturalHeight });
      };
      img.onerror = () => {
        reject(new Error(`Failed to load image: ${imagePath}`));
      };
      img.src = imagePath;
    });
  }

  /**
   * Update component size based on first image
   */
  protected async updateSizeFromFirstImage(): Promise<void> {
    const firstImage = this.getFirstImage();
    if (firstImage) {
      try {
        const dimensions = await this.getImageDimensions(firstImage);
        this.width = dimensions.width;
        this.height = dimensions.height;
      } catch (error) {
        console.warn('Failed to get image dimensions:', error);
        // Use default dimensions
        this.width = 50;
        this.height = 50;
      }
    }
  }
}