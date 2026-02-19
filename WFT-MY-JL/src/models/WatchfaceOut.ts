/**
 * Output format for watch face XML generation
 * Equivalent to C# WatchfaceOut
 */

export enum WatchScreen {
  light = 'light',
  dark = 'dark'
}

export interface WatchImage {
  name: string;
  src: string;
  isPreview?: boolean;
}

export interface Layout {
  ref: string;
  x: number;
  y: number;
}

export interface Theme {
  name: string;
  layout: Layout[];
  type: string;
  isPhotoAlbumWatchface: boolean;
  styleName?: string;
  preview?: string | null;
  bg?: string;
}

export interface Resources {
  images: WatchImage[];
  imageArrays: any[];
  translations: any[];
  dataItemImageValues: any[];
  dataItemImageNumbers: any[];
  dataItemImagePoints: any[];
  widgets: any[];
  slots: any[];
  sprites: any[];
}

export class WatchfaceOut {
  public name?: string;
  public width: number = 0;
  public height: number = 0;
  public id?: string;
  public sku: boolean = false;
  public compressMethod: string = 'None';
  public editable: boolean = false;
  public deviceType?: string;
  public resources: Resources;
  public themes: Theme[];
  public elementCache: Map<string, Resources>;

  constructor() {
    this.resources = {
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
    this.themes = [];
    this.elementCache = new Map();
  }
}

export interface HnTheme {
  version?: string;
  screen?: string;
  extra?: string;
  extra_zh_title?: string;
  extra_zh_briefInfo?: string;
  extra_en_title?: string;
  extra_en_briefInfo?: string;
}

export class WatchInfo {
  public shape?: string;
  public name?: string;
  public deviceType?: string;
  public version?: string;
  public size?: string;
  public author?: string;
  public pkgName?: string;
  public imageFormat: string = 'indexed8';
  public imageCompression: boolean = false;
  public editorVersion?: string;
  public webVersionCreatedAt?: string;
  public editorVersionCreatedAt?: string;
  public webVersionUpdatedAt?: string;
  public editorVersionUpdatedAt?: string;
  public hnTheme: HnTheme;

  constructor() {
    this.hnTheme = {};
  }
}

export interface WatchInfoExtra {
  zh: {
    title: string;
    briefInfo: string;
  };
  en: {
    title: string;
    briefInfo: string;
  };
}