// TypeScript equivalent of DragDataBase.cs
import { DataItemTypeHelper } from '../models/DataItemType';

export enum BindMonitorType {
  Time = 0,
  BloodOxygen = 1,
  Battery = 2,
  Steps = 3,
  HeartRate = 4,
  Calories = 5,
  Distance = 6,
  Sleep = 7,
  Weather = 8
}

export enum Align {
  left = 0,
  center = 1,
  right = 2
}

export interface IDraggable {
  dragId?: string;
  dragName?: string;
  width?: number;
  height?: number;
  left?: number;
  top?: number;
  visible?: boolean;
}

export abstract class DragDataBase implements IDraggable {
  public dragId?: string;
  public dragName?: string;
  public width: number = 0;
  public height: number = 0;
  public x: number = 0;  // left position
  public y: number = 0;  // top position
  public visible?: boolean = true;
  public elementType?: BindMonitorType;

  protected draggableBehavior?: any; // DraggableBehavior equivalent
  private propertyChangeHandlers: Map<string, Function[]> = new Map();

  constructor() {
    this.dragId = this.generateGuid();
  }

  protected generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  /**
   * Property change notification system
   */
  protected notifyPropertyChanged(propertyName: string): void {
    const handlers = this.propertyChangeHandlers.get(propertyName);
    if (handlers) {
      handlers.forEach(handler => handler(this, propertyName));
    }
    
    // Also notify global handlers
    const globalHandlers = this.propertyChangeHandlers.get('*');
    if (globalHandlers) {
      globalHandlers.forEach(handler => handler(this, propertyName));
    }
  }

  /**
   * Add property change handler
   */
  public addPropertyChangeHandler(propertyName: string, handler: Function): void {
    if (!this.propertyChangeHandlers.has(propertyName)) {
      this.propertyChangeHandlers.set(propertyName, []);
    }
    this.propertyChangeHandlers.get(propertyName)!.push(handler);
  }

  /**
   * Remove property change handler
   */
  public removePropertyChangeHandler(propertyName: string, handler: Function): void {
    const handlers = this.propertyChangeHandlers.get(propertyName);
    if (handlers) {
      const index = handlers.indexOf(handler);
      if (index > -1) {
        handlers.splice(index, 1);
      }
    }
  }

  public abstract setSize(): void;
  public abstract loadImages(): void;
  public abstract getOutXml(): any; // DragDataBaseXml equivalent
  public abstract getAllImages(): (string | undefined)[];

  protected onVisibilityChanged(): void {
    // Handle visibility change logic
    if (this.visible) {
      this.setSize();
      this.loadImages();
    }
  }
}

export abstract class DragBindBase {
  public dragName?: string;
  public width?: number = 0;
  public height?: number = 0;
  public left?: number;
  public top?: number;
  public x?: number; // Alternative to left
  public y?: number; // Alternative to top
  public align: Align = Align.left;
  public elementType?: BindMonitorType;
  public dragId?: string;
  public maxNum: number = 0;
  public minNum: number = 0;
  public subItems?: DragBindBase[] = [];
  public visible?: boolean = true;
  public itemName?: string;
  public defaultNum?: number;
  public imageSource?: string[] = [];
  
  private propertyChangeHandlers: Map<string, Function[]> = new Map();

  constructor() {
    this.dragId = this.generateGuid();
    this.subItems = [];
    this.visible = true;
  }

  protected generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  public get verifyNullNum(): boolean {
    return DataItemTypeHelper.VerifyNullNum.includes(this.itemName || '');
  }

  public abstract getAllImages(): (string | undefined)[];
  public abstract getOutXml(): any; // DragDataBaseXml equivalent

  public getLeft(): number {
    if (this.align === Align.center) {
      return Math.floor((this.left || 0) - ((this.width || 0) / 2));
    } else if (this.align === Align.right) {
      return Math.floor((this.left || 0) - (this.width || 0));
    }
    return Math.floor(this.left || 0);
  }

  // Property change notification system
  protected propertyChangeListeners: ((propertyName: string) => void)[] = [];

  public addPropertyChangeListener(callback: (propertyName: string) => void): void {
    this.propertyChangeListeners.push(callback);
  }

  public removePropertyChangeListener(callback: (propertyName: string) => void): void {
    const index = this.propertyChangeListeners.indexOf(callback);
    if (index > -1) {
      this.propertyChangeListeners.splice(index, 1);
    }
  }

  /**
   * Notify property changed (unified with DragDataBase version)
   */
  protected notifyPropertyChanged(propertyName: string): void {
    // For DragBindBase compatibility
    this.propertyChangeListeners.forEach(listener => listener(propertyName));
    
    // For DragDataBase compatibility if handlers are set
    const handlers = this.propertyChangeHandlers?.get(propertyName);
    if (handlers) {
      handlers.forEach(handler => handler(this, propertyName));
    }
  }

  // Reactive property setters
  public setWidth(value: number): void {
    this.width = value;
    this.notifyPropertyChanged('width');
  }

  public setHeight(value: number): void {
    this.height = value;
    this.notifyPropertyChanged('height');
  }

  public setLeft(value: number): void {
    this.left = value;
    this.notifyPropertyChanged('left');
  }

  public setTop(value: number): void {
    this.top = value;
    this.notifyPropertyChanged('top');
  }

  public setVisible(value: boolean): void {
    this.visible = value;
    this.notifyPropertyChanged('visible');
  }
}