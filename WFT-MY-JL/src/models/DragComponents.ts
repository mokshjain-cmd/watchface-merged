// Comprehensive TypeScript models based on C# WatchControlLibrary
import { DataItemTypeHelper, DataItemType } from '../models/DataItemType';
import { DragBindBase, BindMonitorType } from './DragDataBase';
import { DragDateTimeHelper, MonthDayModeEnum } from './DragDateTimeHelper';
import { DragBindDouble } from './DragDouble';
import { DragBindKeyValue } from './DragKeyValue';
import { DragBindPoint } from './DragPoint';
import { DragBindAnimFrame } from './DragAnimFrame';

// Normal DateTime component
export class DragBindNormalDateTime extends DragBindBase {
  public dateTimeFormat?: string;
  public setDateTime?: Date;
  public leadingZero?: boolean = false;
  public unitIcon?: string;
  public imageSource?: string[] = [];

  constructor() {
    super();
  }

  public override getAllImages(): (string | undefined)[] {
    const images: (string | undefined)[] = [...(this.imageSource || [])];
    if (this.unitIcon) {
      images.push(this.unitIcon);
    }
    return images;
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    // Create image array for digits
    const digitArray = {
      name: this.generateElementName(),
      images: this.imageSource?.map(src => ({ src })) || []
    };
    outXml.imageArrays.push(digitArray);

    // Create data item
    const dataItem = {
      name: this.generateElementName(),
      source: DataItemTypeHelper.DataItemTypes[this.itemName || '']?.toString() || '',
      ref: `@${digitArray.name}`,
      format: this.dateTimeFormat,
      leadingZero: this.leadingZero
    };
    outXml.dataItemImageValues.push(dataItem);

    // Add unit icon if present
    if (this.unitIcon) {
      const unitArray = {
        name: this.generateElementName(),
        images: [{ src: this.unitIcon }]
      };
      outXml.imageArrays.push(unitArray);
    }

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
}

// Progress component
export class DragBindProgress extends DragBindBase {
  public imageSource?: string[] = [];
  public targetValue: number = 0;

  constructor(maxNum: number = 100, minNum: number = 0, defaultNum: number = 50) {
    super();
    this.maxNum = maxNum;
    this.minNum = minNum;
    this.defaultNum = defaultNum;
  }

  public override getAllImages(): (string | undefined)[] {
    return this.imageSource || [];
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    const array = {
      name: this.generateElementName(),
      images: this.imageSource?.map(src => ({ src })) || []
    };
    outXml.imageArrays.push(array);

    const dataItem = {
      name: this.generateElementName(),
      source: DataItemTypeHelper.DataItemTypes[this.itemName || '']?.toString() || '',
      ref: `@${array.name}`,
      maxValue: this.maxNum,
      minValue: this.minNum,
      targetValue: this.targetValue
    };
    outXml.dataItemImageValues.push(dataItem);

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
}

// Numbers component
export class DragBindNums extends DragBindBase {
  public imageSource?: string[] = [];

  constructor(maxNum: number = 100, minNum: number = 0, defaultNum: number = 0) {
    super();
    this.maxNum = maxNum;
    this.minNum = minNum;
    this.defaultNum = defaultNum;
  }

  public override getAllImages(): (string | undefined)[] {
    return this.imageSource || [];
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    const array = {
      name: this.generateElementName(),
      images: this.imageSource?.map(src => ({ src })) || []
    };
    outXml.imageArrays.push(array);

    const dataItem = {
      name: this.generateElementName(),
      source: DataItemTypeHelper.DataItemTypes[this.itemName || '']?.toString() || '',
      ref: `@${array.name}`,
      maxValue: this.maxNum,
      minValue: this.minNum,
      defaultValue: this.defaultNum
    };
    outXml.dataItemImageValues.push(dataItem);

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
}

// Switch component (like AM/PM)
export class DragBindSwitch extends DragBindBase {
  public openSource?: string;
  public closeSource?: string;

  constructor() {
    super();
  }

  public override getAllImages(): (string | undefined)[] {
    return [this.openSource, this.closeSource].filter(img => img !== undefined);
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    const array = {
      name: this.generateElementName(),
      images: [
        { src: this.openSource },
        { src: this.closeSource }
      ].filter(img => img.src)
    };
    outXml.imageArrays.push(array);

    const dataItem = {
      name: this.generateElementName(),
      source: DataItemTypeHelper.DataItemTypes[this.itemName || '']?.toString() || '',
      ref: `@${array.name}`
    };
    outXml.dataItemImageValues.push(dataItem);

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
}

// AM/PM component
export class DragBindAMPM extends DragBindBase {
  public amSource?: string;
  public pmSource?: string;

  constructor() {
    super();
  }

  public override getAllImages(): (string | undefined)[] {
    return [this.amSource, this.pmSource].filter(img => img !== undefined);
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    const array = {
      name: this.generateElementName(),
      images: [
        { src: this.amSource },
        { src: this.pmSource }
      ].filter(img => img.src)
    };
    outXml.imageArrays.push(array);

    const dataItem = {
      name: this.generateElementName(),
      source: DataItemType.miscIsPM.toString(),
      ref: `@${array.name}`
    };
    outXml.dataItemImageValues.push(dataItem);

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
}

// Image component
export class DragBindImage extends DragBindBase {
  public source?: string;

  constructor() {
    super();
  }

  public override getAllImages(): (string | undefined)[] {
    return this.source ? [this.source] : [];
  }

  public override getOutXml(): any {
    const outXml = {
      imageArrays: [],
      dataItemImageValues: [],
      layout: null
    };

    if (this.source) {
      const array = {
        name: this.generateElementName(),
        images: [{ src: this.source }]
      };
      outXml.imageArrays.push(array);

      const layout = {
        ref: `@${array.name}`,
        x: Math.floor(this.left || 0),
        y: Math.floor(this.top || 0)
      };
      outXml.layout = layout;
    }

    return outXml;
  }

  private generateElementName(): string {
    return `element_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }
}

// Export all component types
export { DragBindDouble } from './DragDouble';
export { DragBindKeyValue } from './DragKeyValue';
export type { KeyValueData } from './DragKeyValue';
export { DragBindPoint } from './DragPoint';
export { DragBindAnimFrame } from './DragAnimFrame';