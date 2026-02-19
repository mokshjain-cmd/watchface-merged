import { Snapshoot } from './Watch';
import { WatchfaceOut, WatchScreen, Theme } from './WatchfaceOut';
import { OutXmlHelper } from './OutXmlHelper';
import { CommonHelper } from './CommonHelper';

/**
 * Main watch face model containing all styles and configuration
 * Equivalent to C# WatchFace
 */
export class WatchFace extends Snapshoot {
  private _watchName?: string;
  private _watchCode?: string;
  private _cornerX?: number;
  private _cornerY?: number;
  private _width: number = 400;
  private _height: number = 400;
  private _thumbnailWidth: number = 200;
  private _thumbnailHeight: number = 200;
  private _corner: number = 0;
  private _deviceType?: string;
  private _createTime?: Date;
  private _isAlbum: boolean = false;
  private _colorBit?: number;
  private _albumBackground?: any; // AlbumBackground
  private _watchStyles: WatchStyle[] = [];

  constructor() {
    super();
    this._watchStyles = [];
  }

  /**
   * Watch name
   */
  public get watchName(): string | undefined {
    return this._watchName;
  }

  public set watchName(value: string | undefined) {
    this._watchName = value;
    this.onPropertyChanged('watchName');
  }

  /**
   * Watch code
   */
  public get watchCode(): string | undefined {
    return this._watchCode;
  }

  public set watchCode(value: string | undefined) {
    this._watchCode = value;
    this.onPropertyChanged('watchCode');
  }

  /**
   * Corner X radius
   */
  public get cornerX(): number | undefined {
    return this._cornerX;
  }

  public set cornerX(value: number | undefined) {
    this._cornerX = value;
    this.onPropertyChanged('cornerX');
  }

  /**
   * Corner Y radius
   */
  public get cornerY(): number | undefined {
    return this._cornerY;
  }

