import { fabric } from 'fabric';
import { CanvasStore } from '@/stores/CanvasStore';
import { DesignStore } from '@/stores/DesignStore';
import type { CanvasElement } from '@/models/Canvas';
import { ElementType } from '@/models/Canvas';
import type { WatchStyle } from '@/models/WatchFace';
import type { SimplifiedElement } from './CanvasHelpers';
import { toSimplifiedElement } from './CanvasHelpers';

/**
 * CanvasService manages the Fabric.js canvas and integrates with MobX stores
 * This service handles the bridge between Fabric.js objects and our data models
 */
export class CanvasService {
  private canvas: fabric.Canvas | null = null;
  private canvasStore: CanvasStore;
  private designStore: DesignStore;
  private elementMap = new Map<string, fabric.Object>();
  private isUpdating = false;

  constructor(canvasStore: CanvasStore, designStore: DesignStore) {
    this.canvasStore = canvasStore;
    this.designStore = designStore;
  }

  /**
   * Initialize the Fabric.js canvas
   */
  initializeCanvas(canvasElement: HTMLCanvasElement): fabric.Canvas {
    if (this.canvas) {
      this.dispose();
    }

    this.canvas = new fabric.Canvas(canvasElement, {
      width: 400,
      height: 400,
      backgroundColor: '#000000',
      selection: true,
      preserveObjectStacking: true,
    });

    this.setupEventHandlers();
    this.updateCanvasSize();
    
    return this.canvas;
  }

  /**
   * Setup event handlers for canvas interactions
   */
  private setupEventHandlers() {
    if (!this.canvas) return;

    // Selection events
    this.canvas.on('selection:created', (e) => {
      this.handleSelectionChanged(e);
    });

    this.canvas.on('selection:updated', (e) => {
      this.handleSelectionChanged(e);
    });

    this.canvas.on('selection:cleared', () => {
      this.canvasStore.clearSelection();
    });

    // Object modification events
    this.canvas.on('object:modified', (e) => {
      this.handleObjectModified(e);
    });

    this.canvas.on('object:moving', (e) => {
      this.handleObjectMoving(e);
    });

    this.canvas.on('object:scaling', (e) => {
      this.handleObjectScaling(e);
    });

    this.canvas.on('object:rotating', (e) => {
      this.handleObjectRotating(e);
    });

    // Canvas events
    this.canvas.on('mouse:down', (e) => {
      this.handleMouseDown(e);
    });

    this.canvas.on('mouse:up', (e) => {
      this.handleMouseUp(e);
    });

    // Path events for drawing
    this.canvas.on('path:created', (e) => {
      this.handlePathCreated(e);
    });
  }

  /**
   * Update canvas size based on current device profile
   */
  updateCanvasSize() {
    if (!this.canvas) return;

    const deviceProfile = this.designStore.selectedDevice;
    if (deviceProfile) {
      this.canvas.setDimensions({
        width: deviceProfile.specifications.screenWidth,
        height: deviceProfile.specifications.screenHeight,
      });
    }
  }

  /**
   * Load watch style elements onto the canvas
   */
  loadWatchStyle(watchStyle: WatchStyle) {
    if (!this.canvas) return;

    this.clearCanvas();
    
    // Note: For now, skip loading dragBindBases since they need conversion
    // This will be implemented when we have proper mapping from WatchFace elements to CanvasElements
    console.log('Loading watch style:', watchStyle.styleName);
    
    this.canvas.renderAll();
  }

  /**
   * Add a new element to the canvas
   */
  addElementToCanvas(element: CanvasElement): fabric.Object | null {
    if (!this.canvas) return null;

    // Convert to simplified element for easier Fabric.js operations
    const simplified = toSimplifiedElement(element);
    let fabricObject: fabric.Object | null = null;

    switch (element.type) {
      case ElementType.IMAGE:
        fabricObject = this.createImageElement(simplified);
        break;
      case ElementType.TEXT:
        fabricObject = this.createTextElement(simplified);
        break;
      case ElementType.SHAPE:
        fabricObject = this.createShapeElement(simplified);
        break;
      case ElementType.PROGRESS_BAR:
        fabricObject = this.createProgressElement(simplified);
        break;
      case ElementType.TIME_DISPLAY:
        if (simplified.type === ElementType.TIME_DISPLAY) {
          // Check if it's analog or digital based on properties
          const isAnalog = element.properties.clockType === 'analog';
          fabricObject = isAnalog ? 
            this.createAnalogClockElement(simplified) : 
            this.createDigitalClockElement(simplified);
        }
        break;
      case ElementType.DATE_DISPLAY:
        fabricObject = this.createDigitalClockElement(simplified);
        break;
      default:
        console.warn(`Unsupported element type: ${element.type}`);
        return null;
    }

    if (fabricObject) {
      // Store element mapping using a custom property
      (fabricObject as any).elementId = element.id;
      
      this.canvas.add(fabricObject);
      this.elementMap.set(element.id, fabricObject);
      this.updateFabricObjectProperties(fabricObject, simplified);
    }

    return fabricObject;
  }

