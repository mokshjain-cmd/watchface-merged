import { DragImageSource } from './DragImageSource';
import { DragBindBase } from './DragDataBase';

/**
 * Date/Time type enumeration
 */
export enum DateTimeType {
  Month = 'Month',
  Day = 'Day',
  Hour = 'Hour',
  Minute = 'Minute',
  Second = 'Second',
  Week = 'Week'
}

/**
 * Week/Date display component equivalent to C# DragWeek
 * Handles display of date/time components using image arrays
 */
export class DragWeek extends DragImageSource {
  public setDateTime?: Date;
  public dateTimeType: DateTimeType = DateTimeType.Month;

  constructor() {
    super();
  }

  /**
   * Set component size based on first image
   */
  public setSize(): void {
    if (this.imageSource && this.imageSource.length > 0) {
      const imgPath = this.imageSource[0];
      if (imgPath) {
        // In web environment, would get actual image dimensions
        this.height = 40; // Estimated height
        this.width = 40; // Estimated width
      }
    }
  }

  /**
   * Load and display the appropriate image
   */
  public loadImages(): void {
    if (this.setDateTime && this.imageSource && this.imageSource.length > 0) {
      const index = this.getIndex(this.dateTimeType);
      if (index !== null && index >= 0 && index < this.imageSource.length) {
        const imgPath = this.imageSource[index];
        console.log(`Week/Date: showing image index ${index} for ${this.dateTimeType}`);
      }
    }
    this.setSize();
  }

  /**
   * Handle image source changes
   */
  protected onImageSourceChanged(): void {
    super.onImageSourceChanged();
    this.setSize();
    this.loadImages();
  }

  /**
   * Get index for current date/time type
   */
  public getIndex(dateTimeType: DateTimeType): number | null {
    if (!this.setDateTime) return null;

    switch (dateTimeType) {
      case DateTimeType.Month:
        return this.setDateTime.getMonth(); // 0-based
      case DateTimeType.Day:
        return this.setDateTime.getDate() - 1; // Convert to 0-based
      case DateTimeType.Hour:
        return this.setDateTime.getHours();
      case DateTimeType.Minute:
        return this.setDateTime.getMinutes();
      case DateTimeType.Second:
        return this.setDateTime.getSeconds();
      case DateTimeType.Week:
        return this.setDateTime.getDay(); // 0=Sunday, 1=Monday, etc.
      default:
        throw new Error(`Unsupported date/time type: ${dateTimeType}`);
    }
  }

  /**
   * Set the date/time and update display
   */
  public setCurrentDateTime(dateTime: Date): void {
    this.setDateTime = dateTime;
    this.loadImages();
    this.notifyPropertyChanged('setDateTime');
  }

  /**
   * Set the date/time type and update display
   */
  public setDateTimeType(type: DateTimeType): void {
    this.dateTimeType = type;
    this.loadImages();
    this.notifyPropertyChanged('dateTimeType');
  }

  /**
   * Get current display value
   */
  public getCurrentValue(): number | null {
    return this.getIndex(this.dateTimeType);
  }

  /**
   * Get current display value as string
   */
  public getCurrentDisplayString(): string {
    if (!this.setDateTime) return '';

    switch (this.dateTimeType) {
      case DateTimeType.Month:
        return this.setDateTime.toLocaleDateString('en-US', { month: 'long' });
      case DateTimeType.Day:
        return this.setDateTime.getDate().toString();
      case DateTimeType.Hour:
        return this.setDateTime.getHours().toString();
      case DateTimeType.Minute:
        return this.setDateTime.getMinutes().toString().padStart(2, '0');
      case DateTimeType.Second:
        return this.setDateTime.getSeconds().toString().padStart(2, '0');
      case DateTimeType.Week:
        return this.setDateTime.toLocaleDateString('en-US', { weekday: 'long' });
      default:
        return '';
    }
  }

  /**
   * Get XML output for week/date component
   */
  public getOutXml(): any {
    return {
      Week: {
        '@_DateTimeType': this.dateTimeType,
        '@_Value': this.getCurrentValue(),
        Images: this.imageSource
      }
    };
  }
}

/**
 * Drag bind week component
 */
