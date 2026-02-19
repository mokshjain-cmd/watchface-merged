// Helper functions for working with CanvasElement properties
import type { CanvasElement, ElementTransform, ElementStyle } from '@/models/Canvas';
import { ElementType } from '@/models/Canvas';
import { WatchElementType } from '@/types';

/**
 * Maps WatchElementType to ElementType
 */
export function mapWatchElementTypeToElementType(watchType: WatchElementType): ElementType {
  switch (watchType) {
    case WatchElementType.IMAGE:
      return ElementType.IMAGE;
    case WatchElementType.TEXT:
      return ElementType.TEXT;
    case WatchElementType.SHAPE:
      return ElementType.SHAPE;
    case WatchElementType.PROGRESS:
      return ElementType.PROGRESS_BAR;
    case WatchElementType.ANALOG_CLOCK:
      return ElementType.TIME_DISPLAY;
    case WatchElementType.DIGITAL_CLOCK:
      return ElementType.TIME_DISPLAY;
    case WatchElementType.DATE:
      return ElementType.DATE_DISPLAY;
    case WatchElementType.BATTERY:
      return ElementType.BATTERY_INDICATOR;
    case WatchElementType.WEATHER:
      return ElementType.WEATHER_WIDGET;
    case WatchElementType.STEPS:
      return ElementType.STEP_COUNTER;
    case WatchElementType.HEART_RATE:
      return ElementType.WIDGET;
    case WatchElementType.CUSTOM:
      return ElementType.WIDGET;
    default:
      return ElementType.WIDGET;
  }
}

/**
 * Gets a property value from the element with fallback
 */
export function getElementProperty<T>(element: CanvasElement, key: string, defaultValue: T): T {
  return (element.properties[key] as T) ?? defaultValue;
}

/**
 * Gets transform values
 */
export function getElementTransform(element: CanvasElement): ElementTransform {
  return element.transform;
}

/**
 * Gets style values
 */
export function getElementStyle(element: CanvasElement): ElementStyle {
  return element.style;
}

/**
 * Creates a simplified element interface for Fabric.js operations
 */
export interface SimplifiedElement {
  id: string;
  type: ElementType;
  x: number;
  y: number;
  width: number;
  height: number;
  rotation: number;
  scaleX: number;
  scaleY: number;
  opacity: number;
  visible: boolean;
  locked: boolean;
  
  // Style properties
  fill?: string;
  stroke?: string;
  strokeWidth?: number;
  
  // Type-specific properties
  text?: string;
  fontSize?: number;
  fontFamily?: string;
  fontWeight?: string;
  fontStyle?: string;
  textAlign?: string;
  
  imageSrc?: string;
  
  shapeType?: string;
  radius?: number;
  rx?: number;
  ry?: number;
  
  progressType?: string;
  startAngle?: number;
  endAngle?: number;
  
  path?: any;
}

/**
 * Converts CanvasElement to SimplifiedElement for easier Fabric.js operations
 */
export function toSimplifiedElement(element: CanvasElement): SimplifiedElement {
  const transform = element.transform;
  const style = element.style;
  
  const simplified: SimplifiedElement = {
    id: element.id,
    type: element.type,
    x: transform.x,
    y: transform.y,
    width: transform.width,
    height: transform.height,
    rotation: transform.rotation,
    scaleX: transform.scaleX,
    scaleY: transform.scaleY,
    opacity: style.opacity,
    visible: style.visible,
    locked: element.isLocked,
    fill: typeof style.fill === 'string' ? style.fill : undefined,
    stroke: style.stroke,
    strokeWidth: style.strokeWidth,
  };
  
  // Add type-specific properties
  if (element.type === ElementType.TEXT) {
    simplified.text = getElementProperty(element, 'text', 'Text');
    simplified.fontSize = getElementProperty(element, 'fontSize', 16);
    simplified.fontFamily = getElementProperty(element, 'fontFamily', 'Arial');
    simplified.fontWeight = getElementProperty(element, 'fontWeight', 'normal');
    simplified.fontStyle = getElementProperty(element, 'fontStyle', 'normal');
    simplified.textAlign = getElementProperty(element, 'textAlign', 'left');
  }
  
  if (element.type === ElementType.IMAGE) {
    simplified.imageSrc = getElementProperty(element, 'imageSrc', '');
  }
  
  if (element.type === ElementType.SHAPE) {
    simplified.shapeType = getElementProperty(element, 'shapeType', 'rect');
    simplified.radius = getElementProperty(element, 'radius', 50);
    simplified.rx = getElementProperty(element, 'rx', 50);
    simplified.ry = getElementProperty(element, 'ry', 30);
  }
  
  if (element.type === ElementType.PROGRESS_BAR) {
    simplified.progressType = getElementProperty(element, 'progressType', 'arc');
    simplified.startAngle = getElementProperty(element, 'startAngle', 0);
    simplified.endAngle = getElementProperty(element, 'endAngle', Math.PI);
    simplified.radius = getElementProperty(element, 'radius', 50);
  }
  
  return simplified;
}

