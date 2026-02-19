import React, { useState, useEffect } from 'react';
import { HexColorPicker } from 'react-colorful';
import './PropertiesPanel.css';

interface PropertiesPanelProps {
  selectedElement: any;
  canvas: any;
  onUpdate: () => void;
}

export const PropertiesPanel: React.FC<PropertiesPanelProps> = ({
  selectedElement,
  canvas,
  onUpdate,
}) => {
  const [properties, setProperties] = useState<any>({});
  const [showColorPicker, setShowColorPicker] = useState(false);
  const [activeColorField, setActiveColorField] = useState<string>('');

  useEffect(() => {
    if (selectedElement) {
      setProperties({
        fill: selectedElement.fill || '#ffffff',
        stroke: selectedElement.stroke || '',
        strokeWidth: selectedElement.strokeWidth || 0,
        opacity: selectedElement.opacity || 1,
        fontSize: selectedElement.fontSize || 16,
        fontFamily: selectedElement.fontFamily || 'Arial',
        fontWeight: selectedElement.fontWeight || 'normal',
        fontStyle: selectedElement.fontStyle || 'normal',
        textAlign: selectedElement.textAlign || 'left',
        text: selectedElement.text || '',
        left: Math.round(selectedElement.left || 0),
        top: Math.round(selectedElement.top || 0),
        width: Math.round((selectedElement.width || 0) * (selectedElement.scaleX || 1)),
        height: Math.round((selectedElement.height || 0) * (selectedElement.scaleY || 1)),
        angle: Math.round(selectedElement.angle || 0),
        skewX: Math.round(selectedElement.skewX || 0),
        skewY: Math.round(selectedElement.skewY || 0),
        flipX: selectedElement.flipX || false,
        flipY: selectedElement.flipY || false,
        shadow: selectedElement.shadow || null,
        // Data binding properties
        dataSource: selectedElement.dataSource || '',
        dataFormat: selectedElement.dataFormat || '',
        dataUnit: selectedElement.dataUnit || '',
        // Analog hand properties
        handType: selectedElement.handType || 'hour',
        pivotX: selectedElement.pivotX || 0,
        pivotY: selectedElement.pivotY || 0,
        // Progress bar properties
        progressType: selectedElement.progressType || 'linear',
        minValue: selectedElement.minValue || 0,
        maxValue: selectedElement.maxValue || 100,
        currentValue: selectedElement.currentValue || 50,
      });
    }
  }, [selectedElement]);

  const updateProperty = (key: string, value: any) => {
    if (!selectedElement) return;

    setProperties(prev => ({ ...prev, [key]: value }));

    if (key === 'left' || key === 'top') {
      selectedElement.set(key, value);
    } else if (key === 'width') {
      const scaleX = value / (selectedElement.width || 1);
      selectedElement.set('scaleX', scaleX);
    } else if (key === 'height') {
      const scaleY = value / (selectedElement.height || 1);
      selectedElement.set('scaleY', scaleY);
    } else if (key === 'angle') {
      selectedElement.set('angle', value);
    } else if (key === 'flipX' || key === 'flipY') {
      selectedElement.set(key, value);
    } else {
      selectedElement.set(key, value);
    }

    canvas?.renderAll();
    onUpdate();
  };

  const handleColorChange = (color: string) => {
    if (activeColorField) {
      updateProperty(activeColorField, color);
    }
  };

  const openColorPicker = (field: string) => {
    setActiveColorField(field);
    setShowColorPicker(true);
  };

  const addShadow = () => {
    if (!selectedElement) return;
    const shadow = new (window as any).fabric.Shadow({
      color: 'rgba(0,0,0,0.3)',
      blur: 5,
      offsetX: 2,
      offsetY: 2,
    });
    selectedElement.set('shadow', shadow);
    canvas?.renderAll();
    onUpdate();
  };

  const removeShadow = () => {
    if (!selectedElement) return;
    selectedElement.set('shadow', null);
    canvas?.renderAll();
    onUpdate();
  };

  if (!selectedElement) {
    return (
      <div className="properties-panel">
        <div className="properties-header">
          <h3>Properties</h3>
        </div>
        <div className="no-selection">
          <p>Select an element to edit properties</p>
        </div>
      </div>
    );
  }

  const elementType = selectedElement.elementType || 'Element';
  const isText = selectedElement.type === 'text';
  const isAnalogHand = elementType === 'analogHand';
  const isProgressBar = elementType === 'progressBar';

  return (
    <div className="properties-panel">
      <div className="properties-header">
        <h3>Properties</h3>
        <span className="element-type">{elementType}</span>
      </div>

      <div className="properties-content">
        {/* Position and Size */}
        <div className="property-section">
          <h4>Position & Size</h4>
          <div className="property-grid">
            <div className="property-item">
              <label>X</label>
              <input
                type="number"
                value={properties.left}
                onChange={(e) => updateProperty('left', parseInt(e.target.value) || 0)}
              />
            </div>
            <div className="property-item">
              <label>Y</label>
              <input
                type="number"
                value={properties.top}
                onChange={(e) => updateProperty('top', parseInt(e.target.value) || 0)}
              />
            </div>
            <div className="property-item">
              <label>Width</label>
              <input
                type="number"
                value={properties.width}
                onChange={(e) => updateProperty('width', parseInt(e.target.value) || 1)}
              />
            </div>
            <div className="property-item">
              <label>Height</label>
              <input
                type="number"
                value={properties.height}
                onChange={(e) => updateProperty('height', parseInt(e.target.value) || 1)}
              />
            </div>
          </div>
        </div>

        {/* Transform */}
        <div className="property-section">
          <h4>Transform</h4>
          <div className="property-grid">
            <div className="property-item">
              <label>Rotation (°)</label>
              <input
                type="number"
                value={properties.angle}
                onChange={(e) => updateProperty('angle', parseInt(e.target.value) || 0)}
              />
            </div>
            <div className="property-item">
              <label>Skew X</label>
              <input
                type="number"
                value={properties.skewX}
                onChange={(e) => updateProperty('skewX', parseInt(e.target.value) || 0)}
              />
            </div>
            <div className="property-item">
              <label>Skew Y</label>
              <input
                type="number"
                value={properties.skewY}
                onChange={(e) => updateProperty('skewY', parseInt(e.target.value) || 0)}
              />
            </div>
          </div>
          <div className="property-grid">
            <div className="property-item">
              <label>
                <input
                  type="checkbox"
                  checked={properties.flipX}
                  onChange={(e) => updateProperty('flipX', e.target.checked)}
                />
                Flip X
              </label>
            </div>
            <div className="property-item">
              <label>
                <input
                  type="checkbox"
                  checked={properties.flipY}
                  onChange={(e) => updateProperty('flipY', e.target.checked)}
                />
                Flip Y
              </label>
            </div>
          </div>
        </div>

        {/* Text Properties */}
        {isText && (
          <div className="property-section">
            <h4>Text</h4>
            <div className="property-item">
              <label>Content</label>
              <textarea
                value={properties.text}
                onChange={(e) => updateProperty('text', e.target.value)}
                rows={3}
              />
            </div>
            <div className="property-grid">
              <div className="property-item">
                <label>Font Size</label>
                <input
                  type="number"
                  value={properties.fontSize}
                  onChange={(e) => updateProperty('fontSize', parseInt(e.target.value) || 16)}
                />
              </div>
              <div className="property-item">
                <label>Font Family</label>
                <select
                  value={properties.fontFamily}
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
            </div>
            <div className="property-grid">
              <div className="property-item">
                <label>Font Weight</label>
                <select
                  value={properties.fontWeight}
                  onChange={(e) => updateProperty('fontWeight', e.target.value)}
                >
                  <option value="normal">Normal</option>
                  <option value="bold">Bold</option>
                  <option value="lighter">Lighter</option>
                  <option value="bolder">Bolder</option>
                </select>
              </div>
              <div className="property-item">
                <label>Font Style</label>
                <select
                  value={properties.fontStyle}
                  onChange={(e) => updateProperty('fontStyle', e.target.value)}
                >
                  <option value="normal">Normal</option>
                  <option value="italic">Italic</option>
                  <option value="oblique">Oblique</option>
                </select>
              </div>
            </div>
            <div className="property-item">
              <label>Text Align</label>
              <select
                value={properties.textAlign}
                onChange={(e) => updateProperty('textAlign', e.target.value)}
              >
                <option value="left">Left</option>
                <option value="center">Center</option>
                <option value="right">Right</option>
                <option value="justify">Justify</option>
              </select>
            </div>
          </div>
        )}

        {/* Data Binding for Watch Elements */}
        {(elementType === 'time' || elementType === 'date' || elementType === 'battery' || 
          elementType === 'steps' || elementType === 'heartRate' || elementType === 'weather') && (
          <div className="property-section">
            <h4>Data Binding</h4>
            <div className="property-item">
              <label>Data Source</label>
              <select
                value={properties.dataSource}
                onChange={(e) => updateProperty('dataSource', e.target.value)}
              >
                <option value="">Select Data Source</option>
                <option value="time.hour">Hour</option>
                <option value="time.minute">Minute</option>
                <option value="time.second">Second</option>
                <option value="date.day">Day</option>
                <option value="date.month">Month</option>
                <option value="date.year">Year</option>
                <option value="date.weekday">Weekday</option>
                <option value="battery.level">Battery Level</option>
                <option value="health.steps">Steps</option>
                <option value="health.heartRate">Heart Rate</option>
                <option value="health.calories">Calories</option>
                <option value="weather.temperature">Temperature</option>
                <option value="weather.condition">Weather Condition</option>
              </select>
            </div>
            <div className="property-item">
              <label>Format</label>
              <input
                type="text"
                value={properties.dataFormat}
                onChange={(e) => updateProperty('dataFormat', e.target.value)}
                placeholder="e.g., HH:mm, DD/MM/YYYY"
              />
            </div>
            <div className="property-item">
              <label>Unit</label>
              <input
                type="text"
                value={properties.dataUnit}
                onChange={(e) => updateProperty('dataUnit', e.target.value)}
                placeholder="e.g., %, bpm, °C"
              />
            </div>
          </div>
        )}

        {/* Analog Hand Properties */}
        {isAnalogHand && (
          <div className="property-section">
            <h4>Analog Hand</h4>
            <div className="property-item">
              <label>Hand Type</label>
              <select
                value={properties.handType}
                onChange={(e) => updateProperty('handType', e.target.value)}
              >
                <option value="hour">Hour Hand</option>
                <option value="minute">Minute Hand</option>
                <option value="second">Second Hand</option>
              </select>
            </div>
            <div className="property-grid">
              <div className="property-item">
                <label>Pivot X</label>
                <input
                  type="number"
                  value={properties.pivotX}
                  onChange={(e) => updateProperty('pivotX', parseInt(e.target.value) || 0)}
                />
              </div>
              <div className="property-item">
                <label>Pivot Y</label>
                <input
                  type="number"
                  value={properties.pivotY}
                  onChange={(e) => updateProperty('pivotY', parseInt(e.target.value) || 0)}
                />
              </div>
            </div>
          </div>
        )}

        {/* Progress Bar Properties */}
        {isProgressBar && (
          <div className="property-section">
            <h4>Progress Bar</h4>
            <div className="property-item">
              <label>Progress Type</label>
              <select
                value={properties.progressType}
                onChange={(e) => updateProperty('progressType', e.target.value)}
              >
                <option value="linear">Linear</option>
                <option value="circular">Circular</option>
                <option value="arc">Arc</option>
              </select>
            </div>
            <div className="property-grid">
              <div className="property-item">
                <label>Min Value</label>
                <input
                  type="number"
                  value={properties.minValue}
                  onChange={(e) => updateProperty('minValue', parseInt(e.target.value) || 0)}
                />
              </div>
              <div className="property-item">
                <label>Max Value</label>
                <input
                  type="number"
                  value={properties.maxValue}
                  onChange={(e) => updateProperty('maxValue', parseInt(e.target.value) || 100)}
                />
              </div>
            </div>
            <div className="property-item">
              <label>Current Value</label>
              <input
                type="range"
                min={properties.minValue}
                max={properties.maxValue}
                value={properties.currentValue}
                onChange={(e) => updateProperty('currentValue', parseInt(e.target.value))}
              />
              <span className="range-value">{properties.currentValue}</span>
            </div>
          </div>
        )}

        {/* Appearance */}
        <div className="property-section">
          <h4>Appearance</h4>
          <div className="property-item">
            <label>Fill Color</label>
            <div className="color-input-wrapper">
              <div
                className="color-swatch"
                style={{ backgroundColor: properties.fill }}
                onClick={() => openColorPicker('fill')}
              />
              <input
                type="text"
                value={properties.fill}
                onChange={(e) => updateProperty('fill', e.target.value)}
              />
            </div>
          </div>

          <div className="property-item">
            <label>Opacity</label>
            <input
              type="range"
              min="0"
              max="1"
              step="0.1"
              value={properties.opacity}
              onChange={(e) => updateProperty('opacity', parseFloat(e.target.value))}
            />
            <span className="range-value">{Math.round(properties.opacity * 100)}%</span>
          </div>

          <div className="property-grid">
            <div className="property-item">
              <label>Stroke Color</label>
              <div className="color-input-wrapper">
                <div
                  className="color-swatch"
                  style={{ backgroundColor: properties.stroke || '#000000' }}
                  onClick={() => openColorPicker('stroke')}
                />
                <input
                  type="text"
                  value={properties.stroke}
                  onChange={(e) => updateProperty('stroke', e.target.value)}
                />
              </div>
            </div>
            <div className="property-item">
              <label>Stroke Width</label>
              <input
                type="number"
                min="0"
                value={properties.strokeWidth}
                onChange={(e) => updateProperty('strokeWidth', parseInt(e.target.value) || 0)}
              />
            </div>
          </div>

          {/* Shadow Controls */}
          <div className="property-item">
            <label>Shadow</label>
            <div className="shadow-controls">
              {properties.shadow ? (
                <button onClick={removeShadow} className="btn btn-secondary">
                  Remove Shadow
                </button>
              ) : (
                <button onClick={addShadow} className="btn btn-secondary">
                  Add Shadow
                </button>
              )}
            </div>
          </div>

          {showColorPicker && (
            <div className="color-picker-popup">
              <HexColorPicker
                color={properties[activeColorField] || '#ffffff'}
                onChange={handleColorChange}
              />
              <button
                onClick={() => setShowColorPicker(false)}
                className="close-picker"
              >
                Close
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};