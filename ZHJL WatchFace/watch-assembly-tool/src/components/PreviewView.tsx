import React, { useState, useEffect } from 'react';
import { Play, Pause, RotateCcw } from 'lucide-react';
import { WatchFaceProject } from '../App';
import './PreviewView.css';

interface PreviewViewProps {
  project: WatchFaceProject;
}

interface SimulationData {
  time: {
    hour: number;
    minute: number;
    second: number;
    day: number;
    month: number;
    year: number;
    weekDay: number;
  };
  battery: number;
  bloodOxygen: number;
  steps: number;
  heartRate: number;
  calories: number;
  showNoData: boolean; // Toggle to simulate "no data" state
}

export const PreviewView: React.FC<PreviewViewProps> = ({ project }) => {
  const [isPlaying, setIsPlaying] = useState(false);
  const [simulationData, setSimulationData] = useState<SimulationData>({
    time: {
      hour: 12,
      minute: 34,
      second: 56,
      day: 6,
      month: 11,
      year: 2025,
      weekDay: 4 // Thursday
    },
    battery: 85,
    bloodOxygen: 98,
    steps: 8547,
    heartRate: 72,
    calories: 324,
    showNoData: false
  });

  // Auto-update time when playing
  useEffect(() => {
    if (!isPlaying) return;

    const interval = setInterval(() => {
      setSimulationData(prev => {
        let newSecond = prev.time.second + 1;
        let newMinute = prev.time.minute;
        let newHour = prev.time.hour;

        if (newSecond >= 60) {
          newSecond = 0;
          newMinute += 1;
        }
        if (newMinute >= 60) {
          newMinute = 0;
          newHour += 1;
        }
        if (newHour >= 24) {
          newHour = 0;
        }

        return {
          ...prev,
          time: {
            ...prev.time,
            hour: newHour,
            minute: newMinute,
            second: newSecond
          }
        };
      });
    }, 1000);

    return () => clearInterval(interval);
  }, [isPlaying]);

  const handleDataChange = (category: string, field: string, value: number) => {
    if (category === 'time') {
      setSimulationData(prev => ({
        ...prev,
        time: {
          ...prev.time,
          [field]: value
        }
      }));
    } else {
      setSimulationData(prev => ({
        ...prev,
        [field]: value
      }));
    }
  };

  const getImageForValue = (component: any, value: number): string | null => {
    if (!component.images || component.images.length === 0) return null;

    if (component.type === 'dateTime') {
      // For time components, show the appropriate digit
      if (component.config.dateTimeFormat === 'Hour') {
        const hourStr = component.config.leadingZero ? value.toString().padStart(2, '0') : value.toString();
        if (component.config.digitPosition === 'tens') {
          if (!component.config.leadingZero && value < 10) {
            // For no leading zero and single digit, hide tens digit
            return null;
          }
          const tensDigit = parseInt(hourStr.charAt(0)) || 0;
          return component.images[tensDigit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const onesDigit = component.config.leadingZero ? 
            parseInt(hourStr.charAt(1)) || 0 : 
            parseInt(hourStr.charAt(hourStr.length - 1)) || 0;
          return component.images[onesDigit] || component.images[0];
        }
      } else if (component.config.dateTimeFormat === 'Minute') {
        const minuteStr = component.config.leadingZero ? value.toString().padStart(2, '0') : value.toString();
        if (component.config.digitPosition === 'tens') {
          if (!component.config.leadingZero && value < 10) {
            // For no leading zero and single digit, hide tens digit
            return null;
          }
          const tensDigit = parseInt(minuteStr.charAt(0)) || 0;
          return component.images[tensDigit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const onesDigit = component.config.leadingZero ? 
            parseInt(minuteStr.charAt(1)) || 0 : 
            parseInt(minuteStr.charAt(minuteStr.length - 1)) || 0;
          return component.images[onesDigit] || component.images[0];
        }
      } else if (component.config.dateTimeFormat === 'Second') {
        const secondStr = component.config.leadingZero ? value.toString().padStart(2, '0') : value.toString();
        if (component.config.digitPosition === 'tens') {
          if (!component.config.leadingZero && value < 10) {
            // For no leading zero and single digit, hide tens digit
            return null;
          }
          const tensDigit = parseInt(secondStr.charAt(0)) || 0;
          return component.images[tensDigit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const onesDigit = component.config.leadingZero ? 
            parseInt(secondStr.charAt(1)) || 0 : 
            parseInt(secondStr.charAt(secondStr.length - 1)) || 0;
          return component.images[onesDigit] || component.images[0];
        }
      } else if (component.config.dateTimeFormat === 'Day') {
        const dayStr = value.toString().padStart(2, '0');
        if (component.config.digitPosition === 'tens') {
          const tensDigit = parseInt(dayStr.charAt(0)) || 0;
          return component.images[tensDigit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const onesDigit = parseInt(dayStr.charAt(1)) || 0;
          return component.images[onesDigit] || component.images[0];
        }
      } else if (component.config.dateTimeFormat === 'Month') {
        const monthStr = value.toString().padStart(2, '0');
        if (component.config.digitPosition === 'tens') {
          const tensDigit = parseInt(monthStr.charAt(0)) || 0;
          return component.images[tensDigit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const onesDigit = parseInt(monthStr.charAt(1)) || 0;
          return component.images[onesDigit] || component.images[0];
        }
      } else if (component.config.dateTimeFormat === 'Year') {
        const yearStr = value.toString().padStart(4, '0');
        if (component.config.digitPosition === 'thousands') {
          const digit = parseInt(yearStr.charAt(0)) || 0;
          return component.images[digit] || component.images[0];
        } else if (component.config.digitPosition === 'hundreds') {
          const digit = parseInt(yearStr.charAt(1)) || 0;
          return component.images[digit] || component.images[0];
        } else if (component.config.digitPosition === 'tens') {
          const digit = parseInt(yearStr.charAt(2)) || 0;
          return component.images[digit] || component.images[0];
        } else if (component.config.digitPosition === 'ones') {
          const digit = parseInt(yearStr.charAt(3)) || 0;
          return component.images[digit] || component.images[0];
        }
      }
      // Fallback for single digit display
      const digit = value % 10;
      return component.images[digit] || component.images[0];
    } else if (component.type === 'progress') {
      // For progress bars, map value to image index
      const percentage = Math.max(0, Math.min(100, (value / (component.config.maxValue || 100)) * 100));
      const imageIndex = Math.floor((percentage / 100) * (component.images.length - 1));
      return component.images[imageIndex] || component.images[0];
    }

    return component.images[0] || null;
  };

  const renderMultiDigitComponent = (component: any, value: number) => {
    if (component.config.digitPosition !== 'multi') return null;
    
    // Check if this component supports "no data" and if no data mode is enabled
    const supportsNoData = ['Steps', 'Heart Rate', 'Blood Oxygen', 'Calories'].includes(component.config.itemName || component.name);
    const shouldShowNoData = supportsNoData && simulationData.showNoData;
    
    // If showing no data and component has 11 images, show the 11th image (index 10)
    if (shouldShowNoData && component.images.length === 11) {
      return (
        <img
          src={component.images[10]}
          alt={`${component.name} no data`}
          style={{
            width: '100%',
            height: '100%',
            objectFit: 'contain'
          }}
        />
      );
    }
    
    let valueStr = '';
    let digitCount = 2;
    
    if (component.config.dateTimeFormat === 'Year') {
      valueStr = value.toString().padStart(4, '0'); // Year always has leading zeros
      digitCount = 4;
    } else {
      // Respect leadingZero setting for other components
      valueStr = component.config.leadingZero ? 
        value.toString().padStart(2, '0') : 
        value.toString();
      digitCount = valueStr.length; // Adjust digit count based on actual string length
    }
    
    const digitWidth = component.size.width / Math.max(digitCount, 2); // Ensure minimum width
    
    return (
      <div style={{ position: 'relative', width: '100%', height: '100%' }}>
        {valueStr.split('').map((digit, index) => {
          const digitIndex = parseInt(digit);
          const imageToShow = component.images[digitIndex] || component.images[0];
          
          return imageToShow ? (
            <img
              key={index}
              src={imageToShow}
              alt={`${component.name} digit ${digit}`}
              style={{
                position: 'absolute',
                left: index * digitWidth,
                top: 0,
                width: digitWidth,
                height: '100%',
                objectFit: 'contain'
              }}
            />
          ) : null;
        })}
      </div>
    );
  };

  const getCurrentValue = (component: any): number => {
    switch (component.elementType) {
      case 0: // Time
        if (component.config.dateTimeFormat === 'Hour') return simulationData.time.hour;
        if (component.config.dateTimeFormat === 'Minute') return simulationData.time.minute;
        if (component.config.dateTimeFormat === 'Second') return simulationData.time.second;
        if (component.config.dateTimeFormat === 'Day') return simulationData.time.day;
        if (component.config.dateTimeFormat === 'Month') return simulationData.time.month;
        if (component.config.dateTimeFormat === 'Year') return simulationData.time.year;
        if (component.config.dateTimeFormat === 'WeekDay') return simulationData.time.weekDay;
        return 0;
      case 1: return simulationData.bloodOxygen; // Blood Oxygen
      case 2: return simulationData.battery; // Battery
      case 3: return simulationData.steps; // Steps
      case 4: return simulationData.heartRate; // Heart Rate
      case 5: return simulationData.calories; // Calories
      default: return component.config.defaultValue || 0;
    }
  };

  const resetToCurrentTime = () => {
    const now = new Date();
    setSimulationData(prev => ({
      ...prev,
      time: {
        hour: now.getHours(),
        minute: now.getMinutes(),
        second: now.getSeconds(),
        day: now.getDate(),
        month: now.getMonth() + 1,
        year: now.getFullYear(),
        weekDay: now.getDay()
      }
    }));
  };

  return (
    <div className="preview-view view-container">
      <div className="sidebar">
        <div className="sidebar-header">
          <h3>Simulation Controls</h3>
          <div className="playback-controls">
            <button 
              className={`control-btn ${isPlaying ? 'active' : ''}`}
              onClick={() => setIsPlaying(!isPlaying)}
            >
              {isPlaying ? <Pause size={16} /> : <Play size={16} />}
            </button>
            <button className="control-btn" onClick={resetToCurrentTime}>
              <RotateCcw size={16} />
            </button>
          </div>
        </div>

        <div className="simulation-controls">
          {/* Time Controls */}
          <div className="control-group">
            <h4>Time</h4>
            
            <div className="control-row">
              <label>Hour:</label>
              <input
                type="range"
                min="0"
                max="23"
                value={simulationData.time.hour}
                onChange={(e) => handleDataChange('time', 'hour', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.time.hour}</span>
            </div>

            <div className="control-row">
              <label>Minute:</label>
              <input
                type="range"
                min="0"
                max="59"
                value={simulationData.time.minute}
                onChange={(e) => handleDataChange('time', 'minute', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.time.minute}</span>
            </div>

            <div className="control-row">
              <label>Second:</label>
              <input
                type="range"
                min="0"
                max="59"
                value={simulationData.time.second}
                onChange={(e) => handleDataChange('time', 'second', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.time.second}</span>
            </div>

            <div className="control-row">
              <label>Day:</label>
              <input
                type="range"
                min="1"
                max="31"
                value={simulationData.time.day}
                onChange={(e) => handleDataChange('time', 'day', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.time.day}</span>
            </div>

            <div className="control-row">
              <label>Month:</label>
              <input
                type="range"
                min="1"
                max="12"
                value={simulationData.time.month}
                onChange={(e) => handleDataChange('time', 'month', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.time.month}</span>
            </div>
          </div>

          {/* Health & Battery Controls */}
          <div className="control-group">
            <h4>Health & Battery</h4>
            
            <div className="control-row">
              <label>Battery:</label>
              <input
                type="range"
                min="0"
                max="100"
                value={simulationData.battery}
                onChange={(e) => handleDataChange('', 'battery', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.battery}%</span>
            </div>

            <div className="control-row">
              <label>Blood O2:</label>
              <input
                type="range"
                min="90"
                max="100"
                value={simulationData.bloodOxygen}
                onChange={(e) => handleDataChange('', 'bloodOxygen', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.bloodOxygen}%</span>
            </div>

            <div className="control-row">
              <label>Steps:</label>
              <input
                type="range"
                min="0"
                max="20000"
                value={simulationData.steps}
                onChange={(e) => handleDataChange('', 'steps', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.steps.toLocaleString()}</span>
            </div>

            <div className="control-row">
              <label>Heart Rate:</label>
              <input
                type="range"
                min="60"
                max="120"
                value={simulationData.heartRate}
                onChange={(e) => handleDataChange('', 'heartRate', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.heartRate} bpm</span>
            </div>

            <div className="control-row">
              <label>Calories:</label>
              <input
                type="range"
                min="0"
                max="5000"
                value={simulationData.calories}
                onChange={(e) => handleDataChange('', 'calories', parseInt(e.target.value))}
                className="slider"
              />
              <span>{simulationData.calories.toLocaleString()}</span>
            </div>

            <div className="control-section">
              <h4>Data State</h4>
              <div className="control-row">
                <label>Show "No Data":</label>
                <input
                  type="checkbox"
                  checked={simulationData.showNoData}
                  onChange={(e) => handleDataChange('', 'showNoData', e.target.checked ? 1 : 0)}
                  className="control-checkbox"
                />
              </div>
              <p className="help-text">
                Toggle to preview the "no data" image for Steps, Heart Rate, Blood Oxygen, and Calories components
              </p>
            </div>
          </div>
        </div>
      </div>

      <div className="main-area">
        <div className="preview-header">
          <h3>Live Preview</h3>
          <div className="preview-info">
            <span>Components: {project.components.length}</span>
            <span>
              Time: {simulationData.time.hour.toString().padStart(2, '0')}:
              {simulationData.time.minute.toString().padStart(2, '0')}:
              {simulationData.time.second.toString().padStart(2, '0')}
            </span>
          </div>
        </div>

        <div className="canvas-area">
          <div className="watch-canvas">
            {project.components.map(component => {
              const currentValue = getCurrentValue(component);
              
              // Handle analog clock hands
              if (component.type === 'analog') {
                // Calculate rotation angle based on hand type
                // Note: Hand images are designed to point upward at 0°, matching the watch's coordinate system
                let angle = 0;
                const valueIndex = component.config.valueIndex || 0;
                
                if (valueIndex === 0) {
                  // Hour hand: 12-hour format, includes minute contribution
                  const hours = simulationData.time.hour % 12;
                  const minutes = simulationData.time.minute;
                  angle = (hours * 30) + (minutes * 0.5); // 30° per hour + 0.5° per minute
                } else if (valueIndex === 1) {
                  // Minute hand
                  angle = simulationData.time.minute * 6; // 6° per minute
                } else if (valueIndex === 2) {
                  // Second hand
                  angle = simulationData.time.second * 6; // 6° per second
                }
                
                const originX = component.config.originPointX || 13;
                const originY = component.config.originPointY || (project.height / 2);
                
                return (
                  <div
                    key={component.id}
                    className="preview-component analog-hand"
                    style={{
                      position: 'absolute',
                      left: component.position.x,
                      top: component.position.y,
                      width: component.size.width,
                      height: component.size.height,
                    }}
                  >
                    {component.images[0] ? (
                      <img 
                        src={component.images[0]} 
                        alt={component.name}
                        style={{ 
                          width: '100%', 
                          height: '100%', 
                          objectFit: 'contain',
                          transformOrigin: `${originX}px ${originY}px`,
                          transform: `rotate(${angle}deg)`
                        }}
                      />
                    ) : (
                      <div className="placeholder-component">
                        <span>{component.name}</span>
                      </div>
                    )}
                  </div>
                );
              }
              
              return (
                <div
                  key={component.id}
                  className="preview-component"
                  style={{
                    position: 'absolute',
                    left: component.position.x,
                    top: component.position.y,
                    width: component.size.width,
                    height: component.size.height,
                  }}
                >
                  {component.config.digitPosition === 'multi' ? (
                    renderMultiDigitComponent(component, currentValue)
                  ) : (
                    (() => {
                      const imageToShow = getImageForValue(component, currentValue);
                      return imageToShow ? (
                        <img 
                          src={imageToShow} 
                          alt={component.name}
                          style={{ 
                            width: '100%', 
                            height: '100%', 
                            objectFit: 'contain'
                          }}
                        />
                      ) : (
                        <div className="placeholder-component">
                          <span>{component.name}</span>
                        </div>
                      );
                    })()
                  )}
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
};