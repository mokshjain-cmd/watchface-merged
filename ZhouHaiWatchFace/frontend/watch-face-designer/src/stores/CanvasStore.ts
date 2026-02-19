// Canvas store - equivalent to DetailViewModel in C#
import { makeAutoObservable, runInAction } from 'mobx';
import type { 
  CanvasState,
  CanvasElement,
  SelectionState,
  HistoryEntry,
  GridSettings,
  Guide,
  ClipboardData
} from '@/models';
import { 
  CanvasMode,
  ElementType,
  BlendMode
} from '@/models';
import { generateId } from '@/models/ModelHelpers';

export type CanvasTool = 
  | 'select'
  | 'pan'
  | 'zoom'
  | 'text'
  | 'shape'
  | 'image'
  | 'line';

export class CanvasStore {
  // Canvas state
  canvasState: CanvasState;
  
  // Selection
  selectionState: SelectionState = {
    selectedElements: [],
    multiSelect: false,
  };
  
  // History for undo/redo
  history: HistoryEntry[] = [];
  historyIndex = -1;
  maxHistorySize = 50;
  
  // Clipboard
  clipboard: ClipboardData | null = null;
  
  // Grid and guides
  gridSettings: GridSettings = {
    visible: true,
    enabled: true,
    size: 10,
    color: '#E0E0E0',
    opacity: 0.5,
    type: 'dots',
  };
  guides: Guide[] = [];
  
  // Tool state
  currentTool: CanvasTool = 'select';
  isDrawing = false;
  isPanning = false;
  isResizing = false;
  
  // Fabric.js canvas reference
  fabricCanvas: any = null; // Will be set when canvas initializes

  constructor(width = 390, height = 450) {
    this.canvasState = this.createInitialCanvasState(width, height);
    makeAutoObservable(this, {}, { autoBind: true });
  }

  // Computed properties
  get canvasSize() {
    return {
      width: this.canvasState.width,
      height: this.canvasState.height
    };
  }

  get zoomLevel() {
    return this.canvasState.zoom;
  }

  get showGrid() {
    return this.canvasState.gridVisible;
  }

  // Canvas initialization
  initializeCanvas(fabricCanvas: any) {
    runInAction(() => {
      this.fabricCanvas = fabricCanvas;
    });
    
    this.setupCanvasEvents();
  }

  createInitialCanvasState(width: number, height: number): CanvasState {
    return {
      id: generateId(),
      name: 'Main Canvas',
      width,
      height,
      zoom: 1,
      panX: 0,
      panY: 0,
      backgroundColor: '#000000',
      gridVisible: true,
      gridSize: 10,
      snapToGrid: true,
      selectedElementIds: [],
      elements: [],
      layers: [
        {
          id: generateId(),
          name: 'Background',
          isVisible: true,
          isLocked: false,
          opacity: 1,
          blendMode: BlendMode.NORMAL,
          elementIds: [],
          order: 0,
        },
        {
          id: generateId(),
          name: 'Main',
          isVisible: true,
          isLocked: false,
          opacity: 1,
          blendMode: BlendMode.NORMAL,
          elementIds: [],
          order: 1,
        },
      ],
      mode: CanvasMode.DESIGN,
    };
  }

  // Canvas operations
  setCanvasSize(width: number, height: number) {
    this.addToHistory({
      type: 'update',
      elementIds: [],
      description: 'Resize canvas',
      before: { width: this.canvasState.width, height: this.canvasState.height },
      after: { width, height },
    });

    runInAction(() => {
      this.canvasState.width = width;
      this.canvasState.height = height;
    });

    if (this.fabricCanvas) {
      this.fabricCanvas.setDimensions({ width, height });
    }
  }

  setZoom(zoom: number) {
    const clampedZoom = Math.max(0.1, Math.min(5, zoom));
    
    runInAction(() => {
      this.canvasState.zoom = clampedZoom;
    });

    if (this.fabricCanvas) {
      this.fabricCanvas.setZoom(clampedZoom);
    }
  }

