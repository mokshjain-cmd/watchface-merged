// Enums and constants for the Watch Face Designer

export enum DragElementType {
  IMAGE = 'DragImage',
  WIDGET = 'DragWidget',
  NUMS = 'DragNums',
  PROGRESS = 'DragProgress',
  WEEK = 'DragWeek',
  AMPM = 'DragAMPM',
  SWITCH = 'DragSwitch',
  SLOT = 'DragSlot',
  POINT = 'DragPoint',
  NORMAL_DATETIME = 'DragNormalDateTime',
  SINGLE_DIGIT = 'DragSingleDigit',
  MONTH_DAY = 'DragMonthDay',
  DOUBLE = 'DragDouble',
  KEY_VALUE = 'DragKeyValue',
  ANIM_FRAME = 'DragAnimFrame',
}

export enum DeviceType {
  SMARTWATCH = 'SmartWatch',
  FITNESS_TRACKER = 'FitnessTracker',
}

export enum CanvasMode {
  DESIGN = 'design',
  PREVIEW = 'preview',
  EXPORT = 'export',
}

export enum FileFormat {
  JSON = 'json',
  BIN = 'bin',
  PNG = 'png',
  JPG = 'jpg',
}

export const DEFAULT_CANVAS_SIZE = {
  width: 390,
  height: 450,
} as const;

export const DEFAULT_THUMBNAIL_SIZE = {
  width: 220,
  height: 220,
} as const;