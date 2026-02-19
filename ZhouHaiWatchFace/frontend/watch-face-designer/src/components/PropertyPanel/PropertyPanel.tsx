import React, { useEffect, useState } from 'react';
import { observer } from 'mobx-react-lite';
import { canvasStore } from '@/stores/CanvasStore';
import type { CanvasElement } from '@/models/Canvas';
import './PropertyPanel.scss';

interface PropertyPanelProps {
  className?: string;
}

export const PropertyPanel: React.FC<PropertyPanelProps> = observer(({ className = '' }) => {
  const [localProperties, setLocalProperties] = useState<Record<string, any>>({});
  
  const selectedElements = canvasStore.selectedElements;
  const selectedElement = selectedElements.length === 1 ? selectedElements[0] : null;
  const hasMultipleSelection = selectedElements.length > 1;

  // Update local properties when selection changes
  useEffect(() => {
    if (selectedElement) {
      setLocalProperties({
        ...selectedElement.transform,
        ...selectedElement.style,
        ...selectedElement.properties,
        isLocked: selectedElement.isLocked,
        isVisible: selectedElement.isVisible,
      });
    } else {
      setLocalProperties({});
    }
  }, [selectedElement]);

  const updateProperty = (key: string, value: any) => {
    setLocalProperties(prev => ({ ...prev, [key]: value }));
    
    if (selectedElement) {
      // Determine which part of the element to update
      let updates: Partial<CanvasElement> = {};
      
      // Transform properties
      if (['x', 'y', 'width', 'height', 'rotation', 'scaleX', 'scaleY'].includes(key)) {
        updates.transform = { ...selectedElement.transform, [key]: value };
      }
      // Style properties
      else if (['opacity', 'visible', 'fill', 'stroke', 'strokeWidth'].includes(key)) {
        updates.style = { ...selectedElement.style, [key]: value };
      }
      // Element properties
      else if (['isLocked', 'isVisible'].includes(key)) {
        updates[key as keyof CanvasElement] = value;
      }
      // Custom properties
      else {
        updates.properties = { ...selectedElement.properties, [key]: value };
      }
      
      canvasStore.updateElement(selectedElement.id, updates);
    }
  };

  const renderTransformProperties = () => (
    <div className="property-section">
      <h4 className="section-title">Transform</h4>
      <div className="property-grid">
        <div className="property-row">
          <label>X:</label>
          <input
            type="number"
            value={localProperties.x || 0}
            onChange={(e) => updateProperty('x', parseFloat(e.target.value) || 0)}
            step="1"
          />
        </div>
        <div className="property-row">
          <label>Y:</label>
          <input
            type="number"
            value={localProperties.y || 0}
            onChange={(e) => updateProperty('y', parseFloat(e.target.value) || 0)}
            step="1"
          />
        </div>
        <div className="property-row">
          <label>Width:</label>
          <input
            type="number"
            value={localProperties.width || 0}
            onChange={(e) => updateProperty('width', parseFloat(e.target.value) || 1)}
            step="1"
            min="1"
          />
        </div>
        <div className="property-row">
          <label>Height:</label>
          <input
            type="number"
            value={localProperties.height || 0}
            onChange={(e) => updateProperty('height', parseFloat(e.target.value) || 1)}
            step="1"
            min="1"
          />
        </div>
        <div className="property-row">
          <label>Rotation:</label>
          <input
            type="number"
            value={localProperties.rotation || 0}
            onChange={(e) => updateProperty('rotation', parseFloat(e.target.value) || 0)}
            step="1"
            min="-360"
            max="360"
          />
        </div>
        <div className="property-row">
          <label>Scale X:</label>
          <input
            type="number"
            value={localProperties.scaleX || 1}
            onChange={(e) => updateProperty('scaleX', parseFloat(e.target.value) || 1)}
            step="0.1"
            min="0.1"
            max="10"
          />
        </div>
        <div className="property-row">
          <label>Scale Y:</label>
          <input
            type="number"
            value={localProperties.scaleY || 1}
            onChange={(e) => updateProperty('scaleY', parseFloat(e.target.value) || 1)}
            step="0.1"
            min="0.1"
            max="10"
          />
        </div>
      </div>
    </div>
  );

  const renderStyleProperties = () => (
    <div className="property-section">
      <h4 className="section-title">Style</h4>
      <div className="property-grid">
        <div className="property-row">
          <label>Opacity:</label>
          <input
            type="range"
            min="0"
            max="1"
            step="0.01"
            value={localProperties.opacity || 1}
            onChange={(e) => updateProperty('opacity', parseFloat(e.target.value))}
          />
          <span className="value-display">{Math.round((localProperties.opacity || 1) * 100)}%</span>
        </div>
        <div className="property-row">
          <label>Fill:</label>
          <input
            type="color"
            value={typeof localProperties.fill === 'string' ? localProperties.fill : '#ffffff'}
            onChange={(e) => updateProperty('fill', e.target.value)}
          />
        </div>
        <div className="property-row">
          <label>Stroke:</label>
          <input
            type="color"
            value={localProperties.stroke || '#000000'}
            onChange={(e) => updateProperty('stroke', e.target.value)}
          />
        </div>
        <div className="property-row">
          <label>Stroke Width:</label>
          <input
            type="number"
            value={localProperties.strokeWidth || 0}
            onChange={(e) => updateProperty('strokeWidth', parseFloat(e.target.value) || 0)}
            step="1"
            min="0"
            max="50"
          />
        </div>
        <div className="property-row checkbox-row">
          <label>
            <input
              type="checkbox"
              checked={localProperties.visible !== false}
              onChange={(e) => updateProperty('visible', e.target.checked)}
            />
            Visible
          </label>
        </div>
        <div className="property-row checkbox-row">
          <label>
            <input
              type="checkbox"
              checked={localProperties.isLocked === true}
              onChange={(e) => updateProperty('isLocked', e.target.checked)}
            />
            Locked
          </label>
        </div>
      </div>
    </div>
  );

  const renderTextProperties = () => {
    if (!selectedElement || selectedElement.type !== 'text') return null;

    return (
      <div className="property-section">
        <h4 className="section-title">Text</h4>
        <div className="property-grid">
          <div className="property-row">
            <label>Text:</label>
            <textarea
              value={localProperties.text || ''}
              onChange={(e) => updateProperty('text', e.target.value)}
              rows={3}
            />
          </div>
          <div className="property-row">
            <label>Font Size:</label>
            <input
              type="number"
              value={localProperties.fontSize || 16}
              onChange={(e) => updateProperty('fontSize', parseFloat(e.target.value) || 16)}
              step="1"
              min="8"
              max="200"
            />
          </div>
          <div className="property-row">
            <label>Font Family:</label>
            <select
              value={localProperties.fontFamily || 'Arial'}
              onChange={(e) => updateProperty('fontFamily', e.target.value)}
            >
              <option value="Arial">Arial</option>
              <option value="Helvetica">Helvetica</option>
              <option value="Times New Roman">Times New Roman</option>
              <option value="Courier New">Courier New</option>
              <option value="Georgia">Georgia</option>
              <option value="Verdana">Verdana</option>
            </select>
          </div>
          <div className="property-row">
            <label>Font Weight:</label>
            <select
              value={localProperties.fontWeight || 'normal'}
              onChange={(e) => updateProperty('fontWeight', e.target.value)}
            >
              <option value="normal">Normal</option>
              <option value="bold">Bold</option>
              <option value="lighter">Lighter</option>
              <option value="bolder">Bolder</option>
            </select>
          </div>
          <div className="property-row">
            <label>Text Align:</label>
            <select
              value={localProperties.textAlign || 'left'}
              onChange={(e) => updateProperty('textAlign', e.target.value)}
            >
              <option value="left">Left</option>
              <option value="center">Center</option>
              <option value="right">Right</option>
              <option value="justify">Justify</option>
            </select>
          </div>
        </div>
      </div>
    );
  };

  const renderImageProperties = () => {
    if (!selectedElement || selectedElement.type !== 'image') return null;

    return (
      <div className="property-section">
        <h4 className="section-title">Image</h4>
        <div className="property-grid">
          <div className="property-row">
            <label>Source:</label>
            <input
              type="text"
              value={localProperties.imageSrc || ''}
              onChange={(e) => updateProperty('imageSrc', e.target.value)}
              placeholder="Image URL or path"
            />
          </div>
          <div className="property-row">
            <button
              className="file-upload-btn"
              onClick={() => {
                // TODO: Implement file upload
                console.log('Upload image');
              }}
            >
              Upload Image
            </button>
          </div>
        </div>
      </div>
    );
  };

  const renderLayerProperties = () => (
    <div className="property-section">
      <h4 className="section-title">Layer</h4>
      <div className="property-grid">
        <div className="property-row">
          <button
            className="layer-btn"
            onClick={() => {
              // TODO: Bring to front
              console.log('Bring to front');
            }}
          >
            Bring to Front
          </button>
        </div>
        <div className="property-row">
          <button
            className="layer-btn"
            onClick={() => {
              // TODO: Send to back
              console.log('Send to back');
            }}
          >
            Send to Back
          </button>
        </div>
        <div className="property-row">
          <button
            className="layer-btn"
            onClick={() => {
              // TODO: Bring forward
              console.log('Bring forward');
            }}
          >
            Bring Forward
          </button>
        </div>
        <div className="property-row">
          <button
            className="layer-btn"
            onClick={() => {
              // TODO: Send backward
              console.log('Send backward');
            }}
          >
            Send Backward
          </button>
        </div>
      </div>
    </div>
  );

  const panelClasses = `property-panel ${className}`.trim();

  if (hasMultipleSelection) {
    return (
      <div className={panelClasses}>
        <div className="panel-header">
          <h3>Multiple Selection</h3>
          <span className="selection-count">{selectedElements.length} elements</span>
        </div>
        <div className="multiple-selection-info">
          <p>Multiple elements selected. Some properties may not be available.</p>
          {renderStyleProperties()}
          {renderLayerProperties()}
        </div>
      </div>
    );
  }

  if (!selectedElement) {
    return (
      <div className={panelClasses}>
        <div className="panel-header">
          <h3>Properties</h3>
        </div>
        <div className="no-selection">
          <p>No element selected</p>
          <p className="hint">Select an element to edit its properties</p>
        </div>
      </div>
    );
  }

  return (
    <div className={panelClasses}>
      <div className="panel-header">
        <h3>Properties</h3>
        <span className="element-type">{selectedElement.type}</span>
      </div>
      
      <div className="panel-content">
        {renderTransformProperties()}
        {renderStyleProperties()}
        {renderTextProperties()}
        {renderImageProperties()}
        {renderLayerProperties()}
      </div>
    </div>
  );
});

PropertyPanel.displayName = 'PropertyPanel';