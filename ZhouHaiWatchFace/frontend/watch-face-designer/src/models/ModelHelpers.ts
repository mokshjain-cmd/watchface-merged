// Model transformation utilities and helpers
import type { UUID } from '@/types';
import type { 
  WatchFace, 
  DragBindBase, 
  DragImage, 
  DragWidget,
  DeviceConfiguration,
  WatchFaceOutput 
} from '@/models';

/**
 * Generates a new UUID v4
 */
export function generateId(): UUID {
  return crypto.randomUUID() as UUID;
}

/**
 * Creates a default WatchFace with basic properties
 */
export function createDefaultWatchFace(deviceConfig?: DeviceConfiguration): WatchFace {
  const id = generateId();
  const now = new Date();
  
  return {
    id,
    createTime: now,
    updateTime: now,
    watchName: 'New Watch Face',
    watchCode: `WF_${Date.now()}`,
    width: deviceConfig?.width ?? 390,
    height: deviceConfig?.height ?? 450,
    thumbnailWidth: deviceConfig?.thumbnailWidth ?? 220,
    thumbnailHeight: deviceConfig?.thumbnailHeight ?? 220,
    corner: deviceConfig?.corner ?? 5,
    deviceType: deviceConfig?.deviceType ?? 'SmartWatch',
    isAlbum: false,
    colorBit: 16,
    folderName: '',
    watchStyles: [],
  };
}

/**
 * Creates a default device configuration
 */
export function createDefaultDeviceConfig(): DeviceConfiguration {
  return {
    name: 'Default Device',
    deviceType: 'SmartWatch',
    width: 390,
    height: 450,
    thumbnailWidth: 220,
    thumbnailHeight: 220,
    corner: 5,
    dpi: 326,
    colorDepth: 16,
  };
}

/**
 * Creates a base drag element with common properties
 */
export function createBaseDragElement(type: string): DragBindBase {
  return {
    id: generateId(),
    createTime: new Date(),
    updateTime: new Date(),
    dragId: generateId(),
    dragName: `${type}_${Date.now()}`,
    visible: true,
    left: 0,
    top: 0,
    width: 100,
    height: 100,
    zIndex: 1,
  };
}

/**
 * Creates a drag image element
 */
export function createDragImage(source?: string): DragImage {
  return {
    ...createBaseDragElement('DragImage'),
    source: source ?? '',
    opacity: 1,
  };
}

/**
 * Creates a drag widget element
 */
export function createDragWidget(widgetType?: string): DragWidget {
  return {
    ...createBaseDragElement('DragWidget'),
    widgetType: widgetType ?? 'generic',
    properties: {},
  };
}

/**
 * Validates a WatchFace object
 */
