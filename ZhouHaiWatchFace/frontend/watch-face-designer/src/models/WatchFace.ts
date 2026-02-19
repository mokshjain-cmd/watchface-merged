import type { UUID } from '@/types';

// Base interfaces
export interface BaseModel {
  id?: UUID;
  createTime?: Date;
  updateTime?: Date;
}

// Watch Face related interfaces
export interface WatchFace extends BaseModel {
  watchName?: string;
  watchCode?: string;
  cornerX?: number;
  cornerY?: number;
  width: number;
  height: number;
  thumbnailWidth: number;
  thumbnailHeight: number;
  corner: number;
  deviceType?: string;
  isAlbum?: boolean;
  colorBit?: number;
  folderName: string;
  albumBackground?: AlbumBackground;
  watchStyles: WatchStyle[];
}

export interface WatchStyle extends BaseModel {
  styleName?: string;
  screenType: WatchScreenType;
  dragBindBases: DragBindBase[];
  zh?: Watch;
}

export interface Watch {
  item?: MonitorItem;
}

export interface MonitorItem {
  // Monitor item properties
  [key: string]: any;
}

export interface AlbumBackground {
  backgroundSource?: string;
  width?: number;
  height?: number;
}

// Enums
export enum WatchScreenType {
  LIGHT = 'light',
  DARK = 'dark',
}

export enum BindMonitorType {
  // Add monitor types as needed based on C# enum
  TIME = 'time',
  DATE = 'date',
  BATTERY = 'battery',
  STEPS = 'steps',
  WEATHER = 'weather',
}

// Base drag element interface
export interface DragBindBase extends BaseModel {
  dragId?: UUID;
  dragName?: string;
  visible?: boolean;
  elementType?: BindMonitorType;
  left?: number;
  top?: number;
  width?: number;
  height?: number;
  zIndex?: number;
}

// Specific drag element types
export interface DragImage extends DragBindBase {
  source?: string;
  opacity?: number;
}

export interface DragWidget extends DragBindBase {
  widgetType?: string;
  properties?: Record<string, any>;
}

export interface DragNums extends DragBindBase {
  format?: string;
  alignment?: 'left' | 'center' | 'right';
  fontFamily?: string;
  fontSize?: number;
  color?: string;
}

export interface DragProgress extends DragBindBase {
  progressType?: 'linear' | 'circular';
  minValue?: number;
  maxValue?: number;
  backgroundColor?: string;
  foregroundColor?: string;
}

export interface DragWeek extends DragBindBase {
  weekFormat?: 'short' | 'long';
  language?: string;
}

export interface DragAMPM extends DragBindBase {
  amText?: string;
  pmText?: string;
  format?: '12h' | '24h';
}

export interface DragSwitch extends DragBindBase {
  isOn?: boolean;
  onImage?: string;
  offImage?: string;
}

export interface DragSlot extends DragBindBase {
  slotType?: string;
  placeholder?: string;
}

export interface DragPoint extends DragBindBase {
  pointType?: string;
  radius?: number;
  fillColor?: string;
  strokeColor?: string;
}

export interface DragAnimFrame extends DragBindBase {
  frames: string[];
  duration?: number;
  loop?: boolean;
}

// Device configuration
export interface DeviceConfiguration {
  name: string;
  deviceType: string;
  width: number;
  height: number;
  thumbnailWidth: number;
  thumbnailHeight: number;
  corner: number;
  dpi?: number;
  colorDepth?: number;
}

// Export output interfaces
export interface WatchFaceOutput {
  id: string;
  name?: string;
  width: number;
  height: number;
  deviceType?: string;
  themes: Theme[];
  resources: Resources;
  elementCache: Record<string, Resources>;
}

export interface Theme {
  name: string;
  type: 'normal' | 'AOD';
  styleName?: string;
  layout: any[];
  preview?: string;
  bg?: string;
  isPhotoAlbumWatchface?: boolean;
}

export interface Resources {
  translations: Translation[];
  images: WatchImage[];
  imageArrays: ImageArray[];
  dataItemImageValues: DataItemImageValue[];
  dataItemImageNumbers: DataItemImageNumber[];
  dataItemImagePoints: DataItemImagePoint[];
  widgets: Widget[];
  slots: Slot[];
  sprites: Sprite[];
}

export interface Translation {
  key: string;
  value: string;
  language?: string;
}

export interface WatchImage {
  name: string;
  src: string;
  isPreview?: boolean;
  width?: number;
  height?: number;
}

export interface ImageArray {
  name: string;
  images: WatchImage[];
}

export interface DataItemImageValue {
  name: string;
  dataType: string;
  images: WatchImage[];
}

export interface DataItemImageNumber {
  name: string;
  dataType: string;
  format: string;
  images: WatchImage[];
}

export interface DataItemImagePoint {
  name: string;
  dataType: string;
  centerX: number;
  centerY: number;
  images: WatchImage[];
}

export interface Widget {
  name: string;
  type: string;
  properties: Record<string, any>;
}

export interface Slot {
  name: string;
  type: string;
  x: number;
  y: number;
  width: number;
  height: number;
}

export interface Sprite {
  name: string;
  images: WatchImage[];
  frameRate?: number;
}

// Binary file interfaces
export interface BinFileHeader {
  magicWord: number; // 0x1234A55A
  version: Version;
  colorGroupCount: number;
  reserved: Uint8Array; // 3 bytes
  themeCount: number;
  colorCount: number;
  flags: number;
  previewImgDataAddr: number;
  reserved1: number;
  id?: Uint8Array;
  name?: Uint8Array;
}

export interface Version {
  major: number;
  minor: number;
  build: number;
  revision: number;
}

export interface BinTheme {
  // Binary theme properties
  id: string;
  type: 'normal' | 'AOD';
  data: Uint8Array;
}

export interface WatchBinFile {
  header?: BinFileHeader;
  binThemes: BinTheme[];
  recordBases: RecordBase[];
  rawData: Uint8Array;
  currentSize: number;
  cache: Record<string, BinCache>;
}

export interface RecordBase {
  type: string;
  data: Uint8Array;
}

export interface BinCache {
  data: any;
  timestamp: Date;
}