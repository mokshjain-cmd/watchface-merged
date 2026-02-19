import React, { useState } from 'react';
import { AssemblyView } from './components/AssemblyView';
import { PreviewView } from './components/PreviewView';
import './App.css';

export interface WatchFaceComponent {
  id: string;
  type: 'image' | 'dateTime' | 'progress' | 'number' | 'analog';
  elementType: number | null; // 0=time, 1=blood oxygen, 2=battery, etc.
  timeComponent?: number; // 0=year, 1=month, 2=day, 3=hour, 4=minute, 5=second, 6=week, 7=ampm
  name: string;
  images: string[]; // Array of image URLs/paths
  position: { x: number; y: number };
  size: { width: number; height: number };
  config: {
    dateTimeFormat?: string; // 'Hour', 'Minute', 'Second', etc.
    leadingZero?: boolean;
    minValue?: number;
    maxValue?: number;
    defaultValue?: number;
    fillType?: boolean;
    targetValue?: number;
    digitPosition?: 'tens' | 'ones' | 'single' | 'thousands' | 'hundreds' | 'multi'; // For multi-digit time components
    itemName?: string; // From Excel data - Steps, Heart Rate, etc.
    // Analog clock hand properties (DragBindPoint)
    startAngle?: number;
    endAngle?: number;
    originPointX?: number;
    originPointY?: number;
    valueIndex?: number; // 0=hour, 1=minute, 2=second
    value?: number;
    angle?: number;
    // Separator properties
    isSeparator?: boolean;
    separatorType?: 'time' | 'date';
  };
}

export interface WatchFaceProject {
  id: string;
  watchName: string;
  watchCode: string;
  deviceType: string;
  width: number;
  height: number;
  watchShape: 'square' | 'circle';
  createTime: string;
  colorBit: number;
  folderName: string;
  components: WatchFaceComponent[];
}

function App() {
  const [currentView, setCurrentView] = useState<'assembly' | 'preview'>('assembly');
  
  // Read width and height from URL parameters
  const urlParams = new URLSearchParams(window.location.search);
  const urlWidth = urlParams.get('width');
  const urlHeight = urlParams.get('height');
  const urlShape = urlParams.get('shape');
  
  // Determine initial dimensions
  const initialWidth = urlWidth ? parseInt(urlWidth) : 466;
  const initialHeight = urlHeight ? parseInt(urlHeight) : 466;
  
  // Determine watch shape based on URL parameter or dimensions
  const getWatchShape = (shape: string | null, w: number, h: number): 'square' | 'circle' => {
    if (shape) {
      return shape.toLowerCase().includes('circular') ? 'circle' : 'square';
    }
    return w === h ? 'circle' : 'square';
  };
  
  const [currentProject, setCurrentProject] = useState<WatchFaceProject>({
    id: 'project-' + Date.now(),
    watchName: 'My Watch Face',
    watchCode: 'MWF',
    deviceType: 'Hsad',
    width: initialWidth,
    height: initialHeight,
    watchShape: getWatchShape(urlShape, initialWidth, initialHeight),
    createTime: new Date().toISOString(),
    colorBit: 16,
    folderName: 'MWF_MyWatchFace_Hsad',
    components: []
  });

  const handleProjectUpdate = (project: WatchFaceProject) => {
    setCurrentProject(project);
  };

  return (
    <div className="app">
      <div className="app-header">
        <h1>Watch Face Assembly Tool</h1>
        <div className="app-nav">
          <button 
            className={`nav-btn ${currentView === 'assembly' ? 'active' : ''}`}
            onClick={() => setCurrentView('assembly')}
          >
            Assembly
          </button>
          <button 
            className={`nav-btn ${currentView === 'preview' ? 'active' : ''}`}
            onClick={() => setCurrentView('preview')}
          >
            Preview
          </button>
        </div>
      </div>
      
      <div className="app-content">
        {currentView === 'assembly' ? (
          <AssemblyView 
            project={currentProject}
            onProjectUpdate={handleProjectUpdate}
          />
        ) : (
          <PreviewView 
            project={currentProject}
          />
        )}
      </div>
    </div>
  );
}

export default App;