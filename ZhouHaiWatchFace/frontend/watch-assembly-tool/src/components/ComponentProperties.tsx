import React, { useState } from 'react';
import { Trash2, Upload, GripVertical } from 'lucide-react';
import { WatchFaceComponent } from '../App';
import './ComponentProperties.css';

interface ComponentPropertiesProps {
  component: WatchFaceComponent;
  onUpdate: (component: WatchFaceComponent) => void;
  onDelete: () => void;
}

export const ComponentProperties: React.FC<ComponentPropertiesProps> = ({
  component,
  onUpdate,
  onDelete
}) => {
  const [draggedIndex, setDraggedIndex] = useState<number | null>(null);
  const [dragOverIndex, setDragOverIndex] = useState<number | null>(null);

  const handlePropertyChange = (property: string, value: any) => {
    if (property.startsWith('config.')) {
      const configProperty = property.replace('config.', '');
      onUpdate({
        ...component,
        config: {
          ...component.config,
          [configProperty]: value
        }
      });
    } else if (property.startsWith('position.')) {
      const posProperty = property.replace('position.', '');
      onUpdate({
        ...component,
        position: {
          ...component.position,
          [posProperty]: parseFloat(value) || 0
        }
      });
    } else if (property.startsWith('size.')) {
      const sizeProperty = property.replace('size.', '');
      onUpdate({
        ...component,
        size: {
          ...component.size,
          [sizeProperty]: parseFloat(value) || 0
        }
      });
    } else {
      onUpdate({
        ...component,
        [property]: value
      });
    }
  };

  const getElementTypeName = (elementType: number | null) => {
    switch (elementType) {
      case 0: return 'Time';
      case 1: return 'Blood Oxygen';
      case 2: return 'Battery';
      case 3: return 'Steps';
      case 4: return 'Heart Rate';
      default: return 'Static';
    }
  };

  const handleDragStart = (index: number) => {
    setDraggedIndex(index);
  };

  const handleDragOver = (e: React.DragEvent, index: number) => {
    e.preventDefault();
    setDragOverIndex(index);
  };

  const handleDragLeave = () => {
    setDragOverIndex(null);
  };

  const handleDrop = (e: React.DragEvent, dropIndex: number) => {
    e.preventDefault();
    
    if (draggedIndex === null || draggedIndex === dropIndex) {
      setDraggedIndex(null);
      setDragOverIndex(null);
      return;
    }

    // Reorder images
    const newImages = [...component.images];
    const [draggedImage] = newImages.splice(draggedIndex, 1);
    newImages.splice(dropIndex, 0, draggedImage);

    // Update component with reordered images
    onUpdate({
      ...component,
      images: newImages
    });

    setDraggedIndex(null);
    setDragOverIndex(null);
  };

  const handleDragEnd = () => {
    setDraggedIndex(null);
    setDragOverIndex(null);
  };

  const getDateTimeFormatOptions = () => {
    return ['Hour', 'Minute', 'Second', 'Day', 'Month', 'Year', 'WeekDay'];
  };

  return (
    <div className="component-properties">
      <div className="property-group">
        <h4>Basic Properties</h4>
        
        <div className="property-row">
          <label className="property-label">Name:</label>
          <input
            type="text"
            className="property-input"
            value={component.name}
            onChange={(e) => handlePropertyChange('name', e.target.value)}
          />
        </div>

        <div className="property-row">
          <label className="property-label">Type:</label>
          <select
            className="property-input"
            value={component.type}
            onChange={(e) => handlePropertyChange('type', e.target.value)}
          >
            <option value="image">Static Image</option>
            <option value="dateTime">Date/Time</option>
            <option value="progress">Progress Bar</option>
            <option value="number">Number Display</option>
          </select>
        </div>

        <div className="property-row">
          <label className="property-label">Element:</label>
          <select
            className="property-input"
            value={component.elementType !== null ? component.elementType.toString() : ''}
            onChange={(e) => {
              const value = e.target.value;
              const numValue = value ? parseInt(value) : null;
              handlePropertyChange('elementType', numValue);
            }}
          >
            <option value="">Static</option>
            <option value="0">Time</option>
            <option value="1">Blood Oxygen</option>
            <option value="2">Battery</option>
            <option value="3">Steps</option>
            <option value="4">Heart Rate</option>
          </select>
        </div>

        {component.elementType === 0 && (
          <div className="property-row">
            <label className="property-label">Time Component:</label>
            <select
              className="property-input"
              value={component.timeComponent !== undefined ? component.timeComponent.toString() : ''}
              onChange={(e) => {
                const value = e.target.value;
                const numValue = value ? parseInt(value) : undefined;
                handlePropertyChange('timeComponent', numValue);
              }}
            >
              <option value="">Select Component</option>
              <option value="0">Year</option>
              <option value="1">Month</option>
              <option value="2">Day</option>
              <option value="3">Hour</option>
              <option value="4">Minute</option>
              <option value="5">Second</option>
              <option value="6">Week</option>
              <option value="7">AM/PM</option>
            </select>
          </div>
        )}

        {component.config.digitPosition && (
          <div className="property-row">
            <label className="property-label">Digit Position:</label>
            <select
              className="property-input"
              value={component.config.digitPosition}
              onChange={(e) => handlePropertyChange('config.digitPosition', e.target.value)}
            >
              <option value="single">Single Digit</option>
              <option value="multi">Multi-Digit (Full Number)</option>
              <option value="thousands">Thousands Digit</option>
              <option value="hundreds">Hundreds Digit</option>
              <option value="tens">Tens Digit</option>
              <option value="ones">Ones Digit</option>
            </select>
          </div>
        )}
      </div>

      <div className="property-group">
        <h4>Position & Size</h4>
        
        <div className="property-row">
          <label className="property-label">X:</label>
          <input
            type="number"
            className="property-input"
            value={component.position.x}
            onChange={(e) => handlePropertyChange('position.x', e.target.value)}
          />
        </div>

        <div className="property-row">
          <label className="property-label">Y:</label>
          <input
            type="number"
            className="property-input"
            value={component.position.y}
            onChange={(e) => handlePropertyChange('position.y', e.target.value)}
          />
        </div>

        <div className="property-row">
          <label className="property-label">Width:</label>
          <input
            type="number"
            className="property-input"
            value={component.size.width}
            onChange={(e) => handlePropertyChange('size.width', e.target.value)}
          />
        </div>

        <div className="property-row">
          <label className="property-label">Height:</label>
          <input
            type="number"
            className="property-input"
            value={component.size.height}
            onChange={(e) => handlePropertyChange('size.height', e.target.value)}
          />
        </div>
      </div>

      {component.type === 'dateTime' && (
        <div className="property-group">
          <h4>Date/Time Configuration</h4>
          
          <div className="property-row">
            <label className="property-label">Format:</label>
            <select
              className="property-input"
              value={component.config.dateTimeFormat || 'Hour'}
              onChange={(e) => handlePropertyChange('config.dateTimeFormat', e.target.value)}
            >
              {getDateTimeFormatOptions().map(format => (
                <option key={format} value={format}>{format}</option>
              ))}
            </select>
          </div>

          <div className="property-row">
            <label className="property-label">Leading Zero:</label>
            <input
              type="checkbox"
              className="property-checkbox"
              checked={component.config.leadingZero || false}
              onChange={(e) => handlePropertyChange('config.leadingZero', e.target.checked)}
            />
          </div>
        </div>
      )}

      {component.type === 'progress' && (
        <div className="property-group">
          <h4>Progress Configuration</h4>
          
          <div className="property-row">
            <label className="property-label">Min Value:</label>
            <input
              type="number"
              className="property-input"
              value={component.config.minValue || 0}
              onChange={(e) => handlePropertyChange('config.minValue', parseInt(e.target.value))}
            />
          </div>

          <div className="property-row">
            <label className="property-label">Max Value:</label>
            <input
              type="number"
              className="property-input"
              value={component.config.maxValue || 100}
              onChange={(e) => handlePropertyChange('config.maxValue', parseInt(e.target.value))}
            />
          </div>

          <div className="property-row">
            <label className="property-label">Default:</label>
            <input
              type="number"
              className="property-input"
              value={component.config.defaultValue || 50}
              onChange={(e) => handlePropertyChange('config.defaultValue', parseInt(e.target.value))}
            />
          </div>

          <div className="property-row">
            <label className="property-label">Fill Type:</label>
            <input
              type="checkbox"
              className="property-checkbox"
              checked={component.config.fillType || false}
              onChange={(e) => handlePropertyChange('config.fillType', e.target.checked)}
            />
          </div>
        </div>
      )}

      <div className="property-group">
        <h4>Images ({component.images.length})</h4>
        <p className="help-text">Drag and drop to reorder images</p>
        
        <div className="image-gallery">
          {component.images.map((image, index) => (
            <div 
              key={index} 
              className={`image-thumbnail ${draggedIndex === index ? 'dragging' : ''} ${dragOverIndex === index ? 'drag-over' : ''}`}
              draggable
              onDragStart={() => handleDragStart(index)}
              onDragOver={(e) => handleDragOver(e, index)}
              onDragLeave={handleDragLeave}
              onDrop={(e) => handleDrop(e, index)}
              onDragEnd={handleDragEnd}
            >
              <div className="drag-handle">
                <GripVertical size={16} />
              </div>
              <img src={image} alt={`Image ${index}`} />
              <span className="image-index">{index}</span>
            </div>
          ))}
        </div>

        {component.images.length === 0 && (
          <div className="no-images">
            <p>No images uploaded</p>
          </div>
        )}
      </div>

      <div className="property-group">
        <h4>Actions</h4>
        
        <div className="action-buttons">
          <button className="btn btn-secondary">
            <Upload size={16} />
            Replace Images
          </button>
          
          <button className="btn btn-danger" onClick={onDelete}>
            <Trash2 size={16} />
            Delete Component
          </button>
        </div>
      </div>
    </div>
  );
};