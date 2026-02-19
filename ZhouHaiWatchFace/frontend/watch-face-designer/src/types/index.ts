// Global type declarations for the Watch Face Designer

declare global {
  interface Window {
    // Add any global window properties here
  }
}

// Basic utility types
export type UUID = string;

export interface Point {
  x: number;
  y: number;
}

export interface Size {
  width: number;
  height: number;
}

export interface Rect extends Point, Size {}

// Watch element types
export enum WatchElementType {
  IMAGE = 'image',
  TEXT = 'text',
  SHAPE = 'shape',
  PROGRESS = 'progress',
  ANALOG_CLOCK = 'analog_clock',
  DIGITAL_CLOCK = 'digital_clock',
  DATE = 'date',
  BATTERY = 'battery',
  WEATHER = 'weather',
  STEPS = 'steps',
  HEART_RATE = 'heart_rate',
  CUSTOM = 'custom',
}

// API Response types
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}