export function validateWatchFace(watchFace: Partial<WatchFace>): ValidationResult {
  const errors: ValidationError[] = [];

  if (!watchFace.watchName || watchFace.watchName.trim().length === 0) {
    errors.push({
      field: 'watchName',
      message: 'Watch name is required',
      code: 'REQUIRED',
    });
  }

  if (!watchFace.width || watchFace.width <= 0) {
    errors.push({
      field: 'width',
      message: 'Width must be greater than 0',
      code: 'INVALID_VALUE',
    });
  }

  if (!watchFace.height || watchFace.height <= 0) {
    errors.push({
      field: 'height',
      message: 'Height must be greater than 0',
      code: 'INVALID_VALUE',
    });
  }

  if (watchFace.watchName && watchFace.watchName.length > 50) {
    errors.push({
      field: 'watchName',
      message: 'Watch name cannot exceed 50 characters',
      code: 'MAX_LENGTH_EXCEEDED',
    });
  }

  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Validates a drag element
 */
export function validateDragElement(element: Partial<DragBindBase>): ValidationResult {
  const errors: ValidationError[] = [];

  if (!element.dragName || element.dragName.trim().length === 0) {
    errors.push({
      field: 'dragName',
      message: 'Element name is required',
      code: 'REQUIRED',
    });
  }

  if (element.left !== undefined && element.left < 0) {
    errors.push({
      field: 'left',
      message: 'Left position cannot be negative',
      code: 'INVALID_VALUE',
    });
  }

  if (element.top !== undefined && element.top < 0) {
    errors.push({
      field: 'top',
      message: 'Top position cannot be negative',
      code: 'INVALID_VALUE',
    });
  }

  if (element.width !== undefined && element.width <= 0) {
    errors.push({
      field: 'width',
      message: 'Width must be greater than 0',
      code: 'INVALID_VALUE',
    });
  }

  if (element.height !== undefined && element.height <= 0) {
    errors.push({
      field: 'height',
      message: 'Height must be greater than 0',
      code: 'INVALID_VALUE',
    });
  }

  return {
    isValid: errors.length === 0,
    errors,
  };
}

/**
 * Converts a WatchFace to export format
 */
export function convertToExportFormat(watchFace: WatchFace): WatchFaceOutput {
  return {
    id: watchFace.id ?? generateId(),
    name: watchFace.watchName,
    width: watchFace.width,
    height: watchFace.height,
    deviceType: watchFace.deviceType,
    themes: [], // Will be populated by theme conversion
    resources: {
      translations: [],
      images: [],
      imageArrays: [],
      dataItemImageValues: [],
      dataItemImageNumbers: [],
      dataItemImagePoints: [],
      widgets: [],
      slots: [],
      sprites: [],
    },
    elementCache: {},
  };
}

/**
 * Deep clones a model object
 */
export function cloneModel<T>(model: T): T {
  return JSON.parse(JSON.stringify(model));
}

/**
 * Merges model updates while preserving timestamps
 */
export function mergeModelUpdate<T extends { updateTime?: Date }>(
  current: T,
  updates: Partial<T>
): T {
  return {
    ...current,
    ...updates,
    updateTime: new Date(),
  };
}

/**
 * Sanitizes file names for safe usage
 */
export function sanitizeFileName(fileName: string): string {
  return fileName
    .replace(/[^a-zA-Z0-9._-]/g, '_')
    .replace(/_{2,}/g, '_')
    .replace(/^_|_$/g, '');
}

/**
 * Calculates folder name based on watch face properties
 */
export function calculateFolderName(watchFace: Partial<WatchFace>): string {
  const code = watchFace.watchCode ?? 'WF';
  const name = sanitizeFileName(watchFace.watchName ?? 'Untitled');
  const device = watchFace.deviceType ?? 'Unknown';
  
  return `${code}_${name}_${device}`;
}

/**
 * Formats file size in human readable format
 */
export function formatFileSize(bytes: number): string {
  const units = ['B', 'KB', 'MB', 'GB'];
  let size = bytes;
  let unitIndex = 0;

  while (size >= 1024 && unitIndex < units.length - 1) {
    size /= 1024;
    unitIndex++;
  }

  return `${Math.round(size * 100) / 100} ${units[unitIndex]}`;
}

/**
 * Formats date for display
 */
export function formatDate(date: Date, format: 'short' | 'medium' | 'long' = 'medium'): string {
  let options: Intl.DateTimeFormatOptions;
  
  switch (format) {
    case 'short':
      options = { month: 'short', day: 'numeric', year: '2-digit' };
      break;
    case 'long':
      options = { 
        weekday: 'long', 
        month: 'long', 
        day: 'numeric', 
        year: 'numeric', 
        hour: '2-digit', 
        minute: '2-digit' 
      };
      break;
    default:
      options = { month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit' };
  }

  return new Intl.DateTimeFormat('en-US', options).format(date);
}

// Helper types
export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}

export interface ValidationError {
  field: string;
  message: string;
  code: string;
  value?: any;
}

// Constants for validation
export const VALIDATION_CONSTANTS = {
  MAX_WATCH_NAME_LENGTH: 50,
  MAX_ELEMENT_NAME_LENGTH: 30,
  MIN_CANVAS_SIZE: 50,
  MAX_CANVAS_SIZE: 2000,
  MIN_ELEMENT_SIZE: 1,
  MAX_ELEMENT_SIZE: 1000,
  SUPPORTED_IMAGE_FORMATS: ['png', 'jpg', 'jpeg', 'gif', 'webp'],
  MAX_FILE_SIZE: 10 * 1024 * 1024, // 10MB
} as const;