  public set cornerY(value: number | undefined) {
    this._cornerY = value;
    this.onPropertyChanged('cornerY');
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
   * Thumbnail width
   */
  public get thumbnailWidth(): number {
    return this._thumbnailWidth;
  }

  public set thumbnailWidth(value: number) {
    this._thumbnailWidth = value;
    this.onPropertyChanged('thumbnailWidth');
  }

  /**
   * Thumbnail height
   */
  public get thumbnailHeight(): number {
    return this._thumbnailHeight;
  }

  public set thumbnailHeight(value: number) {
    this._thumbnailHeight = value;
    this.onPropertyChanged('thumbnailHeight');
  }

  /**
   * Corner radius
   */
  public get corner(): number {
    return this._corner;
  }

  public set corner(value: number) {
    this._corner = value;
    this.onPropertyChanged('corner');
  }

  /**
   * Device type
   */
  public get deviceType(): string | undefined {
    return this._deviceType;
  }

  public set deviceType(value: string | undefined) {
    this._deviceType = value;
    this.onPropertyChanged('deviceType');
  }

  /**
   * Creation time
   */
  public get createTime(): Date | undefined {
    return this._createTime;
  }

  public set createTime(value: Date | undefined) {
    this._createTime = value;
    this.onPropertyChanged('createTime');
  }

  /**
   * Whether this is an album watch face
   */
  public get isAlbum(): boolean {
    return this._isAlbum;
  }

  public set isAlbum(value: boolean) {
    this._isAlbum = value;
    this.onPropertyChanged('isAlbum');
  }

  /**
   * Color bit depth
   */
  public get colorBit(): number | undefined {
    return this._colorBit;
  }

  public set colorBit(value: number | undefined) {
    this._colorBit = value;
    this.onPropertyChanged('colorBit');
  }

  /**
   * Album background
   */
  public get albumBackground(): any {
    return this._albumBackground;
  }

  public set albumBackground(value: any) {
    this._albumBackground = value;
    this.onPropertyChanged('albumBackground');
  }

  /**
   * Folder name (computed)
   */
  public get folderName(): string {
    return `${this._watchCode}_${this._watchName}_${this._deviceType}`;
  }

  /**
   * Watch styles
   */
  public get watchStyles(): WatchStyle[] {
    return this._watchStyles;
  }

  public set watchStyles(value: WatchStyle[]) {
    this._watchStyles = value || [];
    this.onPropertyChanged('watchStyles');
  }

  /**
   * Add a watch style
   */
  public addWatchStyle(style: WatchStyle): void {
    this._watchStyles.push(style);
    this.onPropertyChanged('watchStyles');
  }

  /**
   * Remove a watch style
   */
  public removeWatchStyle(style: WatchStyle): void {
    const index = this._watchStyles.indexOf(style);
    if (index > -1) {
      this._watchStyles.splice(index, 1);
      this.onPropertyChanged('watchStyles');
    }
  }

  /**
   * Generate watchface output XML
   */
  public getWatchfaceOut(): WatchfaceOut {
    const faceOut = new WatchfaceOut();
    faceOut.name = this._watchName;
    faceOut.width = this._width;
    faceOut.height = this._height;
    faceOut.deviceType = this._deviceType;
    faceOut.id = this.generateId();

    // Handle AOD (Always On Display) style
    const aodStyle = this._watchStyles.find(x => x.screenType === WatchScreen.dark);
    if (aodStyle) {
      const index = this._watchStyles.indexOf(aodStyle);
      this._watchStyles.splice(index, 1);
      this._watchStyles.splice(1, 0, aodStyle);
    }

    // Process each style
    for (const style of this._watchStyles) {
      const xmlOutputs = style.dragBindBases.map(component => component.getOutXml());

      const theme: Theme = {
        name: OutXmlHelper.getWatchElementName(),
        layout: xmlOutputs.map(x => x.layout).filter(x => x),
        type: style.screenType === WatchScreen.light ? 'normal' : 'AOD',
        isPhotoAlbumWatchface: style.screenType === WatchScreen.light && this._isAlbum,
        styleName: style.styleName
      };

      faceOut.themes.push(theme);

      // Create resources for this style
      const resources = {
        images: [],
        imageArrays: [],
        translations: [],
        dataItemImageValues: [],
        dataItemImageNumbers: [],
        dataItemImagePoints: [],
        widgets: [],
        slots: [],
        sprites: []
      };

      // Process images and resources
      for (const xml of xmlOutputs) {
        if (xml.images) {
          for (const image of xml.images) {
            if (!image.src) {
              throw new Error('Image path cannot be empty');
            }
            image.src = CommonHelper.outPath(image.src, this.folderName);
          }
        }

        if (xml.imageArrays) {
          for (const imageArray of xml.imageArrays) {
            if (imageArray.images) {
              for (const image of imageArray.images) {
                if (!image.src) {
                  throw new Error('Image path cannot be empty');
                }
                image.src = CommonHelper.outPath(image.src, this.folderName);
              }
            }
          }
        }
      }

      // Add preview image
      const preview = {
        name: OutXmlHelper.getWatchElementName(),
        src: `_preview/${style.styleName}.png`,
        isPreview: true
      };

      theme.preview = theme.type !== 'AOD' ? `@${preview.name}` : null;
      resources.images.push(preview);

      // Add album background if applicable
      if (this._isAlbum && theme.type !== 'AOD' && this._albumBackground?.backgroundSource) {
        const background = {
          name: OutXmlHelper.getWatchElementName(),
          src: CommonHelper.outPath(this._albumBackground.backgroundSource, this.folderName)
        };
        theme.bg = `@${background.name}`;
        resources.images.push(background);
      }

      // Collect all resources
      for (const xml of xmlOutputs) {
        if (xml.translations) resources.translations.push(...xml.translations);
        if (xml.images) resources.images.push(...xml.images);
        if (xml.imageArrays) resources.imageArrays.push(...xml.imageArrays);
        if (xml.dataItemImageValues) resources.dataItemImageValues.push(...xml.dataItemImageValues);
        if (xml.dataItemImageNumbers) resources.dataItemImageNumbers.push(...xml.dataItemImageNumbers);
        if (xml.dataItemPointers) resources.dataItemImagePoints.push(...xml.dataItemPointers);
        if (xml.widgets) resources.widgets.push(...xml.widgets);
        if (xml.slots) resources.slots.push(...xml.slots);
        if (xml.sprites) resources.sprites.push(...xml.sprites);
      }

      // Add to main resources
      faceOut.resources.translations.push(...resources.translations);
      faceOut.resources.images.push(...resources.images);
      faceOut.resources.imageArrays.push(...resources.imageArrays);
      faceOut.resources.dataItemImageValues.push(...resources.dataItemImageValues);
      faceOut.resources.dataItemImageNumbers.push(...resources.dataItemImageNumbers);
      faceOut.resources.dataItemImagePoints.push(...resources.dataItemImagePoints);
      faceOut.resources.widgets.push(...resources.widgets);
      faceOut.resources.slots.push(...resources.slots);
      faceOut.resources.sprites.push(...resources.sprites);
    }

    return faceOut;
  }

  /**
   * Generate unique ID
   */
  private generateId(): string {
    return Math.random().toString(36).substring(2) + Date.now().toString(36);
  }
}

/**
 * Watch style containing components for a specific screen type
 * Equivalent to C# WatchStyle
 */
export class WatchStyle extends Snapshoot {
  private _styleName?: string;
  private _dragBindBases: any[] = [];
  private _templateBinds: any[] = [];
  private _screenType: WatchScreen = WatchScreen.light;
  private _zh: any; // Watch