  /**
   * Create image element
   */
  private createImageElement(element: SimplifiedElement): fabric.Object {
    const rect = new fabric.Rect({
      width: element.width || 100,
      height: element.height || 100,
      fill: element.fill || '#ffffff',
      stroke: element.stroke,
      strokeWidth: element.strokeWidth || 0,
    });

    // If we have an image source, load it
    if (element.imageSrc) {
      fabric.Image.fromURL(element.imageSrc, (img) => {
        if (this.canvas && img) {
          img.set({
            left: element.x,
            top: element.y,
            width: element.width || img.width,
            height: element.height || img.height,
          });
          
          // Store element ID
          (img as any).elementId = element.id;
          
          this.canvas.remove(rect);
          this.canvas.add(img);
          this.elementMap.set(element.id, img);
        }
      });
    }

    return rect;
  }

  /**
   * Create text element
   */
  private createTextElement(element: SimplifiedElement): fabric.Object {
    return new fabric.Text(element.text || 'Text', {
      fontSize: element.fontSize || 16,
      fontFamily: element.fontFamily || 'Arial',
      fill: element.fill || '#ffffff',
      stroke: element.stroke,
      strokeWidth: element.strokeWidth || 0,
      textAlign: element.textAlign || 'left',
      fontWeight: element.fontWeight || 'normal',
      fontStyle: (element.fontStyle as any) || 'normal',
    });
  }

  /**
   * Create shape element
   */
  private createShapeElement(element: SimplifiedElement): fabric.Object {
    const shapeType = element.shapeType || 'rect';
    
    switch (shapeType) {
      case 'rect':
        return new fabric.Rect({
          width: element.width || 100,
          height: element.height || 100,
          fill: element.fill || 'transparent',
          stroke: element.stroke || '#ffffff',
          strokeWidth: element.strokeWidth || 1,
        });
      
      case 'circle':
        return new fabric.Circle({
          radius: element.radius || 50,
          fill: element.fill || 'transparent',
          stroke: element.stroke || '#ffffff',
          strokeWidth: element.strokeWidth || 1,
        });
      
      case 'ellipse':
        return new fabric.Ellipse({
          rx: element.rx || 50,
          ry: element.ry || 30,
          fill: element.fill || 'transparent',
          stroke: element.stroke || '#ffffff',
          strokeWidth: element.strokeWidth || 1,
        });
      
      case 'triangle':
        return new fabric.Triangle({
          width: element.width || 100,
          height: element.height || 100,
          fill: element.fill || 'transparent',
          stroke: element.stroke || '#ffffff',
          strokeWidth: element.strokeWidth || 1,
        });
      
      default:
        return new fabric.Rect({
          width: element.width || 100,
          height: element.height || 100,
          fill: element.fill || 'transparent',
          stroke: element.stroke || '#ffffff',
          strokeWidth: element.strokeWidth || 1,
        });
    }
  }

  /**
   * Create progress element (arc or line)
   */
  private createProgressElement(element: SimplifiedElement): fabric.Object {
    const progressType = element.progressType || 'arc';
    
    if (progressType === 'arc') {
      return new fabric.Circle({
        radius: element.radius || 50,
        fill: 'transparent',
        stroke: element.stroke || '#ffffff',
        strokeWidth: element.strokeWidth || 5,
        startAngle: element.startAngle || 0,
        endAngle: element.endAngle || Math.PI,
      });
    } else {
      return new fabric.Line([0, 0, element.width || 100, 0], {
        stroke: element.stroke || '#ffffff',
        strokeWidth: element.strokeWidth || 5,
      });
    }
  }