  setPan(x: number, y: number) {
    runInAction(() => {
      this.canvasState.panX = x;
      this.canvasState.panY = y;
    });

    if (this.fabricCanvas) {
      this.fabricCanvas.viewportTransform[4] = x;
      this.fabricCanvas.viewportTransform[5] = y;
      this.fabricCanvas.requestRenderAll();
    }
  }

  setMode(mode: CanvasMode) {
    runInAction(() => {
      this.canvasState.mode = mode;
      if (mode !== CanvasMode.DESIGN) {
        this.clearSelection();
      }
    });
  }

  // Element management
  addElement(elementType: ElementType, properties: any = {}) {
    const newElement: CanvasElement = {
      id: generateId(),
      type: elementType,
      name: `${elementType}_${Date.now()}`,
      layerId: this.canvasState.layers[1].id, // Add to main layer by default
      properties: {
        ...this.getDefaultProperties(elementType),
        ...properties,
      },
      transform: {
        x: 50,
        y: 50,
        width: 100,
        height: 100,
        rotation: 0,
        scaleX: 1,
        scaleY: 1,
        skewX: 0,
        skewY: 0,
        originX: 'left',
        originY: 'top',
      },
      style: {
        opacity: 1,
        visible: true,
      },
      isLocked: false,
      isVisible: true,
      isSelected: false,
    };

    this.addToHistory({
      type: 'create',
      elementIds: [newElement.id],
      description: `Add ${elementType}`,
      before: null,
      after: newElement,
    });

    runInAction(() => {
      this.canvasState.elements.push(newElement);
      const layer = this.canvasState.layers.find(l => l.id === newElement.layerId);
      if (layer) {
        layer.elementIds.push(newElement.id);
      }
    });

    return newElement;
  }

  updateElement(elementId: string, updates: Partial<CanvasElement>) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    this.addToHistory({
      type: 'update',
      elementIds: [elementId],
      description: 'Update element',
      before: { ...element },
      after: { ...element, ...updates },
    });

