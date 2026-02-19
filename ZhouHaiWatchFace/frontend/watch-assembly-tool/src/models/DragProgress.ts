import { DragImageSource } from './DragImageSource';
import { DragBindBase } from './DragDataBase';

/**
 * Progress display component equivalent to C# DragProgress
 * Handles progress bars with various fill types and target values
 */
export class DragProgress extends DragImageSource {
  public maxNum: number = 0;
  public currentNum: number = 0;
  public ratioNum: number = 0;
  public targetValue: number = 0;
  public fillType: boolean = true; // true = normal fill, false = number fill
  public unitSource?: string = '';
  
  private singleWidth: number = 0;

  constructor(maxNum: number = 0) {
    super();
    this.maxNum = maxNum;
  }

  /**
   * Load and display progress images
   */
  public loadImages(): void {
    this.updateProgressDisplay();
  }

  /**
   * Set component size based on content
   */
  public setSize(): void {
    if (this.imageSource && this.imageSource.length > 0) {
      // Get dimensions from first image
      const firstImage = this.imageSource[0];
      if (firstImage) {
        // In web environment, would get actual image dimensions
        this.singleWidth = 30; // Estimated width
        this.height = 40; // Estimated height
        
        if (this.fillType) {
          // Normal fill - width based on single image
          this.width = this.singleWidth;
        } else {
          // Number fill - width based on max digits
          const maxDigits = this.maxNum.toString().length;
          this.width = this.singleWidth * maxDigits;
          
          // Add space for unit icon
          if (this.unitSource) {
            this.width += this.singleWidth;
          }
        }
      }
    }
  }

  /**
   * Update the progress display
   */
  private updateProgressDisplay(): void {
    if (!this.imageSource || this.imageSource.length === 0) return;

    if (this.targetValue === 0) {
      // Show first image only
      console.log(`Progress: showing default image`);
      this.setSize();
      return;
    }

    if (this.fillType) {
      // Normal fill mode
      this.updateNormalFill();
    } else {
      // Number fill mode
      this.updateNumberFill();
    }
    
    this.setSize();
  }

  /**
   * Update normal fill progress
   */
  private updateNormalFill(): void {
    const count = this.imageSource!.length - 1;
    const per = this.targetValue / count;
    const idx = this.currentNum / per;
    const roundedIdx = Math.floor(idx);
    
    const imageIndex = Math.min(roundedIdx, this.imageSource!.length - 1);
    console.log(`Progress: normal fill, showing image index ${imageIndex}`);
  }

  /**
   * Update number fill progress
   */
  private updateNumberFill(): void {
    let displayNum = this.currentNum;
    if (displayNum > this.maxNum) {
      displayNum = this.maxNum;
    }

    const numStr = displayNum.toString();
    console.log(`Progress: number fill, showing digits: ${numStr}`);
  }

  /**
   * Set current progress value
   */
  public setCurrentNum(value: number): void {
    this.currentNum = Math.max(0, Math.min(this.maxNum, value));
    this.loadImages();
    this.notifyPropertyChanged('currentNum');
  }

  /**
   * Set target value for progress calculation
   */
  public setTargetValue(value: number): void {
    this.targetValue = value;
    this.loadImages();
    this.notifyPropertyChanged('targetValue');
  }

  /**
   * Set fill type
   */
  public setFillType(normal: boolean): void {
    this.fillType = normal;
    this.loadImages();
    this.notifyPropertyChanged('fillType');
  }

  /**
   * Get progress percentage
   */
  public getProgressPercentage(): number {
    if (this.targetValue === 0) return 0;
    return Math.min(100, (this.currentNum / this.targetValue) * 100);
  }

  /**
   * Get all images including unit
   */
  public getAllImages(): (string | undefined)[] {
    const images = super.getAllImages();
    
    if (this.unitSource) images.push(this.unitSource);
    
    return images;
  }

  /**
   * Get XML output for progress component
   */
  public getOutXml(): any {
    return {
      Progress: {
        '@_MaxNum': this.maxNum,
        '@_CurrentNum': this.currentNum,
        '@_RatioNum': this.ratioNum,
        '@_TargetValue': this.targetValue,
        '@_FillType': this.fillType,
        Images: this.imageSource,
        UnitSource: this.unitSource
      }
    };
  }