  /**
   * Create analog clock element
   */
  private createAnalogClockElement(element: SimplifiedElement): fabric.Object {
    const clockGroup = new fabric.Group([], {
      width: element.width || 200,
      height: element.height || 200,
    });

    // Create clock face
    const clockFace = new fabric.Circle({
      radius: (element.width || 200) / 2 - 10,
      fill: element.fill || 'transparent',
      stroke: element.stroke || '#ffffff',
      strokeWidth: element.strokeWidth || 2,
      originX: 'center',
      originY: 'center',
    });

    // Create hour markers
    for (let i = 0; i < 12; i++) {
      const angle = (i * 30) * Math.PI / 180;
      const innerRadius = (element.width || 200) / 2 - 20;
      const outerRadius = (element.width || 200) / 2 - 10;
      
      const line = new fabric.Line([
        Math.cos(angle) * innerRadius,
        Math.sin(angle) * innerRadius,
        Math.cos(angle) * outerRadius,
        Math.sin(angle) * outerRadius,
      ], {
        stroke: element.stroke || '#ffffff',
        strokeWidth: 2,
        originX: 'center',
        originY: 'center',
      });
      
      clockGroup.addWithUpdate(line);
    }

    clockGroup.addWithUpdate(clockFace);
    return clockGroup;
  }

  /**
   * Create digital clock element
   */
  private createDigitalClockElement(element: SimplifiedElement): fabric.Object {
    return new fabric.Text('12:34', {
      fontSize: element.fontSize || 24,
      fontFamily: element.fontFamily || 'Arial',
      fill: element.fill || '#ffffff',
      stroke: element.stroke,
      strokeWidth: element.strokeWidth || 0,
      textAlign: 'center',
    });
  }

  /**
   * Update fabric object properties from simplified element data
   */
  private updateFabricObjectProperties(fabricObject: fabric.Object, element: SimplifiedElement) {
    fabricObject.set({
      left: element.x,
      top: element.y,
      angle: element.rotation || 0,
      scaleX: element.scaleX || 1,
      scaleY: element.scaleY || 1,
      opacity: element.opacity !== undefined ? element.opacity : 1,
      visible: element.visible !== false,
      selectable: !element.locked,
      evented: !element.locked,
    });
  }

  /**
   * Handle selection changes
   */
  private handleSelectionChanged(e: fabric.IEvent) {
    const selection = e.selected || [];
    const elementIds = selection
      .map(obj => (obj as any).elementId)
      .filter(id => id) as string[];
    
    this.canvasStore.setSelectedElements(elementIds);
  }

  /**
   * Handle object modification
   */
  private handleObjectModified(e: fabric.IEvent) {
    if (this.isUpdating || !e.target) return;
    
    const elementId = (e.target as any).elementId as string;
    if (elementId) {
      this.syncObjectToStore(e.target, elementId);
    }
  }

  /**
   * Handle object moving
   */
  private handleObjectMoving(e: fabric.IEvent) {
    if (!e.target) return;
    
    const elementId = (e.target as any).elementId as string;
    if (elementId) {
      this.canvasStore.updateElementPosition(elementId, e.target.left!, e.target.top!);
    }
  }

  /**
   * Handle object scaling
   */
  private handleObjectScaling(e: fabric.IEvent) {
    if (!e.target) return;
    
    const elementId = (e.target as any).elementId as string;
    if (elementId) {
      this.canvasStore.updateElementScale(elementId, e.target.scaleX!, e.target.scaleY!);
    }
  }

  /**
   * Handle object rotating
   */
  private handleObjectRotating(e: fabric.IEvent) {
    if (!e.target) return;
    
    const elementId = (e.target as any).elementId as string;
    if (elementId) {
      this.canvasStore.updateElementRotation(elementId, e.target.angle!);
    }
  }

  /**
   * Handle mouse down events
   */
  private handleMouseDown(_e: fabric.IEvent) {
    // Handle tool-specific actions
    const activeTool = this.canvasStore.activeTool;
    
    if (activeTool === 'select') {
      // Default selection behavior
    } else if (activeTool === 'pan') {
      this.canvas?.setCursor('grabbing');
    }
  }

  /**
   * Handle mouse up events
   */
  private handleMouseUp(_e: fabric.IEvent) {
    const activeTool = this.canvasStore.activeTool;
    
    if (activeTool === 'pan') {
      this.canvas?.setCursor('grab');
    }
  }

  /**
   * Handle path creation (for drawing tools)
   */
  private handlePathCreated(_e: fabric.IEvent) {
    // For now, skip path creation until we have proper element creation flow
    // This would need proper conversion from fabric path to CanvasElement
    console.log('Path created event - not implemented yet');
  }

