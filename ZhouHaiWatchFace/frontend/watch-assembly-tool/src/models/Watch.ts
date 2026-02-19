import { BindMonitorType } from './DragDataBase';

/**
 * Base class for objects that support property change notification and snapshots
 * Equivalent to C# Snapshoot
 */
export class Snapshoot {
  private static _doShutterEvent?: (propertyName: string) => void;
  private static _isRevocation: boolean = false;

  /**
   * Event for property changes (used for undo/redo system)
   */
  public static get doShutterEvent(): ((propertyName: string) => void) | undefined {
    return Snapshoot._doShutterEvent;
  }

  public static set doShutterEvent(value: ((propertyName: string) => void) | undefined) {
    Snapshoot._doShutterEvent = value;
  }

  /**
   * Whether we're currently in revocation mode (undo/redo)
   */
  public static get isRevocation(): boolean {
    return Snapshoot._isRevocation;
  }

  public static set isRevocation(value: boolean) {
    Snapshoot._isRevocation = value;
  }

  /**
   * Property change callback
   */
  protected onPropertyChanged(propertyName: string): void {
    if (!Snapshoot.isRevocation) {
      Snapshoot.doShutterEvent?.(propertyName);
    }
  }
}

/**
 * Watch model containing dimensions and components
 * Equivalent to C# Watch
 */
export class Watch extends Snapshoot {
  private _width: number = 400;
  private _height: number = 400;
  private _item?: MonitorItem;
  private _dragBindBases: any[] = [];

  constructor() {
    super();
  }

  /**
   * Watch width
   */
  public get width(): number {
    return this._width;
  }

  public set width(value: number) {
    this._width = value;
    this.onPropertyChanged('width');
  }

  /**
   * Watch height
   */
  public get height(): number {
    return this._height;
  }

  public set height(value: number) {
    this._height = value;
    this.onPropertyChanged('height');
  }

  /**
   * Monitor item for data binding
   */
  public get item(): MonitorItem | undefined {
    return this._item;
  }

  public set item(value: MonitorItem | undefined) {
    this._item = value;
    this.onPropertyChanged('item');
  }

  /**
   * Collection of drag components
   */
  public get dragBindBases(): any[] {
    return this._dragBindBases;
  }

  public set dragBindBases(value: any[]) {
    this._dragBindBases = value || [];
    this.onPropertyChanged('dragBindBases');
  }

  /**
   * Add a component to the watch
   */
  public addComponent(component: any): void {
    this._dragBindBases.push(component);
    this.onPropertyChanged('dragBindBases');
  }

  /**
   * Remove a component from the watch
   */
  public removeComponent(component: any): void {
    const index = this._dragBindBases.indexOf(component);
    if (index > -1) {
      this._dragBindBases.splice(index, 1);
      this.onPropertyChanged('dragBindBases');
    }
  }

  /**
   * Get component by index
   */
  public getComponent(index: number): any {
    return index >= 0 && index < this._dragBindBases.length ? this._dragBindBases[index] : null;
  }

  /**
   * Get component count
   */
  public getComponentCount(): number {
    return this._dragBindBases.length;
  }
}

/**
 * Monitor item containing health and system data
 * Equivalent to C# MonitorItem
 */
export class MonitorItem extends Snapshoot {
  private _kwhNum: number = 0;
  private _stepNum: number = 0;
  private _heartRateNum: number = 0;
  private _calorieNum: number = 0;
  private _currentDateTime: Date;
  private _strengthNum: number = 0;
  private _isOpen: boolean = false;

  // Event handlers
  public numChanged?: (name: string, value: number) => void;
  public timeChanged?: (dateTime: Date) => void;

  constructor() {
    super();
    this._currentDateTime = MonitorItem.defaultTime;
  }

  /**
   * Default time for preview
   */
  public static get defaultTime(): Date {
    return new Date('2024-10-18T10:08:36');
  }

  /**
   * Battery/KWh number
   */
  public get kwhNum(): number {
    return this._kwhNum;
  }

  public set kwhNum(value: number) {
    this._kwhNum = value;
    this.onPropertyChanged('kwhNum');
  }

  /**
   * Step count
   */
  public get stepNum(): number {
    return this._stepNum;
  }

  public set stepNum(value: number) {
    this._stepNum = value;
    this.onPropertyChanged('stepNum');
    this.numChanged?.('StepNum', value);
  }

  /**
   * Heart rate
   */
  public get heartRateNum(): number {
    return this._heartRateNum;
  }

  public set heartRateNum(value: number) {
    this._heartRateNum = value;
    this.onPropertyChanged('heartRateNum');
  }

  /**
   * Calories
   */
  public get calorieNum(): number {
    return this._calorieNum;
  }

  public set calorieNum(value: number) {
    this._calorieNum = value;
    this.onPropertyChanged('calorieNum');
    this.numChanged?.('CalorieNum', value);
  }

  /**
   * Current date/time
   */
  public get currentDateTime(): Date {
    return this._currentDateTime;
  }

  public set currentDateTime(value: Date) {
    this._currentDateTime = value;
    this.onPropertyChanged('currentDateTime');
    this.timeChanged?.(value);
  }

  /**
   * Strength/fitness level
   */
  public get strengthNum(): number {
    return this._strengthNum;
  }

  public set strengthNum(value: number) {
    this._strengthNum = value;
    this.onPropertyChanged('strengthNum');
    this.numChanged?.('StrengthNum', value);
  }

  /**
   * Open/closed state
   */
  public get isOpen(): boolean {
    return this._isOpen;
  }

  public set isOpen(value: boolean) {
    this._isOpen = value;
    this.onPropertyChanged('isOpen');
  }

  /**
   * Reset to default values
   */
  public resetToDefaults(): void {
    this.kwhNum = 0;
    this.stepNum = 0;
    this.heartRateNum = 0;
    this.calorieNum = 0;
    this.currentDateTime = MonitorItem.defaultTime;
    this.strengthNum = 0;
    this.isOpen = false;
  }

  /**
   * Get all numeric values as an object
   */
  public getAllValues(): Record<string, number | boolean | Date> {
    return {
      kwhNum: this.kwhNum,
      stepNum: this.stepNum,
      heartRateNum: this.heartRateNum,
      calorieNum: this.calorieNum,
      currentDateTime: this.currentDateTime,
      strengthNum: this.strengthNum,
      isOpen: this.isOpen
    };
  }
}

/**
 * Monitor configuration
 * Equivalent to C# Monitor
 */
export class Monitor extends Snapshoot {
  private _monitorType?: BindMonitorType;

  /**
   * Monitor type
   */
  public get monitorType(): BindMonitorType | undefined {
    return this._monitorType;
  }

  public set monitorType(value: BindMonitorType | undefined) {
    this._monitorType = value;
    this.onPropertyChanged('monitorType');
  }
}