export class DragBindWeek extends DragBindBase {
  public setDateTime?: Date;
  public dateTimeType: DateTimeType = DateTimeType.Week;

  constructor() {
    super();
  }

  /**
   * Get all images
   */
  public getAllImages(): (string | undefined)[] {
    return [...(this.imageSource || [])];
  }

  /**
   * Get XML output
   */
  public getOutXml(): any {
    const index = this.getIndex(this.dateTimeType);
    
    return {
      Week: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_DateTimeType': this.dateTimeType,
        '@_Index': index,
        Images: this.imageSource?.map((src, idx) => ({
          '@_Index': idx,
          '@_Path': src
        }))
      }
    };
  }

  /**
   * Get index for current date/time type
   */
  public getIndex(dateTimeType: DateTimeType): number | null {
    if (!this.setDateTime) return null;

    switch (dateTimeType) {
      case DateTimeType.Month:
        return this.setDateTime.getMonth(); // 0-based
      case DateTimeType.Day:
        return this.setDateTime.getDate() - 1; // Convert to 0-based
      case DateTimeType.Hour:
        return this.setDateTime.getHours();
      case DateTimeType.Minute:
        return this.setDateTime.getMinutes();
      case DateTimeType.Second:
        return this.setDateTime.getSeconds();
      case DateTimeType.Week:
        return this.setDateTime.getDay(); // 0=Sunday, 1=Monday, etc.
      default:
        return null;
    }
  }

  /**
   * Set current date/time
   */
  public setCurrentDateTime(dateTime: Date): void {
    this.setDateTime = dateTime;
    this.notifyPropertyChanged('setDateTime');
  }

  /**
   * Set date/time type
   */
  public setDateTimeType(type: DateTimeType): void {
    this.dateTimeType = type;
    this.notifyPropertyChanged('dateTimeType');
  }

  /**
   * Get current value based on type
   */
  public getCurrentValue(): number | null {
    return this.getIndex(this.dateTimeType);
  }

  /**
   * Get display string for current value
   */
  public getDisplayString(): string {
    if (!this.setDateTime) return '';

    switch (this.dateTimeType) {
      case DateTimeType.Month:
        return this.setDateTime.toLocaleDateString('en-US', { month: 'short' });
      case DateTimeType.Day:
        return this.setDateTime.getDate().toString();
      case DateTimeType.Hour:
        return this.setDateTime.getHours().toString();
      case DateTimeType.Minute:
        return this.setDateTime.getMinutes().toString().padStart(2, '0');
      case DateTimeType.Second:
        return this.setDateTime.getSeconds().toString().padStart(2, '0');
      case DateTimeType.Week:
        return this.setDateTime.toLocaleDateString('en-US', { weekday: 'short' });
      default:
        return '';
    }
  }

  /**
   * Get week day names (for Week type)
   */
  public static getWeekDayNames(format: 'long' | 'short' = 'long'): string[] {
    const date = new Date();
    const names: string[] = [];
    
    // Start from Sunday (0) to Saturday (6)
    for (let i = 0; i < 7; i++) {
      date.setDate(date.getDate() - date.getDay() + i);
      names.push(date.toLocaleDateString('en-US', { weekday: format }));
    }
    
    return names;
  }

  /**
   * Get month names (for Month type)
   */
  public static getMonthNames(format: 'long' | 'short' = 'long'): string[] {
    const names: string[] = [];
    
    for (let i = 0; i < 12; i++) {
      const date = new Date(2000, i, 1);
      names.push(date.toLocaleDateString('en-US', { month: format }));
    }
    
    return names;
  }

  /**
   * Check if current value is valid for the date/time type
   */
  public isValidValue(): boolean {
    const value = this.getCurrentValue();
    if (value === null) return false;

    switch (this.dateTimeType) {
      case DateTimeType.Month:
        return value >= 0 && value <= 11;
      case DateTimeType.Day:
        return value >= 0 && value <= 30; // Simplified check
      case DateTimeType.Hour:
        return value >= 0 && value <= 23;
      case DateTimeType.Minute:
      case DateTimeType.Second:
        return value >= 0 && value <= 59;
      case DateTimeType.Week:
        return value >= 0 && value <= 6;
      default:
        return false;
    }
  }
}