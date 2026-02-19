// import { DragImageSource } from './DragImageSource';
import { DragBindBase, DragDataBase } from './DragDataBase';
import { DragDateTimeHelper, MonthDayModeEnum } from './DragDateTimeHelper';

/**
 * Month-Day component equivalent to C# DragMonthDay
 * Handles month/day display with various formatting options
 */
export class DragMonthDay extends DragDataBase {
  // Properties
  public leadingZero?: boolean = false;
  public setDateTime?: Date;
  public unitIcon?: string = '';
  public monthIcon?: string = '';
  public dayIcon?: string = '';
  public monthDayMode: MonthDayModeEnum = MonthDayModeEnum.split;
  public imageSource?: string[] = [];

  private singleWidth: number = 0;

  constructor() {
    super();
  }

  /**
   * Sign table for icon mapping
   */
  public get signTable(): { [key: string]: string | undefined } {
    return {
      [DragDateTimeHelper.iconStr]: this.unitIcon,
      [DragDateTimeHelper.monthStr]: this.monthIcon,
      [DragDateTimeHelper.dayStr]: this.dayIcon
    };
  }

  /**
   * Set size based on icon path
   */
  private setSizeByIcon(iconPath?: string): void {
    if (iconPath) {
      // In web environment, we'll estimate or get actual image dimensions
      // For now, using default dimensions
      const estimatedWidth = 20; // Would be actual image width
      const estimatedHeight = 30; // Would be actual image height
      
      this.width = (this.width || 0) + estimatedWidth;
      if (estimatedHeight > (this.height || 0)) {
        this.height = estimatedHeight;
      }
    }
  }

  /**
   * Override setSize to calculate component dimensions
   */
  public setSize(): void {
    if (this.imageSource && this.imageSource.length > 0) {
      const dateTime = this.setDateTime || new Date();
      const monthDayStr = DragDateTimeHelper.getMonthDay(
        this.leadingZero || false,
        this.monthDayMode,
        !!this.unitIcon,
        !!this.monthIcon,
        !!this.dayIcon,
        dateTime
      );

      const imgPath = this.imageSource[0];
      if (imgPath) {
        // In web environment, we would get actual image dimensions
        // For now, using estimated dimensions
        this.singleWidth = 25; // Would be actual bitmap width
        this.height = 40; // Would be actual bitmap height

        // Calculate width based on number of digits
        const digitCount = monthDayStr
          .replace(new RegExp(DragDateTimeHelper.iconStr, 'g'), '')
          .replace(new RegExp(DragDateTimeHelper.monthStr, 'g'), '')
          .replace(new RegExp(DragDateTimeHelper.dayStr, 'g'), '')
          .length;

        this.width = this.singleWidth * digitCount;

        // Add icon sizes
        if (this.monthDayMode === MonthDayModeEnum.split) {
          this.setSizeByIcon(this.unitIcon);
        } else if (this.monthDayMode === MonthDayModeEnum.unit) {
          this.setSizeByIcon(this.monthIcon);
          this.setSizeByIcon(this.dayIcon);
        }
      }
    }
  }

  /**
   * Load images for the component
   */
  public loadImages(): void {
    // Base implementation for loading images
    this.setSize();
  }

  /**
   * Get formatted string for current date/time
   */
  public getFormattedString(): string {
    const dateTime = this.setDateTime || new Date();
    return DragDateTimeHelper.getMonthDay(
      this.leadingZero || false,
      this.monthDayMode,
      !!this.unitIcon,
      !!this.monthIcon,
      !!this.dayIcon,
      dateTime
    );
  }

  /**
   * Get all images including icons
   */
  public getAllImages(): (string | undefined)[] {
    const images = [...(this.imageSource || [])];
    
    if (this.unitIcon) images.push(this.unitIcon);
    if (this.monthIcon) images.push(this.monthIcon);
    if (this.dayIcon) images.push(this.dayIcon);
    
    return images;
  }

  /**
   * Get XML output for month-day component
   */
  public getOutXml(): any {
    return {
      MonthDay: {
        '@_LeadingZero': this.leadingZero,
        '@_Mode': this.monthDayMode,
        UnitIcon: this.unitIcon,
        MonthIcon: this.monthIcon,
        DayIcon: this.dayIcon,
        Images: this.imageSource
      }
    };
  }
}

/**
 * Drag bind month-day component
 */
export class DragBindMonthDay extends DragBindBase {
  public leadingZero?: boolean = true;
  public monthDayMode?: MonthDayModeEnum = MonthDayModeEnum.split;
  public unitIcon?: string;
  public monthIcon?: string;
  public dayIcon?: string;
  public setDateTime?: Date;

  constructor() {
    super();
  }

  /**
   * Get all images including icons
   */
  public override getAllImages(): (string | undefined)[] {
    const images = [...(this.imageSource || [])];
    
    if (this.unitIcon) images.push(this.unitIcon);
    if (this.monthIcon) images.push(this.monthIcon);
    if (this.dayIcon) images.push(this.dayIcon);
    
    return images;
  }

  /**
   * Get XML output
   */
  public override getOutXml(): any {
    const dateTime = this.setDateTime || new Date();
    const monthDayStr = DragDateTimeHelper.getMonthDay(
      this.leadingZero || false,
      this.monthDayMode || MonthDayModeEnum.split,
      !!this.unitIcon,
      !!this.monthIcon,
      !!this.dayIcon,
      dateTime
    );

    return {
      MonthDay: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_LeadingZero': this.leadingZero,
        '@_Mode': this.monthDayMode,
        Images: this.imageSource?.map((src, index) => ({
          '@_Index': index,
          '@_Path': src
        })),
        UnitIcon: this.unitIcon ? { '@_Path': this.unitIcon } : undefined,
        MonthIcon: this.monthIcon ? { '@_Path': this.monthIcon } : undefined,
        DayIcon: this.dayIcon ? { '@_Path': this.dayIcon } : undefined
      }
    };
  }

  /**
   * Set the current date/time for display
   */
  public setCurrentDateTime(dateTime: Date): void {
    this.setDateTime = dateTime;
    // Trigger any necessary updates
    this.notifyPropertyChanged('setDateTime');
  }

  /**
   * Get the current formatted display value
   */
  public getCurrentValue(): string {
    const dateTime = this.setDateTime || new Date();
    return DragDateTimeHelper.getMonthDay(
      this.leadingZero || false,
      this.monthDayMode || MonthDayModeEnum.split,
      !!this.unitIcon,
      !!this.monthIcon,
      !!this.dayIcon,
      dateTime
    );
  }

  /**
   * Update month/day mode
   */
  public setMonthDayMode(mode: MonthDayModeEnum): void {
    this.monthDayMode = mode;
    this.notifyPropertyChanged('monthDayMode');
  }

  /**
   * Toggle leading zero display
   */
  public toggleLeadingZero(): void {
    this.leadingZero = !(this.leadingZero || false);
    this.notifyPropertyChanged('leadingZero');
  }
}