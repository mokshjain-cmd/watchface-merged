import React from 'react';
import { 
  Clock, Calendar, Battery, Activity, Square, Circle, Image, 
  Trash2, Heart, Thermometer, Eye, Zap, Timer, MapPin,
  RotateCcw, RotateCw, Copy, Move, Hand
} from 'lucide-react';
import './Toolbar.css';

interface ToolbarProps {
  onAddTime: () => void;
  onAddDate: () => void;
  onAddBattery: () => void;
  onAddSteps: () => void;
  onAddHeartRate: () => void;
  onAddWeather: () => void;
  onAddCalories: () => void;
  onAddDistance: () => void;
  onAddShape: (type: 'circle' | 'rectangle' | 'line' | 'arc') => void;
  onAddImage: () => void;
  onAddAnalogHands: () => void;
  onAddDigitalNumber: () => void;
  onAddProgressBar: () => void;
  onAddPointer: () => void;
  onDelete: () => void;
  onDuplicate: () => void;
  onUndo: () => void;
  onRedo: () => void;
  hasSelection: boolean;
  canUndo: boolean;
  canRedo: boolean;
  currentTool: string;
  onToolChange: (tool: string) => void;
}

export const Toolbar: React.FC<ToolbarProps> = ({
  onAddTime,
  onAddDate,
  onAddBattery,
  onAddSteps,
  onAddHeartRate,
  onAddWeather,
  onAddCalories,
  onAddDistance,
  onAddShape,
  onAddImage,
  onAddAnalogHands,
  onAddDigitalNumber,
  onAddProgressBar,
  onAddPointer,
  onDelete,
  onDuplicate,
  onUndo,
  onRedo,
  hasSelection,
  canUndo,
  canRedo,
  currentTool,
  onToolChange,
}) => {
  return (
    <div className="toolbar">
      {/* Tools Section */}
      <div className="toolbar-section">
        <h3>Tools</h3>
        <button 
          onClick={() => onToolChange('select')}
          className={`tool-btn ${currentTool === 'select' ? 'active' : ''}`}
          title="Select Tool"
        >
          <Move size={20} />
          <span>Select</span>
        </button>
        <button 
          onClick={() => onToolChange('pan')}
          className={`tool-btn ${currentTool === 'pan' ? 'active' : ''}`}
          title="Pan Tool"
        >
          <Hand size={20} />
          <span>Pan</span>
        </button>
      </div>

      {/* Digital Watch Data */}
      <div className="toolbar-section">
        <h3>Digital Elements</h3>
        <button onClick={onAddTime} className="tool-btn" title="Add Digital Time">
          <Clock size={20} />
          <span>Digital Time</span>
        </button>
        <button onClick={onAddDate} className="tool-btn" title="Add Date">
          <Calendar size={20} />
          <span>Date</span>
        </button>
        <button onClick={onAddDigitalNumber} className="tool-btn" title="Add Digital Numbers">
          <Timer size={20} />
          <span>Digital Numbers</span>
        </button>
      </div>

      {/* Analog Elements */}
      <div className="toolbar-section">
        <h3>Analog Elements</h3>
        <button onClick={onAddAnalogHands} className="tool-btn" title="Add Analog Hands">
          <RotateCcw size={20} />
          <span>Analog Hands</span>
        </button>
        <button onClick={onAddPointer} className="tool-btn" title="Add Pointer">
          <MapPin size={20} />
          <span>Pointer</span>
        </button>
      </div>

      {/* Health & Fitness Data */}
      <div className="toolbar-section">
        <h3>Health & Fitness</h3>
        <button onClick={onAddBattery} className="tool-btn" title="Add Battery">
          <Battery size={20} />
          <span>Battery</span>
        </button>
        <button onClick={onAddSteps} className="tool-btn" title="Add Steps">
          <Activity size={20} />
          <span>Steps</span>
        </button>
        <button onClick={onAddHeartRate} className="tool-btn" title="Add Heart Rate">
          <Heart size={20} />
          <span>Heart Rate</span>
        </button>
        <button onClick={onAddCalories} className="tool-btn" title="Add Calories">
          <Zap size={20} />
          <span>Calories</span>
        </button>
        <button onClick={onAddDistance} className="tool-btn" title="Add Distance">
          <MapPin size={20} />
          <span>Distance</span>
        </button>
      </div>

      {/* Weather */}
      <div className="toolbar-section">
        <h3>Weather</h3>
        <button onClick={onAddWeather} className="tool-btn" title="Add Weather">
          <Thermometer size={20} />
          <span>Weather</span>
        </button>
      </div>

      {/* Shapes & Graphics */}
      <div className="toolbar-section">
        <h3>Shapes & Graphics</h3>
        <button 
          onClick={() => onAddShape('circle')} 
          className="tool-btn" 
          title="Add Circle"
        >
          <Circle size={20} />
          <span>Circle</span>
        </button>
        <button 
          onClick={() => onAddShape('rectangle')} 
          className="tool-btn" 
          title="Add Rectangle"
        >
          <Square size={20} />
          <span>Rectangle</span>
        </button>
        <button 
          onClick={() => onAddShape('line')} 
          className="tool-btn" 
          title="Add Line"
        >
          <span style={{fontSize: '20px'}}>━</span>
          <span>Line</span>
        </button>
        <button 
          onClick={() => onAddShape('arc')} 
          className="tool-btn" 
          title="Add Arc"
        >
          <span style={{fontSize: '20px'}}>◗</span>
          <span>Arc</span>
        </button>
        <button onClick={onAddProgressBar} className="tool-btn" title="Add Progress Bar">
          <span style={{fontSize: '20px'}}>▬</span>
          <span>Progress Bar</span>
        </button>
      </div>

      {/* Media */}
      <div className="toolbar-section">
        <h3>Media</h3>
        <button onClick={onAddImage} className="tool-btn" title="Add Image">
          <Image size={20} />
          <span>Image</span>
        </button>
      </div>

      {/* Actions */}
      <div className="toolbar-section">
        <h3>Actions</h3>
        <button 
          onClick={onUndo} 
          className={`tool-btn ${!canUndo ? 'disabled' : ''}`}
          disabled={!canUndo}
          title="Undo"
        >
          <RotateCcw size={20} />
          <span>Undo</span>
        </button>
        <button 
          onClick={onRedo} 
          className={`tool-btn ${!canRedo ? 'disabled' : ''}`}
          disabled={!canRedo}
          title="Redo"
        >
          <RotateCw size={20} />
          <span>Redo</span>
        </button>
        <button 
          onClick={onDuplicate} 
          className={`tool-btn ${!hasSelection ? 'disabled' : ''}`}
          disabled={!hasSelection}
          title="Duplicate Selected"
        >
          <Copy size={20} />
          <span>Duplicate</span>
        </button>
        <button 
          onClick={onDelete} 
          className={`tool-btn danger ${!hasSelection ? 'disabled' : ''}`}
          disabled={!hasSelection}
          title="Delete Selected"
        >
          <Trash2 size={20} />
          <span>Delete</span>
        </button>
      </div>
    </div>
  );
};