  /**
   * Get current progress image index
   */
  public getCurrentImageIndex(): number {
    if (!this.imageSource || this.imageSource.length === 0) return 0;
    
    if (this.targetValue === 0) return 0;
    
    if (this.fillType) {
      const count = this.imageSource.length - 1;
      const per = this.targetValue / count;
      const idx = this.currentNum / per;
      return Math.min(Math.floor(idx), this.imageSource.length - 1);
    }
    
    return 0;
  }

  /**
   * Check if progress is complete
   */
  public isComplete(): boolean {
    return this.currentNum >= this.targetValue;
  }

  /**
   * Update progress from percentage
   */
  public setFromPercentage(percentage: number): void {
    const clampedPercentage = Math.max(0, Math.min(100, percentage));
    const value = (this.targetValue * clampedPercentage) / 100;
    this.setCurrentNum(Math.round(value));
  }
}

/**
 * Drag bind progress component
 */
export class DragBindProgress extends DragBindBase {
  public maxNum: number = 0;
  public minNum: number = 0;
  public currentNum: number = 0;
  public ratioNum: number = 0;
  public targetValue: number = 0;
  public fillType: boolean = true;
  public unitSource?: string;

  constructor(maxNum: number = 0, minNum: number = 0, defaultNum: number = 0) {
    super();
    this.maxNum = maxNum;
    this.minNum = minNum;
    this.currentNum = defaultNum;
    this.defaultNum = defaultNum;
  }

  /**
   * Get all images including unit
   */
  public getAllImages(): (string | undefined)[] {
    const images = [...(this.imageSource || [])];
    
    if (this.unitSource) images.push(this.unitSource);
    
    return images;
  }

  /**
   * Get XML output
   */
  public getOutXml(): any {
    return {
      Progress: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_MaxNum': this.maxNum,
        '@_MinNum': this.minNum,
        '@_CurrentNum': this.currentNum,
        '@_RatioNum': this.ratioNum,
        '@_TargetValue': this.targetValue,
        '@_FillType': this.fillType,
        Images: this.imageSource?.map((src, index) => ({
          '@_Index': index,
          '@_Path': src
        })),
        UnitSource: this.unitSource ? { '@_Path': this.unitSource } : undefined
      }
    };
  }

  /**
   * Set current progress value with validation
   */
  public setCurrentNum(value: number): void {
    this.currentNum = Math.max(this.minNum, Math.min(this.maxNum, value));
    this.notifyPropertyChanged('currentNum');
  }

  /**
   * Set target value for progress calculation
   */
  public setTargetValue(value: number): void {
    this.targetValue = Math.max(0, value);
    this.notifyPropertyChanged('targetValue');
  }

  /**
   * Get progress percentage
   */
  public getProgressPercentage(): number {
    if (this.targetValue === 0) return 0;
    return Math.min(100, (this.currentNum / this.targetValue) * 100);
  }

  /**
   * Set progress from percentage
   */
  public setFromPercentage(percentage: number): void {
    const clampedPercentage = Math.max(0, Math.min(100, percentage));
    const value = this.minNum + ((this.targetValue - this.minNum) * clampedPercentage / 100);
    this.setCurrentNum(Math.round(value));
  }

  /**
   * Get current image index for progress display
   */
  public getCurrentImageIndex(): number {
    if (!this.imageSource || this.imageSource.length === 0) return 0;
    
    if (this.targetValue === 0) return 0;
    
    if (this.fillType) {
      // Normal fill - calculate based on progress
      const count = this.imageSource.length - 1;
      const per = this.targetValue / count;
      const idx = this.currentNum / per;
      return Math.min(Math.floor(idx), this.imageSource.length - 1);
    } else {
      // Number fill - use current number directly
      return Math.min(this.currentNum, this.imageSource.length - 1);
    }
  }

  /**
   * Check if progress is complete
   */
  public isComplete(): boolean {
    return this.currentNum >= this.targetValue;
  }

  /**
   * Get fill type description
   */
  public getFillTypeDescription(): string {
    return this.fillType ? 'Normal Fill' : 'Number Fill';
  }

  /**
   * Toggle fill type
   */
  public toggleFillType(): void {
    this.fillType = !this.fillType;
    this.notifyPropertyChanged('fillType');
  }

  /**
   * Increment progress
   */
  public increment(step: number = 1): void {
    this.setCurrentNum(this.currentNum + step);
  }

  /**
   * Decrement progress
   */
  public decrement(step: number = 1): void {
    this.setCurrentNum(this.currentNum - step);
  }

  /**
   * Reset to default/minimum value
   */
  public reset(): void {
    this.setCurrentNum(this.defaultNum || this.minNum);
  }
}