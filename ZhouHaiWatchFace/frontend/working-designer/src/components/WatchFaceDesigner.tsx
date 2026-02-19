import React, { useEffect, useRef, useState } from 'react';
import * as fabric from 'fabric';
import { Toolbar } from './Toolbar';
import { PropertiesPanel } from './PropertiesPanel';
import { ExportPanel } from './ExportPanel';
import './WatchFaceDesigner.css';

export interface WatchElement {
  id: string;
  type: 'time' | 'date' | 'battery' | 'steps' | 'heartRate' | 'weather' | 'calories' | 'distance' | 
        'image' | 'text' | 'shape' | 'analogHand' | 'digitalNumber' | 'progressBar' | 'pointer';
  x: number;
  y: number;
  width: number;
  height: number;
  properties: Record<string, any>;
}

export interface WatchFaceData {
  id: string;
  name: string;
  width: number;
  height: number;
  background?: string;
  elements: WatchElement[];
  createdAt: string;
  version: string;
}

export const WatchFaceDesigner: React.FC = () => {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [canvas, setCanvas] = useState<fabric.Canvas | null>(null);
  const [selectedElement, setSelectedElement] = useState<fabric.Object | null>(null);
  const [currentTool, setCurrentTool] = useState('select');
  const [history, setHistory] = useState<string[]>([]);
  const [historyIndex, setHistoryIndex] = useState(-1);
  const [watchData, setWatchData] = useState<WatchFaceData>({
    id: 'watch-' + Date.now(),
    name: 'My Watch Face',
    width: 450,
    height: 450,
    elements: [],
    createdAt: new Date().toISOString(),
    version: '1.0'
  });

  useEffect(() => {
    if (canvasRef.current) {
      const fabricCanvas = new fabric.Canvas(canvasRef.current, {
        width: 450,
        height: 450,
        backgroundColor: '#000000',
        preserveObjectStacking: true,
      });

      // Add watch face background circle
      const watchBorder = new fabric.Circle({
        left: 0,
        top: 0,
        radius: 225,
        fill: 'transparent',
        stroke: '#333',
        strokeWidth: 2,
        selectable: false,
        evented: false,
      });
      fabricCanvas.add(watchBorder);

      // Handle object selection
      fabricCanvas.on('selection:created', (e: any) => {
        setSelectedElement(e.selected?.[0] || null);
      });

      fabricCanvas.on('selection:updated', (e: any) => {
        setSelectedElement(e.selected?.[0] || null);
      });

      fabricCanvas.on('selection:cleared', () => {
        setSelectedElement(null);
      });

      // Handle object modifications for history
      fabricCanvas.on('object:modified', () => {
        saveToHistory(fabricCanvas);
      });

      fabricCanvas.on('object:added', () => {
        saveToHistory(fabricCanvas);
      });

      fabricCanvas.on('object:removed', () => {
        saveToHistory(fabricCanvas);
      });

      setCanvas(fabricCanvas);
      
      // Save initial state
      saveToHistory(fabricCanvas);

      return () => {
        fabricCanvas.dispose();
      };
    }
  }, []);

  const saveToHistory = (fabricCanvas: fabric.Canvas) => {
    const state = JSON.stringify(fabricCanvas.toJSON());
    setHistory(prev => {
      const newHistory = prev.slice(0, historyIndex + 1);
      newHistory.push(state);
      return newHistory.slice(-50); // Keep last 50 states
    });
    setHistoryIndex(prev => prev + 1);
    updateWatchData(fabricCanvas);
  };

  const updateWatchData = (fabricCanvas: fabric.Canvas) => {
    const objects = fabricCanvas.getObjects().filter(obj => obj.selectable);
    const elements: WatchElement[] = objects.map((obj, index) => ({
      id: `element-${index}`,
      type: (obj as any).elementType || 'shape',
      x: obj.left || 0,
      y: obj.top || 0,
      width: (obj.width || 0) * (obj.scaleX || 1),
      height: (obj.height || 0) * (obj.scaleY || 1),
      properties: {
        fill: (obj as any).fill,
        stroke: (obj as any).stroke,
        strokeWidth: (obj as any).strokeWidth,
        text: (obj as any).text,
        fontSize: (obj as any).fontSize,
        fontFamily: (obj as any).fontFamily,
        fontWeight: (obj as any).fontWeight,
        opacity: obj.opacity,
        angle: obj.angle,
        dataSource: (obj as any).dataSource,
        dataFormat: (obj as any).dataFormat,
        handType: (obj as any).handType,
      }
    }));

    setWatchData(prev => ({
      ...prev,
      elements
    }));
  };

  // Element creation functions
  const addTimeElement = () => {
    if (!canvas) return;
    
    const timeText = new fabric.Text('12:34', {
      left: 175,
      top: 200,
      fontSize: 48,
      fill: '#ffffff',
      fontFamily: 'Arial, sans-serif',
      fontWeight: 'bold',
    });
    
    (timeText as any).elementType = 'time';
    (timeText as any).dataSource = 'time.current';
    (timeText as any).dataFormat = 'HH:mm';
    canvas.add(timeText);
    canvas.setActiveObject(timeText);
  };

  const addDateElement = () => {
    if (!canvas) return;
    
    const dateText = new fabric.Text('Wed 06', {
      left: 185,
      top: 260,
      fontSize: 18,
      fill: '#cccccc',
      fontFamily: 'Arial, sans-serif',
    });
    
    (dateText as any).elementType = 'date';
    (dateText as any).dataSource = 'date.current';
    (dateText as any).dataFormat = 'ddd DD';
    canvas.add(dateText);
    canvas.setActiveObject(dateText);
  };

  const addBatteryElement = () => {
    if (!canvas) return;
    
    const batteryText = new fabric.Text('85%', {
      left: 50,
      top: 50,
      fontSize: 16,
      fill: '#00ff00',
      fontFamily: 'Arial, sans-serif',
    });
    
    (batteryText as any).elementType = 'battery';
    (batteryText as any).dataSource = 'battery.level';
    (batteryText as any).dataFormat = '{value}%';
    canvas.add(batteryText);
    canvas.setActiveObject(batteryText);
  };

  const addStepsElement = () => {
    if (!canvas) return;
    
    const stepsText = new fabric.Text('8,547', {
      left: 350,
      top: 50,
      fontSize: 16,
      fill: '#ffaa00',
      fontFamily: 'Arial, sans-serif',
    });
    
    (stepsText as any).elementType = 'steps';
    (stepsText as any).dataSource = 'health.steps';
    (stepsText as any).dataFormat = '{value:,}';
    canvas.add(stepsText);
    canvas.setActiveObject(stepsText);
  };

  const addHeartRateElement = () => {
    if (!canvas) return;
    
    const heartRateText = new fabric.Text('72 bpm', {
      left: 200,
      top: 350,
      fontSize: 14,
      fill: '#ff6b6b',
      fontFamily: 'Arial, sans-serif',
    });
    
    (heartRateText as any).elementType = 'heartRate';
    (heartRateText as any).dataSource = 'health.heartRate';
    (heartRateText as any).dataFormat = '{value} bpm';
    canvas.add(heartRateText);
    canvas.setActiveObject(heartRateText);
  };

  const addWeatherElement = () => {
    if (!canvas) return;
    
    const weatherText = new fabric.Text('22°C', {
      left: 50,
      top: 350,
      fontSize: 16,
      fill: '#87ceeb',
      fontFamily: 'Arial, sans-serif',
    });
    
    (weatherText as any).elementType = 'weather';
    (weatherText as any).dataSource = 'weather.temperature';
    (weatherText as any).dataFormat = '{value}°C';
    canvas.add(weatherText);
    canvas.setActiveObject(weatherText);
  };

  const addCaloriesElement = () => {
    if (!canvas) return;
    
    const caloriesText = new fabric.Text('324 cal', {
      left: 350,
      top: 350,
      fontSize: 14,
      fill: '#ffa500',
      fontFamily: 'Arial, sans-serif',
    });
    
    (caloriesText as any).elementType = 'calories';
    (caloriesText as any).dataSource = 'health.calories';
    (caloriesText as any).dataFormat = '{value} cal';
    canvas.add(caloriesText);
    canvas.setActiveObject(caloriesText);
  };

  const addDistanceElement = () => {
    if (!canvas) return;
    
    const distanceText = new fabric.Text('2.4 km', {
      left: 300,
      top: 100,
      fontSize: 14,
      fill: '#98fb98',
      fontFamily: 'Arial, sans-serif',
    });
    
    (distanceText as any).elementType = 'distance';
    (distanceText as any).dataSource = 'health.distance';
    (distanceText as any).dataFormat = '{value} km';
    canvas.add(distanceText);
    canvas.setActiveObject(distanceText);
  };

  const addShapeElement = (shapeType: 'circle' | 'rectangle' | 'line' | 'arc') => {
    if (!canvas) return;
    
    let shape: fabric.Object;
    
    switch (shapeType) {
      case 'circle':
        shape = new fabric.Circle({
          left: 150,
          top: 150,
          radius: 30,
          fill: '#ff6b6b',
          stroke: '#ffffff',
          strokeWidth: 2,
        });
        break;
      case 'rectangle':
        shape = new fabric.Rect({
          left: 150,
          top: 150,
          width: 60,
          height: 40,
          fill: '#4ecdc4',
          stroke: '#ffffff',
          strokeWidth: 2,
        });
        break;
      case 'line':
        shape = new fabric.Line([150, 150, 250, 200], {
          stroke: '#ffffff',
          strokeWidth: 3,
        });
        break;
      case 'arc':
        // Create arc using Path
        shape = new fabric.Path('M 150 150 A 50 50 0 0 1 250 150', {
          stroke: '#ffffff',
          strokeWidth: 3,
          fill: '',
        });
        break;
      default:
        return;
    }
    
    (shape as any).elementType = 'shape';
    canvas.add(shape);
    canvas.setActiveObject(shape);
  };

  const addAnalogHands = () => {
    if (!canvas) return;
    
    // Hour hand
    const hourHand = new fabric.Line([225, 225, 225, 150], {
      stroke: '#ffffff',
      strokeWidth: 6,
      strokeLineCap: 'round',
    });
    (hourHand as any).elementType = 'analogHand';
    (hourHand as any).handType = 'hour';
    (hourHand as any).dataSource = 'time.hour';
    
    // Minute hand
    const minuteHand = new fabric.Line([225, 225, 225, 100], {
      stroke: '#ffffff',
      strokeWidth: 4,
      strokeLineCap: 'round',
    });
    (minuteHand as any).elementType = 'analogHand';
    (minuteHand as any).handType = 'minute';
    (minuteHand as any).dataSource = 'time.minute';
    
    // Second hand
    const secondHand = new fabric.Line([225, 225, 225, 80], {
      stroke: '#ff0000',
      strokeWidth: 2,
      strokeLineCap: 'round',
    });
    (secondHand as any).elementType = 'analogHand';
    (secondHand as any).handType = 'second';
    (secondHand as any).dataSource = 'time.second';
    
    // Center dot
    const centerDot = new fabric.Circle({
      left: 220,
      top: 220,
      radius: 5,
      fill: '#ffffff',
      selectable: false,
    });
    
    canvas.add(hourHand, minuteHand, secondHand, centerDot);
    canvas.setActiveObject(hourHand);
  };

  const addDigitalNumber = () => {
    if (!canvas) return;
    
    const digitalText = new fabric.Text('88:88', {
      left: 150,
      top: 300,
      fontSize: 32,
      fill: '#00ffff',
      fontFamily: 'Courier New, monospace',
      fontWeight: 'bold',
    });
    
    (digitalText as any).elementType = 'digitalNumber';
    (digitalText as any).dataSource = 'time.current';
    (digitalText as any).dataFormat = 'HH:mm';
    canvas.add(digitalText);
    canvas.setActiveObject(digitalText);
  };

  const addProgressBar = () => {
    if (!canvas) return;
    
    // Background bar
    const bgBar = new fabric.Rect({
      left: 50,
      top: 400,
      width: 350,
      height: 10,
      fill: '#333333',
      rx: 5,
      ry: 5,
    });
    
    // Progress bar
    const progressBar = new fabric.Rect({
      left: 50,
      top: 400,
      width: 175, // 50% progress
      height: 10,
      fill: '#4caf50',
      rx: 5,
      ry: 5,
    });
    
    (progressBar as any).elementType = 'progressBar';
    (progressBar as any).dataSource = 'battery.level';
    (progressBar as any).progressType = 'linear';
    (progressBar as any).minValue = 0;
    (progressBar as any).maxValue = 100;
    (progressBar as any).currentValue = 50;
    
    canvas.add(bgBar, progressBar);
    canvas.setActiveObject(progressBar);
  };

  const addPointer = () => {
    if (!canvas) return;
    
    const pointer = new fabric.Triangle({
      left: 200,
      top: 100,
      width: 20,
      height: 30,
      fill: '#ffff00',
      stroke: '#000000',
      strokeWidth: 1,
    });
    
    (pointer as any).elementType = 'pointer';
    (pointer as any).dataSource = 'battery.level';
    canvas.add(pointer);
    canvas.setActiveObject(pointer);
  };

  const addImageElement = () => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    
    input.onchange = (e) => {
      const file = (e.target as HTMLInputElement).files?.[0];
      if (!file || !canvas) return;
      
      const reader = new FileReader();
      reader.onload = (event) => {
        const imgUrl = event.target?.result as string;
        
        const img = new Image();
        img.onload = () => {
          const fabricImg = new fabric.Image(img, {
            left: 100,
            top: 100,
            scaleX: 0.5,
            scaleY: 0.5,
          });
          
          (fabricImg as any).elementType = 'image';
          canvas.add(fabricImg);
          canvas.setActiveObject(fabricImg);
        };
        img.src = imgUrl;
      };
      reader.readAsDataURL(file);
    };
    
    input.click();
  };

  // Action functions
  const deleteSelected = () => {
    if (!canvas || !selectedElement) return;
    
    canvas.remove(selectedElement);
    setSelectedElement(null);
  };

  const duplicateSelected = () => {
    if (!canvas || !selectedElement) return;
    
    // Serialize and deserialize for cloning
    const objData = selectedElement.toObject();
    
    if ((selectedElement as any).type === 'text') {
      const cloned = new fabric.Text((selectedElement as any).text, {
        ...objData,
        left: (selectedElement.left || 0) + 20,
        top: (selectedElement.top || 0) + 20,
      });
      
      // Copy custom properties
      (cloned as any).elementType = (selectedElement as any).elementType;
      (cloned as any).dataSource = (selectedElement as any).dataSource;
      (cloned as any).dataFormat = (selectedElement as any).dataFormat;
      (cloned as any).handType = (selectedElement as any).handType;
      
      canvas.add(cloned);
      canvas.setActiveObject(cloned);
    } else if ((selectedElement as any).type === 'circle') {
      const cloned = new fabric.Circle({
        ...objData,
        left: (selectedElement.left || 0) + 20,
        top: (selectedElement.top || 0) + 20,
      });
      
      (cloned as any).elementType = (selectedElement as any).elementType;
      canvas.add(cloned);
      canvas.setActiveObject(cloned);
    } else if ((selectedElement as any).type === 'rect') {
      const cloned = new fabric.Rect({
        ...objData,
        left: (selectedElement.left || 0) + 20,
        top: (selectedElement.top || 0) + 20,
      });
      
      (cloned as any).elementType = (selectedElement as any).elementType;
      canvas.add(cloned);
      canvas.setActiveObject(cloned);
    } else if ((selectedElement as any).type === 'line') {
      const cloned = new fabric.Line(
        [(selectedElement as any).x1, (selectedElement as any).y1, 
         (selectedElement as any).x2, (selectedElement as any).y2], {
        ...objData,
        left: (selectedElement.left || 0) + 20,
        top: (selectedElement.top || 0) + 20,
      });
      
      (cloned as any).elementType = (selectedElement as any).elementType;
      (cloned as any).handType = (selectedElement as any).handType;
      canvas.add(cloned);
      canvas.setActiveObject(cloned);
    }
  };

  const undo = () => {
    if (historyIndex > 0 && canvas) {
      setHistoryIndex(historyIndex - 1);
      const prevState = history[historyIndex - 1];
      canvas.loadFromJSON(prevState, () => {
        canvas.renderAll();
        updateWatchData(canvas);
      });
    }
  };

  const redo = () => {
    if (historyIndex < history.length - 1 && canvas) {
      setHistoryIndex(historyIndex + 1);
      const nextState = history[historyIndex + 1];
      canvas.loadFromJSON(nextState, () => {
        canvas.renderAll();
        updateWatchData(canvas);
      });
    }
  };

  const exportWatchFace = (): WatchFaceData => {
    if (canvas) {
      updateWatchData(canvas);
    }
    return watchData;
  };

  const clearCanvas = () => {
    if (!canvas) return;
    
    canvas.clear();
    
    // Re-add the watch border
    const watchBorder = new fabric.Circle({
      left: 0,
      top: 0,
      radius: 225,
      fill: 'transparent',
      stroke: '#333',
      strokeWidth: 2,
      selectable: false,
      evented: false,
    });
    canvas.add(watchBorder);
    
    setWatchData(prev => ({
      ...prev,
      elements: []
    }));
  };

  return (
    <div className="watch-face-designer">
      <div className="designer-header">
        <h1>Watch Face Designer</h1>
        <div className="header-actions">
          <button onClick={clearCanvas} className="btn btn-secondary">
            Clear Canvas
          </button>
          <ExportPanel 
            watchData={watchData} 
            canvas={canvas}
            onExport={exportWatchFace}
          />
        </div>
      </div>
      
      <div className="designer-body">
        <Toolbar
          onAddTime={addTimeElement}
          onAddDate={addDateElement}
          onAddBattery={addBatteryElement}
          onAddSteps={addStepsElement}
          onAddHeartRate={addHeartRateElement}
          onAddWeather={addWeatherElement}
          onAddCalories={addCaloriesElement}
          onAddDistance={addDistanceElement}
          onAddShape={addShapeElement}
          onAddImage={addImageElement}
          onAddAnalogHands={addAnalogHands}
          onAddDigitalNumber={addDigitalNumber}
          onAddProgressBar={addProgressBar}
          onAddPointer={addPointer}
          onDelete={deleteSelected}
          onDuplicate={duplicateSelected}
          onUndo={undo}
          onRedo={redo}
          hasSelection={!!selectedElement}
          canUndo={historyIndex > 0}
          canRedo={historyIndex < history.length - 1}
          currentTool={currentTool}
          onToolChange={setCurrentTool}
        />
        
        <div className="canvas-container">
          <div className="canvas-wrapper">
            <canvas ref={canvasRef} />
          </div>
        </div>
        
        <PropertiesPanel
          selectedElement={selectedElement}
          canvas={canvas}
          onUpdate={() => canvas && updateWatchData(canvas)}
        />
      </div>
    </div>
  );
};