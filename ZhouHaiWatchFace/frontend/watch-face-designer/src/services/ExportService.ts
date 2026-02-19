// Export service for generating watch face data in the original app's format
import { CanvasStore } from '../stores/CanvasStore';
import { AppStore } from '../stores/AppStore';
import { ElementType } from '../models';
import type { CanvasElement } from '../models';

export interface WatchFaceExport {
  name: string;
  folderName: string;
  createTime: string;
  modifyTime: string;
  description?: string;
  author?: string;
  version: string;
  watchStyles: WatchStyleExport[];
  deviceWidth: number;
  deviceHeight: number;
}

export interface WatchStyleExport {
  styleName: string;
  isDefault: boolean;
  dragBindBases: DragBindBaseExport[];
}

export interface DragBindBaseExport {
  id: string;
  elementType: ElementType;
  name: string;
  x: number;
  y: number;
  width: number;
  height: number;
  rotation: number;
  scaleX: number;
  scaleY: number;
  isVisible: boolean;
  isLocked: boolean;
  opacity: number;
  zIndex: number;
  
  // Type-specific properties
  properties: Record<string, any>;
  
  // Data binding information
  dataBinding?: {
    type: string;
    value: string;
    format?: string;
  };
}

export class ExportService {
  constructor(
    private canvasStore: CanvasStore,
    private appStore: AppStore
  ) {}

  /**
   * Export the current watch face design to JSON format compatible with the original app
   */
  exportToJSON(): WatchFaceExport {
    const currentProject = this.appStore.currentProject;
    const canvasState = this.canvasStore.canvasState;
    
    const watchFace: WatchFaceExport = {
      name: currentProject?.name || 'Untitled Watch Face',
      folderName: currentProject?.name?.replace(/\s+/g, '_') || 'untitled_watch_face',
      createTime: currentProject?.createTime?.toISOString() || new Date().toISOString(),
      modifyTime: new Date().toISOString(),
      description: currentProject?.description || '',
      author: 'Web Designer',
      version: '1.0.0',
      deviceWidth: canvasState.width,
      deviceHeight: canvasState.height,
      watchStyles: [
        {
          styleName: 'Default',
          isDefault: true,
          dragBindBases: this.convertElementsToDragBindBases(canvasState.elements)
        }
      ]
    };

    return watchFace;
  }

  /**
   * Convert canvas elements to DragBindBase format used by the original app
   */
  private convertElementsToDragBindBases(elements: CanvasElement[]): DragBindBaseExport[] {
    return elements.map((element, index) => ({
      id: element.id,
      elementType: element.type,
      name: element.name || `Element_${index + 1}`,
      x: element.transform.x,
      y: element.transform.y,
      width: element.transform.width,
      height: element.transform.height,
      rotation: element.transform.rotation || 0,
      scaleX: element.transform.scaleX || 1,
      scaleY: element.transform.scaleY || 1,
      isVisible: element.isVisible,
      isLocked: element.isLocked,
      opacity: (element as any).opacity || 1,
      zIndex: (element as any).zIndex || index,
      properties: this.convertElementProperties(element),
      dataBinding: this.extractDataBinding(element)
    }));
  }

  /**
   * Convert element properties to match original app format
   */
  private convertElementProperties(element: CanvasElement): Record<string, any> {
    const baseProperties = {
      ...element.properties
    };

    // Type-specific property conversions
    switch (element.type) {
      case ElementType.TEXT:
        return {
          ...baseProperties,
          text: baseProperties.text || 'Sample Text',
          fontSize: baseProperties.fontSize || 16,
          fontFamily: baseProperties.fontFamily || 'Arial',
          color: baseProperties.color || '#FFFFFF',
          textAlign: baseProperties.textAlign || 'left',
          fontWeight: baseProperties.fontWeight || 'normal'
        };

      case ElementType.IMAGE:
        return {
          ...baseProperties,
          src: baseProperties.src || '',
          alt: baseProperties.alt || '',
          preserveAspectRatio: baseProperties.preserveAspectRatio ?? true
        };

      case ElementType.SHAPE:
        return {
          ...baseProperties,
          shapeType: baseProperties.shapeType || 'rectangle',
          fill: baseProperties.fill || '#FFFFFF',
          stroke: baseProperties.stroke || 'none',
          strokeWidth: baseProperties.strokeWidth || 0
        };

      case ElementType.TEXT:
        return {
          ...baseProperties,
          stroke: baseProperties.stroke || '#FFFFFF',
          strokeWidth: baseProperties.strokeWidth || 1,
          strokeDashArray: baseProperties.strokeDashArray || []
        };

      default:
        return baseProperties;
    }
  }

  /**
   * Extract data binding information from element properties
   */
  private extractDataBinding(element: CanvasElement): { type: string; value: string; format?: string } | undefined {
    // Check if element has data binding based on its type and properties
    switch (element.type) {
      case ElementType.TEXT:
        if (element.properties.dataSource) {
          return {
            type: 'text',
            value: element.properties.dataSource,
            format: element.properties.dataFormat
          };
        }
        break;

      case ElementType.IMAGE:
        if (element.properties.dynamicSource) {
          return {
            type: 'image',
            value: element.properties.dynamicSource,
            format: element.properties.dataFormat
          };
        }
        break;
    }

    return undefined;
  }

  /**
   * Download the exported JSON file
   */
  downloadJSON(filename?: string): void {
    const watchFace = this.exportToJSON();
    const jsonString = JSON.stringify(watchFace, null, 2);
    const blob = new Blob([jsonString], { type: 'application/json' });
    
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename || `${watchFace.folderName}.json`;
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    URL.revokeObjectURL(url);
  }

  /**
   * Generate preview data for the watch face
   */
  generatePreviewData(): {
    thumbnail: string;
    metadata: Partial<WatchFaceExport>;
  } {
    const watchFace = this.exportToJSON();
    
    // Generate thumbnail (this would need canvas-to-image conversion)
    const thumbnail = this.generateThumbnail();
    
    return {
      thumbnail,
      metadata: {
        name: watchFace.name,
        author: watchFace.author,
        createTime: watchFace.createTime,
        deviceWidth: watchFace.deviceWidth,
        deviceHeight: watchFace.deviceHeight,
        version: watchFace.version
      }
    };
  }

  /**
   * Generate a thumbnail image of the current canvas
   */
  private generateThumbnail(): string {
    // This would integrate with the Fabric.js canvas to generate a data URL
    // For now, return a placeholder
    return 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==';
  }

  /**
   * Validate the exported data structure
   */
  validateExport(watchFace: WatchFaceExport): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!watchFace.name?.trim()) {
      errors.push('Watch face name is required');
    }

    if (!watchFace.folderName?.trim()) {
      errors.push('Folder name is required');
    }

    if (watchFace.deviceWidth <= 0 || watchFace.deviceHeight <= 0) {
      errors.push('Invalid device dimensions');
    }

    if (!watchFace.watchStyles?.length) {
      errors.push('At least one watch style is required');
    }

    // Validate each style
    watchFace.watchStyles?.forEach((style, index) => {
      if (!style.styleName?.trim()) {
        errors.push(`Style ${index + 1}: Name is required`);
      }

      style.dragBindBases?.forEach((element, elemIndex) => {
        if (!element.id?.trim()) {
          errors.push(`Style ${index + 1}, Element ${elemIndex + 1}: ID is required`);
        }

        if (!element.elementType) {
          errors.push(`Style ${index + 1}, Element ${elemIndex + 1}: Element type is required`);
        }
      });
    });

    return {
      isValid: errors.length === 0,
      errors
    };
  }
}