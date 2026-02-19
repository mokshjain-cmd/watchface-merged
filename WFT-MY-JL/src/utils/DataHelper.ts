import * as XLSX from 'xlsx';
import { DragBindBase, BindMonitorType } from '../models/DragDataBase';
import { DragBindSingleDigit } from '../models/DragSingleDigit';
import { 
  DragBindNormalDateTime, 
  DragBindProgress, 
  DragBindNums, 
  DragBindSwitch, 
  DragBindAMPM, 
  DragBindImage 
} from '../models/DragComponents';
import { DragBindDouble } from '../models/DragDouble';
import { DragBindKeyValue } from '../models/DragKeyValue';
import { DragBindPoint } from '../models/DragPoint';
import { DragBindAnimFrame } from '../models/DragAnimFrame';
import { DragBindMonthDay } from '../models/DragMonthDay';
import { DragBindWeek, DateTimeType } from '../models/DragWeek';
import { DataItemType, DataItemTypeHelper } from '../models/DataItemType';
import { CommonHelper } from '../models/CommonHelper';

export interface WatchSetting {
  ItemName?: string;
  ControlType?: string;
  BindMonitorType?: BindMonitorType;
  MaxNum?: number;
  MinNum?: number;
  Default?: number;
  TargetValue?: number;
}

export class DataHelper {
  private static _watchSettings: WatchSetting[] | null = null;

  // Load Excel file and parse it
  public static async loadWatchSettings(): Promise<void> {
    try {
      const response = await fetch('/WatchFace.xlsx');
      const arrayBuffer = await response.arrayBuffer();
      const workbook = XLSX.read(arrayBuffer, { type: 'array' });
      
      // Assuming the data is in the first worksheet
      const firstSheetName = workbook.SheetNames[0];
      const worksheet = workbook.Sheets[firstSheetName];
      
      // Convert to JSON
      const jsonData = XLSX.utils.sheet_to_json(worksheet) as any[];
      
      // Map to WatchSetting objects
      this._watchSettings = jsonData.map(row => ({
        ItemName: row['ItemName'] || row['Item Name'] || '',
        ControlType: row['ControlType'] || row['Control Type'] || '',
        BindMonitorType: this.parseBindMonitorType(row['BindMonitorType'] || row['Bind Monitor Type']),
        MaxNum: row['MaxNum'] || row['Max Num'],
        MinNum: row['MinNum'] || row['Min Num'],
        Default: row['Default'],
        TargetValue: row['TargetValue'] || row['Target Value']
      }));

      console.log('Loaded watch settings:', this._watchSettings);
    } catch (error) {
      console.error('Error loading watch settings:', error);
      this._watchSettings = [];
    }
  }

  private static parseBindMonitorType(value: any): BindMonitorType {
    if (typeof value === 'string') {
      switch (value.toLowerCase()) {
        case 'time': return BindMonitorType.Time;
        case 'bloodoxygen': return BindMonitorType.BloodOxygen;
        case 'battery': return BindMonitorType.Battery;
        case 'steps': return BindMonitorType.Steps;
        case 'heartrate': return BindMonitorType.HeartRate;
        case 'calories': return BindMonitorType.Calories;
        case 'distance': return BindMonitorType.Distance;
        case 'sleep': return BindMonitorType.Sleep;
        case 'weather': return BindMonitorType.Weather;
        default: return BindMonitorType.Time;
      }
    }
    return value || BindMonitorType.Time;
  }

  public static get watchSettings(): WatchSetting[] {
    return this._watchSettings || [];
  }

  public static getWatchSettingByItemName(itemName: string): WatchSetting | undefined {
    return this._watchSettings?.find(x => x.ItemName === itemName);
  }

  public static getDecimalPlaces(itemName: string): number {
    switch (itemName) {
      case 'Sleep Duration':
      case 'Step Distance':
        return 2;
      default:
        return 1;
    }
  }