  constructor() {
    super();
    this._dragBindBases = [];
    this._templateBinds = [];
    // Initialize Zh with default values
    this._zh = {
      item: {
        numChanged: undefined,
        timeChanged: undefined
      }
    };
  }

  /**
   * Style name
   */
  public get styleName(): string | undefined {
    return this._styleName;
  }

  public set styleName(value: string | undefined) {
    this._styleName = value;
    this.onPropertyChanged('styleName');
  }

  /**
   * Drag components
   */
  public get dragBindBases(): any[] {
    return this._dragBindBases;
  }

  public set dragBindBases(value: any[]) {
    this._dragBindBases = value || [];
    this.onPropertyChanged('dragBindBases');
  }

  /**
   * Template bindings
   */
  public get templateBinds(): any[] {
    return this._templateBinds;
  }

  public set templateBinds(value: any[]) {
    this._templateBinds = value || [];
    this.onPropertyChanged('templateBinds');
  }

  /**
   * Screen type (light/dark)
   */
  public get screenType(): WatchScreen {
    return this._screenType;
  }

  public set screenType(value: WatchScreen) {
    this._screenType = value;
    this.onPropertyChanged('screenType');
  }

  /**
   * Chinese watch configuration
   */
  public get zh(): any {
    return this._zh;
  }

  public set zh(value: any) {
    this._zh = value;
    this.onPropertyChanged('zh');
  }

  /**
   * Calculate time angle for pointers
   */
  public static getTimeAngle(idx: number, dateTime: Date): number {
    switch (idx) {
      case 0: // Hour
        return dateTime.getHours() * 30 + dateTime.getMinutes() / 2;
      case 1: // Minute
        return dateTime.getMinutes() * 6;
      case 2: // Second
        return dateTime.getSeconds() * 6;
      default:
        return 0;
    }
  }

  /**
   * Handle number changes from monitor item
   */
  private handleNumChanged(numName: string, value: number): void {
    const nameMap: { [key: string]: string } = {
      'StepNum': 'Steps',
      'CalorieNum': 'Calories',
      'StrengthNum': 'Exercise'
    };

    const mappedName = nameMap[numName];
    if (!mappedName) return;

    for (const item of this._templateBinds) {
      if (item.itemName && item.itemName.includes(mappedName)) {
        if (item.constructor?.name === 'DragBindNums') {
          item.currentNum = value;
        } else if (item.constructor?.name === 'DragBindProgress') {
          item.currentNum = value;
        }
      }
    }
  }

  /**
   * Handle time changes from monitor item
   */
  private handleTimeChanged(time: Date): void {
    for (const item of this._templateBinds) {
      if (item.constructor?.name === 'DragBindPoint') {
        item.value = WatchStyle.getTimeAngle(item.valueIndex, time);
      }
    }
  }
}