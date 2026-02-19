import { DragImageSource } from './DragImageSource';
import { DragBindBase } from './DragDataBase';

/**
 * Numeric display component equivalent to C# DragNums
 * Handles display of numbers with support for units, minus signs, and empty states
 */
export class DragNums extends DragImageSource {
  public maxNum: number = 0;
  public currentNum: number = 0;
  public unitSource?: string = '';
  public minusSource?: string = '';
  public emptySource?: string = '';
  
  private singleWidth: number = 0;

  constructor(maxNum: number = 0) {
    super();
    this.maxNum = maxNum;
  }

  /**
   * Load and display numeric images
   */
  public loadImages(): void {
    // In web environment, this would update the display
    this.updateNumericDisplay();
  }

  /**
   * Set component size based on content
   */
  public setSize(): void {
    if (this.imageSource && this.imageSource.length > 0) {
      // Calculate size based on current number
      const displayNum = Math.min(this.currentNum, this.maxNum);
      const numStr = displayNum.toString();
      
      // Get dimensions from first image
      const firstImage = this.imageSource[0];
      if (firstImage) {
        // In web environment, would get actual image dimensions
        this.singleWidth = 25; // Estimated width
        this.height = 40; // Estimated height
        
        // Calculate total width based on digit count
        let totalWidth = this.singleWidth * numStr.length;
        
        // Add space for minus sign if negative
        if (this.currentNum < 0 && this.minusSource) {
          totalWidth += this.singleWidth;
        }
        
        // Add space for unit icon
        if (this.unitSource) {
          totalWidth += this.singleWidth;
        }
        
        this.width = totalWidth;
      }
    }
  }

  /**
   * Update the numeric display
   */
  private updateNumericDisplay(): void {
    let displayNum = this.currentNum;
    if (displayNum > this.maxNum) {
      displayNum = this.maxNum;
    }

    const numStr = Math.abs(displayNum).toString();
    const isNegative = this.currentNum < 0;

    // This would update the actual display in a web environment
    console.log(`Displaying number: ${isNegative ? '-' : ''}${numStr}`);
    
    this.setSize();
  }

  /**
   * Set current number and update display
   */
  public setCurrentNum(value: number): void {
    this.currentNum = value;
    this.loadImages();
    this.notifyPropertyChanged('currentNum');
  }

  /**
   * Set maximum number
   */
  public setMaxNum(value: number): void {
    this.maxNum = value;
    this.loadImages();
    this.notifyPropertyChanged('maxNum');
  }

  /**
   * Get all images including unit and minus
   */
  public getAllImages(): (string | undefined)[] {
    const images = super.getAllImages();
    
    if (this.unitSource) images.push(this.unitSource);
    if (this.minusSource) images.push(this.minusSource);
    if (this.emptySource) images.push(this.emptySource);
    
    return images;
  }

  /**
   * Get XML output for numeric component
   */
  public getOutXml(): any {
    return {
      Nums: {
        '@_MaxNum': this.maxNum,
        '@_CurrentNum': this.currentNum,
        Images: this.imageSource,
        UnitSource: this.unitSource,
        MinusSource: this.minusSource,
        EmptySource: this.emptySource
      }
    };
  }

  /**
   * Get digit images for current number
   */
  public getDigitImages(): string[] {
    const result: string[] = [];
    const displayNum = Math.min(Math.abs(this.currentNum), this.maxNum);
    const numStr = displayNum.toString();

    for (const char of numStr) {
      if (char >= '0' && char <= '9') {
        const digitIndex = parseInt(char);
        if (this.imageSource && digitIndex < this.imageSource.length) {
          result.push(this.imageSource[digitIndex]);
        }
      }
    }

    return result;
  }

  /**
   * Check if number is in valid range
   */
  public isValidNumber(): boolean {
    return this.currentNum >= 0 && this.currentNum <= this.maxNum;
  }

  /**
   * Increment current number
   */
  public increment(): void {
    if (this.currentNum < this.maxNum) {
      this.setCurrentNum(this.currentNum + 1);
    }
  }

  /**
   * Decrement current number
   */
  public decrement(): void {
    if (this.currentNum > 0) {
      this.setCurrentNum(this.currentNum - 1);
    }
  }
}

/**
 * Drag bind numeric component
 */
export class DragBindNums extends DragBindBase {
  public maxNum: number = 0;
  public minNum: number = 0;
  public currentNum: number = 0;
  public unitSource?: string;
  public minusSource?: string;
  public emptySource?: string;

  constructor(maxNum: number = 0, minNum: number = 0, defaultNum: number = 0) {
    super();
    this.maxNum = maxNum;
    this.minNum = minNum;
    this.currentNum = defaultNum;
    this.defaultNum = defaultNum;
  }

  /**
   * Get all images including unit and minus
   */
  public getAllImages(): (string | undefined)[] {
    const images = [...(this.imageSource || [])];
    
    if (this.unitSource) images.push(this.unitSource);
    if (this.minusSource) images.push(this.minusSource);
    if (this.emptySource) images.push(this.emptySource);
    
    return images;
  }

  /**
   * Get XML output
   */
  public getOutXml(): any {
    return {
      Nums: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_MaxNum': this.maxNum,
        '@_MinNum': this.minNum,
        '@_CurrentNum': this.currentNum,
        Images: this.imageSource?.map((src, index) => ({
          '@_Index': index,
          '@_Path': src
        })),
        UnitSource: this.unitSource ? { '@_Path': this.unitSource } : undefined,
        MinusSource: this.minusSource ? { '@_Path': this.minusSource } : undefined,
        EmptySource: this.emptySource ? { '@_Path': this.emptySource } : undefined
      }
    };
  }

  /**
   * Set current number with validation
   */
  public setCurrentNum(value: number): void {
    // Clamp value between min and max
    this.currentNum = Math.max(this.minNum, Math.min(this.maxNum, value));
    this.notifyPropertyChanged('currentNum');
  }

  /**
   * Get current value as percentage (for progress components)
   */
  public getPercentage(): number {
    if (this.maxNum <= this.minNum) return 0;
    return ((this.currentNum - this.minNum) / (this.maxNum - this.minNum)) * 100;
  }

  /**
   * Set value from percentage
   */
  public setFromPercentage(percentage: number): void {
    const clampedPercentage = Math.max(0, Math.min(100, percentage));
    const value = this.minNum + ((this.maxNum - this.minNum) * clampedPercentage / 100);
    this.setCurrentNum(Math.round(value));
  }

  /**
   * Get digit count for current number
   */
  public getDigitCount(): number {
    return Math.abs(this.currentNum).toString().length;
  }

  /**
   * Check if current number is negative
   */
  public isNegative(): boolean {
    return this.currentNum < 0;
  }

  /**
   * Get formatted display string
   */
  public getDisplayString(): string {
    let result = Math.abs(this.currentNum).toString();
    
    if (this.isNegative() && this.minusSource) {
      result = '-' + result;
    }
    
    return result;
  }

  /**
   * Reset to default value
   */
  public resetToDefault(): void {
    this.setCurrentNum(this.defaultNum || this.minNum);
  }

  /**
   * Increment with bounds checking
   */
  public increment(step: number = 1): void {
    this.setCurrentNum(this.currentNum + step);
  }

  /**
   * Decrement with bounds checking
   */
  public decrement(step: number = 1): void {
    this.setCurrentNum(this.currentNum - step);
  }
}