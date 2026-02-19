// Canvas and design related models
import type { UUID } from '@/types';

// Temporary fabric type until we add proper types
type FabricObject = any;
export interface CanvasState {
  id: UUID;
  name: string;
  width: number;
  height: number;
  zoom: number;
  panX: number;
  panY: number;
  backgroundColor?: string;
  gridVisible: boolean;
  gridSize: number;
  snapToGrid: boolean;
  selectedElementIds: UUID[];
  elements: CanvasElement[];
  layers: CanvasLayer[];
  mode: CanvasMode;
}

export enum CanvasMode {
  DESIGN = 'design',
  PREVIEW = 'preview',
  EXPORT = 'export',
  SELECT = 'select',
  PAN = 'pan',
  ZOOM = 'zoom',
}

export interface CanvasElement {
  id: UUID;
  type: ElementType;
  name: string;
  layerId: UUID;
  fabricObject?: FabricObject; // Reference to Fabric.js object
  properties: ElementProperties;
  transform: ElementTransform;
  style: ElementStyle;
  isLocked: boolean;
  isVisible: boolean;
  isSelected: boolean;
}

export enum ElementType {
  IMAGE = 'image',
  TEXT = 'text',
  SHAPE = 'shape',
  WIDGET = 'widget',
  GROUP = 'group',
  BACKGROUND = 'background',
  TIME_DISPLAY = 'time_display',
  DATE_DISPLAY = 'date_display',
  BATTERY_INDICATOR = 'battery_indicator',
  STEP_COUNTER = 'step_counter',
  WEATHER_WIDGET = 'weather_widget',
  PROGRESS_BAR = 'progress_bar',
  ANIMATION = 'animation',
}

export interface CanvasLayer {
  id: UUID;
  name: string;
  isVisible: boolean;
  isLocked: boolean;
  opacity: number;
  blendMode: BlendMode;
  elementIds: UUID[];
  order: number;
}

export enum BlendMode {
  NORMAL = 'normal',
  MULTIPLY = 'multiply',
  SCREEN = 'screen',
  OVERLAY = 'overlay',
  SOFT_LIGHT = 'soft-light',
  HARD_LIGHT = 'hard-light',
  COLOR_DODGE = 'color-dodge',
  COLOR_BURN = 'color-burn',
  DARKEN = 'darken',
  LIGHTEN = 'lighten',
  DIFFERENCE = 'difference',
  EXCLUSION = 'exclusion',
}

export interface ElementProperties {
  // Common properties
  dataBinding?: DataBinding;
  animations?: Animation[];
  interactions?: Interaction[];
  
  // Type-specific properties
  [key: string]: any;
}

export interface ElementTransform {
  x: number;
  y: number;
  width: number;
  height: number;
  rotation: number; // degrees
  scaleX: number;
  scaleY: number;
  skewX: number;
  skewY: number;
  originX: 'left' | 'center' | 'right';
  originY: 'top' | 'center' | 'bottom';
}

export interface ElementStyle {
  opacity: number;
  visible: boolean;
  fill?: string | GradientFill | PatternFill;
  stroke?: string;
  strokeWidth?: number;
  strokeDashArray?: number[];
  shadow?: Shadow;
  blur?: number;
  filters?: Filter[];
}

export interface GradientFill {
  type: 'linear' | 'radial';
  colorStops: ColorStop[];
  angle?: number; // for linear gradients
  centerX?: number; // for radial gradients
  centerY?: number; // for radial gradients
  radius?: number; // for radial gradients
}

export interface PatternFill {
  type: 'image' | 'pattern';
  source: string;
  repeat: 'repeat' | 'repeat-x' | 'repeat-y' | 'no-repeat';
  offsetX?: number;
  offsetY?: number;
  scaleX?: number;
  scaleY?: number;
}

export interface ColorStop {
  offset: number; // 0-1
  color: string;
}

export interface Shadow {
  color: string;
  blur: number;
  offsetX: number;
  offsetY: number;
}

export interface Filter {
  type: 'blur' | 'brightness' | 'contrast' | 'grayscale' | 'hue-rotate' | 'invert' | 'saturate' | 'sepia';
  value: number;
}