    runInAction(() => {
      Object.assign(element, updates);
    });
  }

  deleteElement(elementId: string) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    this.addToHistory({
      type: 'delete',
      elementIds: [elementId],
      description: 'Delete element',
      before: element,
      after: null,
    });

    runInAction(() => {
      this.canvasState.elements = this.canvasState.elements.filter(e => e.id !== elementId);
      this.canvasState.selectedElementIds = this.canvasState.selectedElementIds.filter(id => id !== elementId);
      
      // Remove from layer
      this.canvasState.layers.forEach(layer => {
        layer.elementIds = layer.elementIds.filter(id => id !== elementId);
      });
    });
  }

  duplicateElement(elementId: string) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    const duplicatedElement: CanvasElement = {
      ...element,
      id: generateId(),
      name: `${element.name}_copy`,
      transform: {
        ...element.transform,
        x: element.transform.x + 20,
        y: element.transform.y + 20,
      },
      isSelected: false,
    };

    this.addToHistory({
      type: 'create',
      elementIds: [duplicatedElement.id],
      description: 'Duplicate element',
      before: null,
      after: duplicatedElement,
    });

    runInAction(() => {
      this.canvasState.elements.push(duplicatedElement);
      const layer = this.canvasState.layers.find(l => l.id === duplicatedElement.layerId);
      if (layer) {
        layer.elementIds.push(duplicatedElement.id);
      }
    });

    return duplicatedElement;
  }

  // Selection management
  selectElement(elementId: string, multiSelect = false) {
    runInAction(() => {
      if (!multiSelect) {
        this.canvasState.selectedElementIds = [elementId];
        this.selectionState.selectedElements = [elementId];
        this.selectionState.multiSelect = false;
      } else {
        if (!this.canvasState.selectedElementIds.includes(elementId)) {
          this.canvasState.selectedElementIds.push(elementId);
          this.selectionState.selectedElements.push(elementId);
        }
        this.selectionState.multiSelect = true;
      }
      
      // Update element selection state
      this.canvasState.elements.forEach(element => {
        element.isSelected = this.canvasState.selectedElementIds.includes(element.id);
      });
    });
  }

  deselectElement(elementId: string) {
    runInAction(() => {
      this.canvasState.selectedElementIds = this.canvasState.selectedElementIds.filter(id => id !== elementId);
      this.selectionState.selectedElements = this.selectionState.selectedElements.filter(id => id !== elementId);
      
      const element = this.canvasState.elements.find(e => e.id === elementId);
      if (element) {
        element.isSelected = false;
      }
    });
  }

  clearSelection() {
    runInAction(() => {
      this.canvasState.selectedElementIds = [];
      this.selectionState.selectedElements = [];
      this.selectionState.multiSelect = false;
      
      this.canvasState.elements.forEach(element => {
        element.isSelected = false;
      });
    });
  }

  selectAll() {
    const visibleElements = this.canvasState.elements.filter(e => e.isVisible);
    runInAction(() => {
      this.canvasState.selectedElementIds = visibleElements.map(e => e.id);
      this.selectionState.selectedElements = visibleElements.map(e => e.id);
      this.selectionState.multiSelect = true;
      
      this.canvasState.elements.forEach(element => {
        element.isSelected = visibleElements.some(e => e.id === element.id);
      });
    });
  }

  // Clipboard operations
  copySelection() {
    const selectedElements = this.canvasState.elements.filter(e => 
      this.canvasState.selectedElementIds.includes(e.id)
    );

    if (selectedElements.length > 0) {
      runInAction(() => {
        this.clipboard = {
          type: 'elements',
          elements: selectedElements.map(element => ({ ...element })),
          timestamp: new Date(),
        };
      });
    }
  }

  paste() {
    if (!this.clipboard || this.clipboard.type !== 'elements') return;

    const pastedElements = this.clipboard.elements.map(element => ({
      ...element,
      id: generateId(),
      name: `${element.name}_pasted`,
      transform: {
        ...element.transform,
        x: element.transform.x + 20,
        y: element.transform.y + 20,
      },
      isSelected: true,
    }));

    this.addToHistory({
      type: 'create',
      elementIds: pastedElements.map(e => e.id),
      description: 'Paste elements',
      before: null,
      after: pastedElements,
    });

    runInAction(() => {
      this.canvasState.elements.push(...pastedElements);
      this.canvasState.selectedElementIds = pastedElements.map(e => e.id);
      this.selectionState.selectedElements = pastedElements.map(e => e.id);
      
      // Add to current layer
      const mainLayer = this.canvasState.layers.find(l => l.name === 'Main');
      if (mainLayer) {
        mainLayer.elementIds.push(...pastedElements.map(e => e.id));
      }
    });
  }

  // Tool management
  setTool(tool: CanvasTool) {
    runInAction(() => {
      this.currentTool = tool;
    });
  }

  // History management (undo/redo)
  addToHistory(actionData: {
    type: 'create' | 'update' | 'delete' | 'move' | 'resize' | 'rotate' | 'style' | 'group' | 'ungroup';
    elementIds: string[];
    description: string;
    before?: any;
    after?: any;
    data?: any;
  }) {
    const entry: HistoryEntry = {
      id: generateId(),
      timestamp: new Date(),
      action: {
        type: actionData.type,
        elementIds: actionData.elementIds,
        before: actionData.before,
        after: actionData.after,
      },
      description: actionData.description,
      data: actionData.data,
    };

    runInAction(() => {
      // Remove entries after current index (for when we're not at the end)
      this.history = this.history.slice(0, this.historyIndex + 1);
      
      // Add new entry
      this.history.push(entry);
      this.historyIndex = this.history.length - 1;
      
      // Limit history size
      if (this.history.length > this.maxHistorySize) {
        this.history = this.history.slice(-this.maxHistorySize);
        this.historyIndex = this.history.length - 1;
      }
    });
  }

  undo() {
    if (!this.canUndo) return;

    const entry = this.history[this.historyIndex];
    this.executeHistoryAction(entry, true);
    
    runInAction(() => {
      this.historyIndex--;
    });
  }

  redo() {
    if (!this.canRedo) return;

    runInAction(() => {
      this.historyIndex++;
    });
    
    const entry = this.history[this.historyIndex];
    this.executeHistoryAction(entry, false);
  }

  // Computed properties
  get selectedElements() {
    return this.canvasState.elements.filter(e => 
      this.canvasState.selectedElementIds.includes(e.id)
    );
  }

  get canUndo() {
    return this.historyIndex >= 0;
  }

  get canRedo() {
    return this.historyIndex < this.history.length - 1;
  }

  get hasSelection() {
    return this.canvasState.selectedElementIds.length > 0;
  }

  // Private methods
  private getDefaultProperties(elementType: ElementType) {
    const defaults: Record<ElementType, any> = {
      [ElementType.IMAGE]: { source: '', opacity: 1 },
      [ElementType.TEXT]: { text: 'Text', fontSize: 16, color: '#FFFFFF' },
      [ElementType.SHAPE]: { fill: '#FFFFFF', stroke: '#000000' },
      [ElementType.WIDGET]: { widgetType: 'generic' },
      [ElementType.GROUP]: {},
      [ElementType.BACKGROUND]: { source: '', opacity: 1 },
      [ElementType.TIME_DISPLAY]: { format: 'HH:mm', fontSize: 24 },
      [ElementType.DATE_DISPLAY]: { format: 'MMM dd', fontSize: 16 },
      [ElementType.BATTERY_INDICATOR]: { showPercentage: true },
      [ElementType.STEP_COUNTER]: { showGoal: false },
      [ElementType.WEATHER_WIDGET]: { showIcon: true, showTemp: true },
      [ElementType.PROGRESS_BAR]: { min: 0, max: 100, value: 50 },
      [ElementType.ANIMATION]: { frames: [], duration: 1000 },
    };

    return defaults[elementType] || {};
  }

  private setupCanvasEvents() {
    if (!this.fabricCanvas) return;

    // Setup fabric.js event listeners for selection, moving, etc.
    this.fabricCanvas.on('selection:created', (e: any) => {
      const selectedObjects = e.selected || [e.target];
      const elementIds = selectedObjects.map((obj: any) => obj.elementId).filter(Boolean);
      if (elementIds.length > 0) {
        this.selectElement(elementIds[0], elementIds.length > 1);
      }
    });

    this.fabricCanvas.on('selection:cleared', () => {
      this.clearSelection();
    });
  }

  private executeHistoryAction(entry: HistoryEntry, isUndo: boolean) {
    const { action } = entry;
    
    switch (action.type) {
      case 'create':
        if (isUndo) {
          // Remove elements
          action.elementIds.forEach(id => {
            const index = this.canvasState.elements.findIndex(e => e.id === id);
            if (index >= 0) {
              this.canvasState.elements.splice(index, 1);
            }
          });
        } else {
          // Add elements
          if (action.after) {
            const elements = Array.isArray(action.after) ? action.after : [action.after];
            this.canvasState.elements.push(...elements);
          }
        }
        break;

      case 'delete':
        if (isUndo) {
          // Restore elements
          if (action.before) {
            const elements = Array.isArray(action.before) ? action.before : [action.before];
            this.canvasState.elements.push(...elements);
          }
        } else {
          // Remove elements
          action.elementIds.forEach(id => {
            const index = this.canvasState.elements.findIndex(e => e.id === id);
            if (index >= 0) {
              this.canvasState.elements.splice(index, 1);
            }
          });
        }
        break;

      case 'update':
        const data = isUndo ? action.before : action.after;
        if (data) {
          action.elementIds.forEach(id => {
            const element = this.canvasState.elements.find(e => e.id === id);
            if (element) {
              Object.assign(element, data);
            }
          });
        }
        break;
    }
  }

  // Active tool
  activeTool: CanvasTool = 'select';

  // Tool management
  setActiveTool(tool: CanvasTool) {
    runInAction(() => {
      this.activeTool = tool;
    });
  }

  // Element selection methods for CanvasService integration
  setSelectedElements(elementIds: string[]) {
    runInAction(() => {
      this.canvasState.selectedElementIds = elementIds;
      this.selectionState.selectedElements = elementIds;
      this.selectionState.multiSelect = elementIds.length > 1;
      
      this.canvasState.elements.forEach(element => {
        element.isSelected = elementIds.includes(element.id);
      });
    });
  }

  // Element property update methods for CanvasService integration
  updateElementPosition(elementId: string, x: number, y: number) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    this.updateElement(elementId, {
      transform: {
        ...element.transform,
        x,
        y,
      },
    });
  }

  updateElementScale(elementId: string, scaleX: number, scaleY: number) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    this.updateElement(elementId, {
      transform: {
        ...element.transform,
        scaleX,
        scaleY,
      },
    });
  }

  updateElementRotation(elementId: string, rotation: number) {
    const element = this.canvasState.elements.find(e => e.id === elementId);
    if (!element) return;

    this.updateElement(elementId, {
      transform: {
        ...element.transform,
        rotation,
      },
    });
  }

  // Tool management
  setCurrentTool(tool: CanvasTool) {
    runInAction(() => {
      this.currentTool = tool;
    });
  }

  // Zoom operations
  zoomIn() {
    const newZoom = Math.min(5, this.canvasState.zoom * 1.2);
    this.setZoom(newZoom);
  }

  zoomOut() {
    const newZoom = Math.max(0.1, this.canvasState.zoom / 1.2);
    this.setZoom(newZoom);
  }

  resetZoom() {
    this.setZoom(1);
  }

  // Clipboard operations
  copySelectedElements() {
    const selectedElements = this.selectedElements;
    if (selectedElements.length === 0) return;

    const elementData = selectedElements.map(element => ({
      ...element,
      id: generateId(), // Generate new IDs for pasted elements
    }));

    runInAction(() => {
      this.clipboard = {
        type: 'elements',
        elements: elementData,
        timestamp: new Date(),
      };
    });
  }

  cutSelectedElements() {
    this.copySelectedElements();
    this.deleteSelectedElements();
  }

  pasteElements() {
    if (!this.clipboard || this.clipboard.elements.length === 0) return;

    const pastedElements = this.clipboard.elements.map(element => ({
      ...element,
      id: generateId(),
      transform: {
        ...element.transform,
        x: element.transform.x + 10, // Offset pasted elements slightly
        y: element.transform.y + 10,
      },
    }));

    this.addToHistory({
      type: 'create',
      elementIds: pastedElements.map(e => e.id),
      description: `Paste ${pastedElements.length} element(s)`,
      before: {},
      after: { elements: pastedElements },
    });

    runInAction(() => {
      this.canvasState.elements.push(...pastedElements);
      this.setSelectedElements(pastedElements.map(e => e.id));
    });
  }

  // Selection operations
  selectAllElements() {
    const allElementIds = this.canvasState.elements.map(e => e.id);
    this.setSelectedElements(allElementIds);
  }

  deleteSelectedElements() {
    const selectedIds = this.canvasState.selectedElementIds;
    if (selectedIds.length === 0) return;

    const elementsToDelete = this.canvasState.elements.filter(e => 
      selectedIds.includes(e.id)
    );

    this.addToHistory({
      type: 'delete',
      elementIds: selectedIds,
      description: `Delete ${selectedIds.length} element(s)`,
      before: { elements: elementsToDelete },
      after: {},
    });

    runInAction(() => {
      this.canvasState.elements = this.canvasState.elements.filter(e => 
        !selectedIds.includes(e.id)
      );
      this.canvasState.selectedElementIds = [];
    });
  }

  // Get element by ID
  getElementById(elementId: string): CanvasElement | undefined {
    return this.canvasState.elements.find(e => e.id === elementId);
  }

  // Grid controls
  toggleGrid() {
    runInAction(() => {
      this.canvasState.gridVisible = !this.canvasState.gridVisible;
    });
  }
}

// Create singleton instance
export const canvasStore = new CanvasStore();