  /**
   * Sync fabric object properties back to store
   */
  private syncObjectToStore(fabricObject: fabric.Object, elementId: string) {
    const element = this.canvasStore.getElementById(elementId);
    if (!element) return;

    // Create updates using the proper CanvasElement structure
    const updatedTransform = {
      ...element.transform,
      x: fabricObject.left!,
      y: fabricObject.top!,
      rotation: fabricObject.angle!,
      scaleX: fabricObject.scaleX!,
      scaleY: fabricObject.scaleY!,
    };

    const elementUpdates: Partial<CanvasElement> = {
      transform: updatedTransform,
    };

    // Update type-specific properties in the properties object
    if (fabricObject instanceof fabric.Text) {
      elementUpdates.properties = {
        ...element.properties,
        text: fabricObject.text,
        fontSize: fabricObject.fontSize,
        fontFamily: fabricObject.fontFamily,
      };
    } else if (fabricObject instanceof fabric.Rect) {
      elementUpdates.transform = {
        ...updatedTransform,
        width: fabricObject.width! * fabricObject.scaleX!,
        height: fabricObject.height! * fabricObject.scaleY!,
      };
    }

    this.canvasStore.updateElement(elementId, elementUpdates);
  }

  /**
   * Clear all objects from canvas
   */
  clearCanvas() {
    if (!this.canvas) return;
    
    this.canvas.clear();
    this.elementMap.clear();
  }

  /**
   * Remove element from canvas
   */
  removeElement(elementId: string) {
    const fabricObject = this.elementMap.get(elementId);
    if (fabricObject && this.canvas) {
      this.canvas.remove(fabricObject);
      this.elementMap.delete(elementId);
    }
  }

  /**
   * Update element on canvas
   */
  updateElement(elementId: string, elementData: Partial<CanvasElement>) {
    const fabricObject = this.elementMap.get(elementId);
    if (!fabricObject) return;

    this.isUpdating = true;
    
    // Update fabric object properties from transform
    if (elementData.transform) {
      if (elementData.transform.x !== undefined) fabricObject.set('left', elementData.transform.x);
      if (elementData.transform.y !== undefined) fabricObject.set('top', elementData.transform.y);
      if (elementData.transform.rotation !== undefined) fabricObject.set('angle', elementData.transform.rotation);
      if (elementData.transform.scaleX !== undefined) fabricObject.set('scaleX', elementData.transform.scaleX);
      if (elementData.transform.scaleY !== undefined) fabricObject.set('scaleY', elementData.transform.scaleY);
    }
    
    // Update fabric object properties from style
    if (elementData.style) {
      if (elementData.style.opacity !== undefined) fabricObject.set('opacity', elementData.style.opacity);
      if (elementData.style.visible !== undefined) fabricObject.set('visible', elementData.style.visible);
    }
    
    // Update lock state
    if (elementData.isLocked !== undefined) {
      fabricObject.set('selectable', !elementData.isLocked);
      fabricObject.set('evented', !elementData.isLocked);
    }

    // Update type-specific properties from properties object
    if (fabricObject instanceof fabric.Text && elementData.properties?.text !== undefined) {
      fabricObject.set('text', elementData.properties.text);
    }

    this.canvas?.renderAll();
    this.isUpdating = false;
  }

  /**
   * Set canvas zoom level
   */
  setZoom(zoom: number) {
    if (!this.canvas) return;
    
    this.canvas.setZoom(zoom);
    this.canvas.renderAll();
  }

  /**
   * Pan canvas to position
   */
  panTo(x: number, y: number) {
    if (!this.canvas) return;
    
    this.canvas.absolutePan({ x, y });
  }

  /**
   * Enable/disable drawing mode
   */
  setDrawingMode(enabled: boolean) {
    if (!this.canvas) return;
    
    this.canvas.isDrawingMode = enabled;
    if (enabled) {
      this.canvas.freeDrawingBrush.width = 2;
      this.canvas.freeDrawingBrush.color = '#ffffff';
    }
  }

  /**
   * Export canvas as image
   */
  exportAsImage(format: string = 'png', quality: number = 1): string | null {
    if (!this.canvas) return null;
    
    return this.canvas.toDataURL({
      format,
      quality,
      multiplier: 1,
    });
  }

  /**
   * Get canvas instance
   */
  getCanvas(): fabric.Canvas | null {
    return this.canvas;
  }

  /**
   * Dispose of canvas and cleanup
   */
  dispose() {
    if (this.canvas) {
      this.canvas.dispose();
      this.canvas = null;
    }
    this.elementMap.clear();
  }
}

// Create singleton instance
export let canvasService: CanvasService;

export const initializeCanvasService = (canvasStore: CanvasStore, designStore: DesignStore) => {
  canvasService = new CanvasService(canvasStore, designStore);
  return canvasService;
};