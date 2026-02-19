// Device and project related models
import type { UUID } from '@/types';

export interface Project extends BaseProjectModel {
  name: string;
  description?: string;
  watchFaces: ProjectWatchFace[];
  settings: ProjectSettings;
  recentFiles: RecentFile[];
}

export interface BaseProjectModel {
  id: UUID;
  createTime: Date;
  updateTime: Date;
  version: string;
}

export interface ProjectWatchFace {
  id: UUID;
  name: string;
  deviceType: string;
  width: number;
  height: number;
  thumbnailPath?: string;
  previewPath?: string;
  lastModified: Date;
  folderPath: string;
}

export interface ProjectSettings {
  defaultDevice?: string;
  autoSave: boolean;
  autoSaveInterval: number; // in minutes
  showGrid: boolean;
  snapToGrid: boolean;
  gridSize: number;
  theme: 'light' | 'dark' | 'auto';
  language: string;
  exportFormats: ExportFormat[];
}

export interface ExportFormat {
  type: 'json' | 'bin' | 'png' | 'jpg';
  enabled: boolean;
  quality?: number; // for images
  compression?: boolean; // for bin files
}

export interface RecentFile {
  id: UUID;
  name: string;
  path: string;
  lastOpened: Date;
  deviceType: string;
  thumbnailPath?: string;
}

export interface DeviceProfile {
  id: string;
  name: string;
  displayName: string;
  manufacturer?: string;
  model?: string;
  specifications: DeviceSpecifications;
  constraints: DeviceConstraints;
  supportedFeatures: SupportedFeatures;
}

export interface DeviceSpecifications {
  screenWidth: number;
  screenHeight: number;
  thumbnailWidth: number;
  thumbnailHeight: number;
  cornerRadius: number;
  dpi: number;
  colorDepth: number; // bits per pixel
  refreshRate?: number; // Hz
  hasAOD: boolean; // Always On Display support
}

export interface DeviceConstraints {
  maxFileSize: number; // in bytes
  maxImageSize: number; // in bytes
  maxAnimationFrames: number;
  supportedImageFormats: string[];
  maxElementsPerTheme: number;
  maxThemes: number;
}

export interface SupportedFeatures {
  customFonts: boolean;
  animations: boolean;
  widgets: string[]; // supported widget types
  complications: string[]; // supported complication types
  gestures: string[]; // supported gesture types
  sensors: string[]; // supported sensor types
}

// Menu and UI models
export interface MenuItem {
  id: string;
  label: string;
  icon?: string;
  action?: string;
  children?: MenuItem[];
  isEnabled: boolean;
  isVisible: boolean;
  shortcut?: string;
}

export interface ToolbarItem {
  id: string;
  type: 'button' | 'toggle' | 'dropdown' | 'separator' | 'group';
  label?: string;
  icon?: string;
  tooltip?: string;
  action?: string;
  isEnabled: boolean;
  isVisible: boolean;
  isActive?: boolean; // for toggle items
  children?: ToolbarItem[]; // for groups and dropdowns
}

export interface PropertyGroup {
  id: string;
  label: string;
  icon?: string;
  properties: PropertyItem[];
  isCollapsed: boolean;
  isVisible: boolean;
}

export interface PropertyItem {
  id: string;
  label: string;
  type: PropertyType;
  value: any;
  defaultValue?: any;
  options?: PropertyOption[]; // for select/radio types
  min?: number; // for number/range types
  max?: number; // for number/range types
  step?: number; // for number/range types
  placeholder?: string; // for text types
  isRequired: boolean;
  isReadOnly: boolean;
  isVisible: boolean;
  validation?: ValidationRule[];
  dependsOn?: string; // property ID that this depends on
}

export enum PropertyType {
  TEXT = 'text',
  NUMBER = 'number',
  BOOLEAN = 'boolean',
  COLOR = 'color',
  IMAGE = 'image',
  SELECT = 'select',
  RADIO = 'radio',
  CHECKBOX = 'checkbox',
  RANGE = 'range',
  DATE = 'date',
  TIME = 'time',
  FILE = 'file',
  FONT = 'font',
  POSITION = 'position',
  SIZE = 'size',
}

export interface PropertyOption {
  value: any;
  label: string;
  icon?: string;
  isDisabled?: boolean;
}

export interface ValidationRule {
  type: 'required' | 'min' | 'max' | 'pattern' | 'custom';
  value?: any;
  message: string;
  validator?: (value: any) => boolean;
}

// Recent watches model (from C# code)
export interface RecentlyWatches {
  name: string;
  previewPath?: string;
  deviceType?: string;
  createTime?: Date;
  lastModified?: Date;
}