/**
 * Updates CanvasElement from SimplifiedElement changes
 */
export function updateElementFromSimplified(
  element: CanvasElement, 
  updates: Partial<SimplifiedElement>
): Partial<CanvasElement> {
  const elementUpdates: Partial<CanvasElement> = {};
  
  // Transform updates
  if (updates.x !== undefined || updates.y !== undefined || updates.width !== undefined || 
      updates.height !== undefined || updates.rotation !== undefined || 
      updates.scaleX !== undefined || updates.scaleY !== undefined) {
    elementUpdates.transform = {
      ...element.transform,
      ...(updates.x !== undefined && { x: updates.x }),
      ...(updates.y !== undefined && { y: updates.y }),
      ...(updates.width !== undefined && { width: updates.width }),
      ...(updates.height !== undefined && { height: updates.height }),
      ...(updates.rotation !== undefined && { rotation: updates.rotation }),
      ...(updates.scaleX !== undefined && { scaleX: updates.scaleX }),
      ...(updates.scaleY !== undefined && { scaleY: updates.scaleY }),
    };
  }
  
  // Style updates
  if (updates.opacity !== undefined || updates.visible !== undefined || 
      updates.fill !== undefined || updates.stroke !== undefined || 
      updates.strokeWidth !== undefined) {
    elementUpdates.style = {
      ...element.style,
      ...(updates.opacity !== undefined && { opacity: updates.opacity }),
      ...(updates.visible !== undefined && { visible: updates.visible }),
      ...(updates.fill !== undefined && { fill: updates.fill }),
      ...(updates.stroke !== undefined && { stroke: updates.stroke }),
      ...(updates.strokeWidth !== undefined && { strokeWidth: updates.strokeWidth }),
    };
  }
  
  // Lock state update
  if (updates.locked !== undefined) {
    elementUpdates.isLocked = updates.locked;
  }
  
  // Property updates
  const propertyUpdates: Record<string, any> = {};
  if (updates.text !== undefined) propertyUpdates.text = updates.text;
  if (updates.fontSize !== undefined) propertyUpdates.fontSize = updates.fontSize;
  if (updates.fontFamily !== undefined) propertyUpdates.fontFamily = updates.fontFamily;
  if (updates.fontWeight !== undefined) propertyUpdates.fontWeight = updates.fontWeight;
  if (updates.fontStyle !== undefined) propertyUpdates.fontStyle = updates.fontStyle;
  if (updates.textAlign !== undefined) propertyUpdates.textAlign = updates.textAlign;
  if (updates.imageSrc !== undefined) propertyUpdates.imageSrc = updates.imageSrc;
  if (updates.shapeType !== undefined) propertyUpdates.shapeType = updates.shapeType;
  if (updates.radius !== undefined) propertyUpdates.radius = updates.radius;
  if (updates.rx !== undefined) propertyUpdates.rx = updates.rx;
  if (updates.ry !== undefined) propertyUpdates.ry = updates.ry;
  if (updates.progressType !== undefined) propertyUpdates.progressType = updates.progressType;
  if (updates.startAngle !== undefined) propertyUpdates.startAngle = updates.startAngle;
  if (updates.endAngle !== undefined) propertyUpdates.endAngle = updates.endAngle;
  if (updates.path !== undefined) propertyUpdates.path = updates.path;
  
  if (Object.keys(propertyUpdates).length > 0) {
    elementUpdates.properties = {
      ...element.properties,
      ...propertyUpdates,
    };
  }
  
  return elementUpdates;
}