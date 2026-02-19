/**
 * Utility converters and helpers for web-based watch face designer
 * Equivalent to C# ConvertAll.cs
 */

import { CommonHelper } from './CommonHelper';

/**
 * Utility class for converting and processing data
 */
export class ConvertAll {
  /**
   * Convert image path to absolute path and load as HTMLImageElement
   */
  public static async convertImagePath(value: string): Promise<HTMLImageElement | null> {
    if (!value || value === '') {
      return null;
    }

    try {
      const path = CommonHelper.absolutePath(value);
      const img = new Image();

      return new Promise((resolve, reject) => {
        img.onload = () => resolve(img);
        img.onerror = () => resolve(null);
        img.src = path;
      });
    } catch {
      return null;
    }
  }

  /**
   * Get default button background path
   */
  public static getDefaultButtonBackground(): string {
    return CommonHelper.addBtnBackgroundPath;
  }

  /**
   * Convert data item name to category
   */
  public static convertItemName(value: string): string {
    if (value.includes('Steps')) {
      return 'Steps';
    }
    if (value.includes('Calories')) {
      return 'Calories';
    }
    if (value.includes('Exercise')) {
      return 'Exercise';
    }
    return 'Other';
  }

  /**
   * Check equality (equivalent to EqualityConverter)
   */
  public static equalityCheck(value: any, parameter: any): boolean {
    return value?.toString() === parameter?.toString();
  }
}

/**
 * Image loading utility for web
 */
export class ImageLoader {
  private static imageCache = new Map<string, HTMLImageElement>();

  /**
   * Load image from path with caching
   */
  public static async loadImage(path: string): Promise<HTMLImageElement | null> {
    if (this.imageCache.has(path)) {
      return this.imageCache.get(path)!;
    }

    try {
      const img = new Image();
      return new Promise((resolve, reject) => {
        img.onload = () => {
          this.imageCache.set(path, img);
          resolve(img);
        };
        img.onerror = () => resolve(null);
        img.src = path;
      });
    } catch {
      return null;
    }
  }

  /**
   * Clear image cache
   */
  public static clearCache(): void {
    this.imageCache.clear();
  }
}