export interface DataBinding {
  type: DataBindingType;
  source: string;
  format?: string;
  updateInterval?: number; // ms
  condition?: string; // expression for conditional display
}

export enum DataBindingType {
  STATIC = 'static',
  TIME = 'time',
  DATE = 'date',
  BATTERY = 'battery',
  STEPS = 'steps',
  HEART_RATE = 'heart_rate',
  WEATHER = 'weather',
  NOTIFICATIONS = 'notifications',
  CALORIES = 'calories',
  DISTANCE = 'distance',
  CUSTOM = 'custom',
}

export interface Animation {
  id: UUID;
  name: string;
  type: AnimationType;
  duration: number; // ms
  delay?: number; // ms
  iterations: number | 'infinite';
  direction: 'normal' | 'reverse' | 'alternate' | 'alternate-reverse';
  easing: EasingFunction;
  keyframes: Keyframe[];
  trigger?: AnimationTrigger;
}

export enum AnimationType {
  FADE = 'fade',
  SLIDE = 'slide',
  SCALE = 'scale',
  ROTATE = 'rotate',
  BOUNCE = 'bounce',
  PULSE = 'pulse',
  SHAKE = 'shake',
  FLIP = 'flip',
  CUSTOM = 'custom',
}

export interface Keyframe {
  offset: number; // 0-1
  properties: Partial<ElementTransform & ElementStyle>;
}

export interface AnimationTrigger {
  type: 'time' | 'event' | 'condition';
  value: string | number;
}

export enum EasingFunction {
  LINEAR = 'linear',
  EASE = 'ease',
  EASE_IN = 'ease-in',
  EASE_OUT = 'ease-out',
  EASE_IN_OUT = 'ease-in-out',
  CUBIC_BEZIER = 'cubic-bezier',
}

export interface Interaction {
  id: UUID;
  type: InteractionType;
  trigger: InteractionTrigger;
  action: InteractionAction;
  condition?: string;
}

export enum InteractionType {
  TAP = 'tap',
  DOUBLE_TAP = 'double-tap',
  LONG_PRESS = 'long-press',
  SWIPE = 'swipe',
  PINCH = 'pinch',
  HOVER = 'hover',
}

export interface InteractionTrigger {
  type: InteractionType;
  area?: 'element' | 'screen';
  direction?: 'up' | 'down' | 'left' | 'right'; // for swipe
}

export interface InteractionAction {
  type: 'navigate' | 'toggle' | 'animate' | 'vibrate' | 'sound' | 'custom';
  target?: string;
  parameters?: Record<string, any>;
}

// Selection and clipboard models
export interface SelectionState {
  selectedElements: UUID[];
  selectionBounds?: SelectionBounds;
  multiSelect: boolean;
}

export interface SelectionBounds {
  x: number;
  y: number;
  width: number;
  height: number;
  rotation: number;
}

export interface ClipboardData {
  type: 'elements';
  elements: CanvasElement[];
  timestamp: Date;
}

// History and undo/redo models
export interface HistoryEntry {
  id: UUID;
  timestamp: Date;
  action: HistoryAction;
  description: string;
  data: any;
  canvasState?: Partial<CanvasState>;
}

export interface HistoryAction {
  type: 'create' | 'update' | 'delete' | 'move' | 'resize' | 'rotate' | 'style' | 'group' | 'ungroup';
  elementIds: UUID[];
  before?: any;
  after?: any;
}

// Grid and guides
export interface GridSettings {
  visible: boolean;
  enabled: boolean;
  size: number;
  color: string;
  opacity: number;
  type: 'dots' | 'lines';
}

export interface Guide {
  id: UUID;
  type: 'horizontal' | 'vertical';
  position: number;
  color: string;
  isVisible: boolean;
}

// Export and preview models
export interface CanvasExportSettings {
  format: 'png' | 'jpg' | 'svg' | 'pdf' | 'bin';
  quality: number; // 0-1 for jpeg
  scale: number; // export scale factor
  backgroundColor?: string;
  transparent: boolean; // for png
  includeHiddenLayers: boolean;
  compression?: boolean; // for bin format
}

export interface PreviewSettings {
  deviceFrame: boolean;
  deviceType: string;
  orientation: 'portrait' | 'landscape';
  scale: number;
  backgroundColor?: string;
}