  public static get dateTimeTypes(): Record<string, DateTimeType> {
    return {
      'Week': DateTimeType.Week,
      'Month (Normal)': DateTimeType.Month,
      'Day (Normal)': DateTimeType.Day,
      'Hour (Normal)': DateTimeType.Hour,
      'Minute (Normal)': DateTimeType.Minute,
      'Second (Normal)': DateTimeType.Second
    };
  }

  // Convert C# control types to our web component types
  public static mapControlTypeToComponentType(controlType: string): string {
    switch (controlType) {
      case 'DragNormalDateTime':
      case 'DragSingleDigit':
        return 'dateTime';
      case 'DragNums':
      case 'DragDouble':
        return 'number';
      case 'DragProgress':
        return 'progress';
      case 'DragImage':
      case 'DragPoint':
        return 'image';
      case 'DragSwitch':
      case 'DragAMPM':
        return 'switch';
      case 'DragAnimFrame':
        return 'animation';
      default:
        return 'image';
    }
  }

  // Map BindMonitorType to our elementType numbers
  public static mapBindMonitorTypeToElementType(bindMonitorType: BindMonitorType): number {
    return bindMonitorType;
  }

  // Get component configuration based on watch setting
  public static getComponentConfig(setting: WatchSetting) {
    return {
      dateTimeFormat: setting.ItemName,
      leadingZero: true,
      minValue: setting.MinNum || 0,
      maxValue: setting.MaxNum || 100,
      defaultValue: setting.Default || 0,
      targetValue: setting.TargetValue || 0
    };
  }

