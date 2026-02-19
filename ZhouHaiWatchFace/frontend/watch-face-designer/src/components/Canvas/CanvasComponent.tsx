import React, { useEffect, useRef } from 'react';
import { observer } from 'mobx-react-lite';
import { canvasService, initializeCanvasService } from '@/services/CanvasService';
import { canvasStore } from '@/stores/CanvasStore';
import { designStore } from '@/stores/DesignStore';
import './CanvasComponent.scss';

interface CanvasComponentProps {
  className?: string;
  width?: number;
  height?: number;
}

export const CanvasComponent: React.FC<CanvasComponentProps> = observer(({
  className = '',
  width = 400,
  height = 400
}) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const canvasServiceRef = useRef<typeof canvasService | null>(null);

  useEffect(() => {
    if (!canvasRef.current) return;

    // Initialize canvas service if not already done
    if (!canvasServiceRef.current) {
      canvasServiceRef.current = initializeCanvasService(canvasStore, designStore);
    }

    // Initialize Fabric.js canvas
    const fabricCanvas = canvasServiceRef.current.initializeCanvas(canvasRef.current);

    // Set initial canvas size
    fabricCanvas.setDimensions({ width, height });

    // Update canvas size when device changes
    canvasServiceRef.current.updateCanvasSize();

    // Cleanup on unmount
    return () => {
      canvasServiceRef.current?.dispose();
      canvasServiceRef.current = null;
    };
  }, [width, height]);

  // Update canvas size when dimensions change
  useEffect(() => {
    if (canvasServiceRef.current) {
      const canvas = canvasServiceRef.current.getCanvas();
      if (canvas) {
        canvas.setDimensions({ width, height });
        canvas.renderAll();
      }
    }
  }, [width, height]);

  // Update canvas when device profile changes
  useEffect(() => {
    if (canvasServiceRef.current && designStore.selectedDevice) {
      canvasServiceRef.current.updateCanvasSize();
    }
  }, [designStore.selectedDevice]);

  // Update zoom level
  useEffect(() => {
    if (canvasServiceRef.current) {
      canvasServiceRef.current.setZoom(canvasStore.canvasState.zoom);
    }
  }, [canvasStore.canvasState.zoom]);

  // Update active tool
  useEffect(() => {
    if (canvasServiceRef.current) {
      const canvas = canvasServiceRef.current.getCanvas();
      if (canvas) {
        // Update drawing mode based on active tool
        canvas.isDrawingMode = canvasStore.activeTool === 'line';
        
        // Update cursor based on tool
        switch (canvasStore.activeTool) {
          case 'pan':
            canvas.defaultCursor = 'grab';
            canvas.hoverCursor = 'grab';
            break;
          case 'zoom':
            canvas.defaultCursor = 'zoom-in';
            canvas.hoverCursor = 'zoom-in';
            break;
          default:
            canvas.defaultCursor = 'default';
            canvas.hoverCursor = 'move';
            break;
        }
      }
    }
  }, [canvasStore.activeTool]);

  // Load current watch style elements
  useEffect(() => {
    if (canvasServiceRef.current && designStore.currentStyle) {
      canvasServiceRef.current.loadWatchStyle(designStore.currentStyle);
    }
  }, [designStore.currentStyle]);

  const handleCanvasClick = (event: React.MouseEvent<HTMLCanvasElement>) => {
    // Handle canvas click events based on active tool
    const rect = canvasRef.current?.getBoundingClientRect();
    if (!rect) return;

    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    console.log(`Canvas clicked at (${x}, ${y}) with tool: ${canvasStore.activeTool}`);
  };

  const handleContextMenu = (event: React.MouseEvent<HTMLCanvasElement>) => {
    event.preventDefault();
    // TODO: Show context menu
    console.log('Context menu requested');
  };

  const containerClasses = `canvas-container ${className}`.trim();

  return (
    <div className={containerClasses}>
      <div className="canvas-wrapper">
        <canvas
          ref={canvasRef}
          width={width}
          height={height}
          onClick={handleCanvasClick}
          onContextMenu={handleContextMenu}
          className="fabric-canvas"
        />
        
        {/* Canvas overlay for grid, guides, etc. */}
        <div className="canvas-overlay">
          {canvasStore.canvasState.gridVisible && (
            <div 
              className="canvas-grid"
              style={{
                backgroundSize: `${canvasStore.canvasState.gridSize}px ${canvasStore.canvasState.gridSize}px`,
                width,
                height,
              }}
            />
          )}
          
          {/* Selection indicator */}
          {canvasStore.selectedElements.length > 0 && (
            <div className="selection-indicator">
              {canvasStore.selectedElements.length} element(s) selected
            </div>
          )}
        </div>
      </div>
      
      {/* Canvas controls */}
      <div className="canvas-controls">
        <button
          className={`control-btn ${canvasStore.canvasState.gridVisible ? 'active' : ''}`}
          onClick={() => canvasStore.toggleGrid()}
          title="Toggle Grid"
        >
          ⊞
        </button>
        
        <button
          className="control-btn"
          onClick={() => canvasStore.setZoom(canvasStore.canvasState.zoom + 0.1)}
          title="Zoom In"
        >
          +
        </button>
        
        <span className="zoom-level">
          {Math.round(canvasStore.canvasState.zoom * 100)}%
        </span>
        
        <button
          className="control-btn"
          onClick={() => canvasStore.setZoom(Math.max(0.1, canvasStore.canvasState.zoom - 0.1))}
          title="Zoom Out"
        >
          -
        </button>
        
        <button
          className="control-btn"
          onClick={() => canvasStore.setZoom(1)}
          title="Reset Zoom"
        >
          🎯
        </button>
      </div>
    </div>
  );
});

CanvasComponent.displayName = 'CanvasComponent';