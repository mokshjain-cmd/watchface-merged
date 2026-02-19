// TypeScript equivalent of DragSingleDigit.cs
import { DragBindBase, DragDataBase } from './DragDataBase';
import { DataItemTypeHelper } from '../models/DataItemType';

export class DragSingleDigit extends DragDataBase {
  public setDateTime?: Date;
  public dateTimeFormat?: string;
  public currentNum?: number = 0;
  public imageSource?: string[] = [];

  constructor() {
    super();
  }

  public setSize(): void {
    if (this.imageSource && this.imageSource.length > 0) {
      const imgPath = this.imageSource[0];
      if (imgPath) {
        // In web environment, we can use Image to get dimensions
        const img = new Image();
        img.onload = () => {
          this.width = img.naturalWidth;
          this.height = img.naturalHeight;
        };
        img.src = imgPath;
      }
    }
  }

  public loadImages(): void {
    if (!this.dateTimeFormat) return;

    let index: number;
    
    if (this.setDateTime) {
      // Time-based component
      let num: number;
      
      if (this.dateTimeFormat.includes('Hour')) {
        num = this.setDateTime.getHours();
      } else if (this.dateTimeFormat.includes('Year')) {
        num = this.setDateTime.getFullYear();
      } else if (this.dateTimeFormat.includes('Minute')) {
        num = this.setDateTime.getMinutes();
      } else if (this.dateTimeFormat.includes('Second')) {
        num = this.setDateTime.getSeconds();
      } else if (this.dateTimeFormat.includes('Month')) {
        num = this.setDateTime.getMonth() + 1; // JavaScript months are 0-based
      } else if (this.dateTimeFormat.includes('Day')) {
        num = this.setDateTime.getDate();
      } else {
        throw new Error('Unknown type');
      }
      
      index = this.getSingleNum(num, this.dateTimeFormat);
    } else {
      // Number-based component
      index = this.getSingleNum(this.currentNum || 0, this.dateTimeFormat);
    }

    // In web environment, this would update the visual representation
    this.currentImageIndex = index;
  }

  private getSingleNum(num: number, dateTimeFormat: string): number {
    if (dateTimeFormat.includes('Ten-Thousands')) {
      return Math.floor(num / 10000);
    } else if (dateTimeFormat.includes('Thousands')) {
      return Math.floor(num / 1000) % 10;
    } else if (dateTimeFormat.includes('Hundreds')) {
      return Math.floor(num / 100) % 10;
    } else if (dateTimeFormat.includes('Tens')) {
      return Math.floor(num / 10) % 10;
    } else if (dateTimeFormat.includes('Units') || dateTimeFormat.includes('Ones')) {
      return num % 10;
    } else {
      throw new Error('Unknown error');
    }
  }

  // Property for current image index
  public currentImageIndex: number = 0;

  public getAllImages(): (string | undefined)[] {
    return this.imageSource || [];
  }

  public getOutXml(): any {
    return {
      SingleDigit: {
        '@_DateTimeFormat': this.dateTimeFormat,
        '@_CurrentNum': this.currentNum,
        Images: this.imageSource
      }
    };
  }
}

export class DragBindSingleDigit extends DragBindBase {
  public dateTimeFormat?: string;
  public setDateTime?: Date;
  public currentNum?: number = 0;
  public imageSource?: string[] = [];

  constructor() {
    super();
  }

  public getAllImages(): (string | undefined)[] {
    return this.imageSource || [];
  }

  public getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    // Create image array
    const array = {
      name: this.generateElementName(),
      images: this.imageSource?.map(src => ({ src })) || []
    };
    outXml.imageArrays.push(array);

    // Create data item
    const dataItem = {
      name: this.generateElementName(),
      source: DataItemTypeHelper.DataItemTypes[this.itemName || '']?.toString() || '',
      ref: `@${array.name}`
    };
    outXml.dataItemImageValues.push(dataItem);

    // Create layout
    const layout = {
      ref: `@${dataItem.name}`,
      x: Math.floor(this.left || 0),
      y: Math.floor(this.top || 0)
    };
    outXml.layout = layout;

    return outXml;
  }

  private generateElementName(): string {
    return `element_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  // Property setters with change notification
  public setDateTimeFormat(value: string): void {
    this.dateTimeFormat = value;
    this.notifyPropertyChanged('dateTimeFormat');
  }

  public setSetDateTime(value: Date): void {
    this.setDateTime = value;
    this.notifyPropertyChanged('setDateTime');
  }

  public setCurrentNum(value: number): void {
    this.currentNum = value;
    this.notifyPropertyChanged('currentNum');
  }

  public setImageSource(value: string[]): void {
    this.imageSource = value;
    this.notifyPropertyChanged('imageSource');
  }
}

// Utility converter class equivalent
export class IsShowConverter {
  public static convert(value: any): 'visible' | 'hidden' {
    return value == null ? 'hidden' : 'visible';
  }
}