  // Create DragBindBase equivalent to C# GetDragBindBase method
  public static getDragBindBase(
    folderName: string, 
    setting: WatchSetting | undefined, 
    dragName: string | undefined, 
    itemSource: string[]
  ): DragBindBase | null {
    
    if (!setting) return null;

    // Process image sources (equivalent to ImageCache.GetImage/GetEzipImage)
    let processedItemSource = itemSource;
    
    if (setting.ControlType === "DragImage") {
      // For DragImage, use images directly
      processedItemSource = itemSource;
    } else {
      // For other types, check minimum requirements
      if ((setting.ControlType === "DragSwitch" || setting.ControlType === "DragAMPM") && itemSource.length < 2) {
        console.error("Please select at least 2 images");
        return null;
      }
      processedItemSource = itemSource;
    }

    // Create appropriate component based on control type using our new TypeScript models
    switch (setting.ControlType) {
      case "DragNormalDateTime": {
        const component = new DragBindNormalDateTime();
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.dateTimeFormat = setting.ItemName;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragSingleDigit": {
        const component = new DragBindSingleDigit();
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.dateTimeFormat = setting.ItemName;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragNums": {
        const component = new DragBindNums(
          setting.MaxNum || 0,
          setting.MinNum || 0,
          setting.Default || 0
        );
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        component.defaultNum = setting.Default;
        return component;
      }

      case "DragProgress": {
        const component = new DragBindProgress(
          setting.MaxNum || 0,
          setting.MinNum || 0,
          setting.Default || 0
        );
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        component.defaultNum = setting.Default;
        component.targetValue = setting.TargetValue || 0;
        return component;
      }

      case "DragSwitch": {
        const component = new DragBindSwitch();
        component.dragName = dragName;
        component.openSource = processedItemSource[0];
        component.closeSource = processedItemSource[1];
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragAMPM": {
        const component = new DragBindAMPM();
        component.dragName = dragName;
        component.amSource = processedItemSource[0];
        component.pmSource = processedItemSource[1];
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragImage": {
        const component = new DragBindImage();
        component.dragName = dragName;
        component.source = processedItemSource[0];
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragMonthDay": {
        const component = new DragBindMonthDay();
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragWeek": {
        const component = new DragBindWeek();
        component.dragName = dragName;
        component.imageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragDouble": {
        const component = new DragBindDouble(
          setting.MaxNum || 99.99,
          setting.MinNum || 0,
          setting.Default || 0
        );
        component.dragName = dragName;
        component.ImageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragKeyValue": {
        const keyValues = new Map<number, string>();
        // For key-value components, we need to set up the key-value mapping
        // This would typically come from additional configuration
        const component = new DragBindKeyValue(keyValues, processedItemSource);
        component.dragName = dragName;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragPoint": {
        const component = new DragBindPoint(processedItemSource[0]);
        component.dragName = dragName;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      case "DragAnimFrame": {
        const component = new DragBindAnimFrame();
        component.dragName = dragName;
        component.ImageSource = processedItemSource;
        component.elementType = setting.BindMonitorType;
        component.itemName = setting.ItemName;
        return component;
      }

      default:
        console.warn(`Unknown control type: ${setting.ControlType}`);
        return null;
    }
  }

  /**
   * Enhanced component creation with full TypeScript model integration
   */
  public static createComponent(type: string, config: any): DragBindBase | null {
    switch (type) {
      case 'time-hour':
        return this.createTimeComponent('Hour', config);
      case 'time-minute':
        return this.createTimeComponent('Minute', config);
      case 'time-second':
        return this.createTimeComponent('Second', config);
      case 'date-day':
        return this.createTimeComponent('Day', config);
      case 'date-month':
        return this.createTimeComponent('Month', config);
      case 'date-week':
        return this.createWeekComponent(config);
      case 'health-steps':
        return this.createHealthComponent('Steps', config);
      case 'health-heartrate':
        return this.createHealthComponent('HeartRate', config);
      case 'health-calories':
        return this.createHealthComponent('Calories', config);
      case 'system-battery':
        return this.createSystemComponent('Battery', config);
      case 'double-value':
        return this.createDoubleComponent(config);
      case 'key-value':
        return this.createKeyValueComponent(config);
      case 'pointer':
        return this.createPointerComponent(config);
      case 'animation':
        return this.createAnimationComponent(config);
      default:
        console.warn(`Unknown component type: ${type}`);
        return null;
    }
  }

  /**
   * Create time-based components
   */
  private static createTimeComponent(format: string, config: any): DragBindBase {
    if (config.multiDigit) {
      const component = new DragBindNormalDateTime();
      component.dateTimeFormat = format;
      component.imageSource = config.images || [];
      component.elementType = BindMonitorType.Time;
      return component;
    } else {
      const component = new DragBindSingleDigit();
      component.dateTimeFormat = format;
      component.imageSource = config.images || [];
      component.elementType = BindMonitorType.Time;
      return component;
    }
  }

  /**
   * Create week component
   */
  private static createWeekComponent(config: any): DragBindWeek {
    const component = new DragBindWeek();
    component.imageSource = config.images || [];
    component.elementType = BindMonitorType.Time;
    return component;
  }

  /**
   * Create health-related components
   */
  private static createHealthComponent(type: string, config: any): DragBindBase {
    let bindType: BindMonitorType;
    
    switch (type) {
      case 'Steps':
        bindType = BindMonitorType.Steps;
        break;
      case 'HeartRate':
        bindType = BindMonitorType.HeartRate;
        break;
      case 'Calories':
        bindType = BindMonitorType.Calories;
        break;
      default:
        bindType = BindMonitorType.Steps;
    }

    if (config.displayType === 'progress') {
      const component = new DragBindProgress(
        config.maxValue || 100,
        config.minValue || 0,
        config.defaultValue || 0
      );
      component.imageSource = config.images || [];
      component.elementType = bindType;
      return component;
    } else {
      const component = new DragBindNums(
        config.maxValue || 9999,
        config.minValue || 0,
        config.defaultValue || 0
      );
      component.imageSource = config.images || [];
      component.elementType = bindType;
      return component;
    }
  }

  /**
   * Create system components
   */
  private static createSystemComponent(type: string, config: any): DragBindBase {
    const component = new DragBindProgress(100, 0, 50);
    component.imageSource = config.images || [];
    component.elementType = BindMonitorType.Battery;
    return component;
  }

  /**
   * Create double value components
   */
  private static createDoubleComponent(config: any): DragBindDouble {
    const component = new DragBindDouble(
      config.maxValue || 99.99,
      config.minValue || 0,
      config.defaultValue || 0
    );
    component.ImageSource = config.images || [];
    component.decimalPlaces = config.decimalPlaces || 1;
    component.TrailingZero = config.trailingZero || false;
    component.UnitSource = config.unitImage;
    component.PointSource = config.decimalPointImage;
    component.elementType = this.mapTypeToBindMonitorType(config.dataType);
    return component;
  }

  /**
   * Create key-value components
   */
  private static createKeyValueComponent(config: any): DragBindKeyValue {
    const keyValues = new Map<number, string>();
    if (config.keyValues) {
      Object.entries(config.keyValues).forEach(([key, value]) => {
        keyValues.set(parseInt(key), value as string);
      });
    }
    const component = new DragBindKeyValue(keyValues, config.images || []);
    component.elementType = this.mapTypeToBindMonitorType(config.dataType);
    return component;
  }

  /**
   * Create pointer components
   */
  private static createPointerComponent(config: any): DragBindPoint {
    const component = new DragBindPoint(config.images?.[0]);
    component.StartAngle = config.startAngle || 0;
    component.EndAngle = config.endAngle || 360;
    component.ValueIndex = config.valueIndex || 0;
    component.elementType = BindMonitorType.Time;
    return component;
  }

  /**
   * Create animation components
   */
  private static createAnimationComponent(config: any): DragBindAnimFrame {
    const component = new DragBindAnimFrame();
    component.ImageSource = config.images || [];
    component.FrameRate = config.frameRate;
    component.RepeatCount = config.repeatCount;
    component.IsRepeat = config.isRepeat || false;
    component.elementType = this.mapTypeToBindMonitorType(config.dataType);
    return component;
  }

  /**
   * Map data type to BindMonitorType
   */
  private static mapTypeToBindMonitorType(dataType: string): BindMonitorType {
    switch (dataType) {
      case 'time': return BindMonitorType.Time;
      case 'battery': return BindMonitorType.Battery;
      case 'steps': return BindMonitorType.Steps;
      case 'heartrate': return BindMonitorType.HeartRate;
      case 'calories': return BindMonitorType.Calories;
      default: return BindMonitorType.Time;
    }
  }

  /**
   * Validate component configuration using TypeScript models
   */
  public static validateComponent(component: DragBindBase): string[] {
    const errors: string[] = [];

    if (!component.dragName) {
      errors.push('Component must have a name');
    }

    if (!component.imageSource || component.imageSource.length === 0) {
      errors.push('Component must have at least one image');
    }

    // Type-specific validation
    if (component instanceof DragBindSwitch || component instanceof DragBindAMPM) {
      if (component.imageSource && component.imageSource.length < 2) {
        errors.push('Switch/AMPM components require at least 2 images');
      }
    }

    if (component instanceof DragBindProgress || component instanceof DragBindNums) {
      if (component.maxNum <= component.minNum) {
        errors.push('Maximum value must be greater than minimum value');
      }
    }

    return errors;
  }

  /**
   * Export component to JSON with full TypeScript model support
   */
  public static exportComponent(component: DragBindBase): any {
    return {
      type: component.constructor.name,
      config: {
        dragName: component.dragName,
        elementType: component.elementType,
        itemName: component.itemName,
        imageSource: component.imageSource,
        position: { x: component.x || 0, y: component.y || 0 },
        size: { width: component.width || 0, height: component.height || 0 },
        properties: this.getComponentProperties(component)
      },
      xml: component.getOutXml()
    };
  }

  /**
   * Get component-specific properties
   */
  private static getComponentProperties(component: DragBindBase): any {
    const props: any = {};

    if (component instanceof DragBindNormalDateTime || component instanceof DragBindSingleDigit) {
      props.dateTimeFormat = (component as any).dateTimeFormat;
    }

    if (component instanceof DragBindProgress || component instanceof DragBindNums) {
      props.maxNum = component.maxNum;
      props.minNum = component.minNum;
      props.defaultNum = component.defaultNum;
    }

    if (component instanceof DragBindProgress) {
      props.targetValue = (component as any).targetValue;
      props.fillType = (component as any).fillType;
    }

    return props;
  }
}