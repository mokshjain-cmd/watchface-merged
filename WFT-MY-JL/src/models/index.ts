/**
 * Complete TypeScript models converted from C# WatchControlLibrary
 * This file exports all the drag components and related models
 */

// Core base classes
export { DragDataBase, DragBindBase, BindMonitorType, Align } from './DragDataBase';
export type { IDraggable } from './DragDataBase';
export { DragImageSource } from './DragImageSource';

// Data types and helpers
export { DataItemType, DataItemTypeHelper } from './DataItemType';
export { DragDateTimeHelper, MonthDayModeEnum } from './DragDateTimeHelper';
export { CommonHelper } from './CommonHelper';

// Single digit components
export { DragSingleDigit } from './DragSingleDigit';

// Comprehensive drag components
export { 
  DragBindNormalDateTime,
  DragBindProgress,
  DragBindNums,
  DragBindSwitch,
  DragBindAMPM,
  DragBindImage
} from './DragComponents';

// Specialized components
export { DragMonthDay, DragBindMonthDay } from './DragMonthDay';
export { DragNums, DragBindNums as DragBindNumsSpecialized } from './DragNums';
export { DragProgress, DragBindProgress as DragBindProgressSpecialized } from './DragProgress';
export { DragWeek, DragBindWeek, DateTimeType } from './DragWeek';
export { DragWidget, Orientation, Direction } from './DragWidget';

/**
 * Summary of converted C# files:
 * 
 * ✅ DataItemType.cs → DataItemType.ts (Complete enum system with 100+ data types)
 * ✅ DragDataBase.cs → DragDataBase.ts (Base classes with property notification)
 * ✅ BindingHelper.cs → BindingHelper.ts (Property binding system)
 * ✅ DragSingleDigit.cs → DragSingleDigit.ts (Single digit display components)
 * ✅ DragDateTimeHelper.cs → DragDateTimeHelper.ts (Date/time formatting utilities)
 * ✅ CommonHelper.cs → CommonHelper.ts (Common utility functions)
 * ✅ DragImageSource.cs → DragImageSource.ts (Image-based component base class)
 * ✅ DragMonthDay.cs → DragMonthDay.ts (Month/day display components)
 * ✅ DragNums.cs → DragNums.ts (Numeric display components)
 * ✅ DragProgress.cs → DragProgress.ts (Progress bar components)
 * ✅ DragWeek.cs → DragWeek.ts (Week/date display components)
 * ✅ DragWidget.cs → DragWidget.ts (Container/layout widget)
 * ✅ DragComponents.ts (Comprehensive collection of all drag component types)
 * 
 * Features implemented:
 * - Complete type system matching C# enums and classes
 * - Property change notification system
 * - XML output generation for each component type
 * - Image loading and size calculation
 * - Date/time formatting and manipulation
 * - Layout and positioning for container widgets
 * - Validation and error handling
 * - Deep cloning and serialization support
 * 
 * Remaining files to convert (~30 more):
 * - DragAMPM.cs
 * - DragAnimFrame.cs  
 * - DragDouble.cs
 * - DragImage.cs
 * - DragSlot.cs
 * - DragSwitch.cs
 * - DraggableBehavior.cs
 * - BitmapImageHelper.cs
 * - ImageCache.cs
 * - MD5Helper.cs
 * - OutXmlHelper.cs
 * - StaticData.cs
 * - TreeViewEx.cs
 * - And various other helper classes
 */

/**
 * Component type registry for factory pattern
 * Note: Some components are temporarily commented out due to import issues
 */
// export const ComponentTypeRegistry = {
//   'DragNormalDateTime': DragBindNormalDateTime,
//   'DragSingleDigit': DragSingleDigit,
//   'DragNums': DragBindNums,
//   'DragProgress': DragBindProgress,
//   'DragSwitch': DragBindSwitch,
//   'DragAMPM': DragBindAMPM,
//   'DragImage': DragBindImage,
//   'DragMonthDay': DragBindMonthDay,
//   'DragWeek': DragBindWeek,
//   'DragWidget': DragWidget
// } as const;

// export type ComponentType = keyof typeof ComponentTypeRegistry;

/**
 * Factory function to create components by type
 */
// export function createComponent(type: ComponentType, ...args: any[]): DragBindBase | DragWidget {
//   const ComponentClass = ComponentTypeRegistry[type];
//   return new ComponentClass(...args);
// }

/**
 * Validation helper for component types
 */
// export function isValidComponentType(type: string): type is ComponentType {
//   return type in ComponentTypeRegistry;
// }

/**
 * Get all available component types
 */
// export function getAvailableComponentTypes(): ComponentType[] {
//   return Object.keys(ComponentTypeRegistry) as ComponentType[];
// }