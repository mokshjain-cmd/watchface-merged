import React from 'react';
import { observer } from 'mobx-react-lite';
import { useStores } from '../../hooks/useStores';
import './DeviceSelector.scss';

interface Device {
  id: string;
  name: string;
  screenSize: { width: number; height: number };
  pixelDensity: number;
  description?: string;
}

const SUPPORTED_DEVICES: Device[] = [
  {
    id: 'generic-round-360',
    name: 'Generic Round (360px)',
    screenSize: { width: 360, height: 360 },
    pixelDensity: 1,
    description: 'Standard round smartwatch'
  },
  {
    id: 'generic-round-454',
    name: 'Generic Round (454px)',
    screenSize: { width: 454, height: 454 },
    pixelDensity: 1,
    description: 'Large round smartwatch'
  },
  {
    id: 'honor-watch-4',
    name: 'Honor Watch 4',
    screenSize: { width: 466, height: 466 },
    pixelDensity: 1.5,
    description: 'Honor Watch 4 series'
  },
  {
    id: 'apple-watch-series-7',
    name: 'Apple Watch Series 7',
    screenSize: { width: 396, height: 484 },
    pixelDensity: 2,
    description: 'Apple Watch 45mm'
  },
  {
    id: 'samsung-galaxy-watch-4',
    name: 'Samsung Galaxy Watch 4',
    screenSize: { width: 450, height: 450 },
    pixelDensity: 1.5,
    description: 'Samsung Galaxy Watch 4 44mm'
  }
];

export const DeviceSelector: React.FC = observer(() => {
  const { canvasStore } = useStores();
  const [isOpen, setIsOpen] = React.useState(false);

  const currentDevice = SUPPORTED_DEVICES.find(
    device => device.screenSize.width === canvasStore.canvasSize.width &&
              device.screenSize.height === canvasStore.canvasSize.height
  ) || SUPPORTED_DEVICES[0];

  const handleDeviceSelect = (device: Device) => {
    canvasStore.setCanvasSize(device.screenSize.width, device.screenSize.height);
    setIsOpen(false);
  };

  return (
    <div className="device-selector">
      <button 
        className="device-selector-trigger"
        onClick={() => setIsOpen(!isOpen)}
        title="Select target device"
      >
        <i className="icon-device" />
        <span className="device-name">{currentDevice.name}</span>
        <span className="device-resolution">
          {currentDevice.screenSize.width}×{currentDevice.screenSize.height}
        </span>
        <i className={`icon-chevron ${isOpen ? 'up' : 'down'}`} />
      </button>

      {isOpen && (
        <>
          <div className="device-selector-overlay" onClick={() => setIsOpen(false)} />
          <div className="device-selector-dropdown">
            <div className="dropdown-header">
              <h4>Select Target Device</h4>
            </div>
            <div className="device-list">
              {SUPPORTED_DEVICES.map(device => (
                <button
                  key={device.id}
                  className={`device-item ${device.id === currentDevice.id ? 'active' : ''}`}
                  onClick={() => handleDeviceSelect(device)}
                >
                  <div className="device-info">
                    <div className="device-name">{device.name}</div>
                    <div className="device-details">
                      <span className="resolution">
                        {device.screenSize.width}×{device.screenSize.height}
                      </span>
                      {device.pixelDensity !== 1 && (
                        <span className="density">@{device.pixelDensity}x</span>
                      )}
                    </div>
                    {device.description && (
                      <div className="device-description">{device.description}</div>
                    )}
                  </div>
                  {device.id === currentDevice.id && (
                    <i className="icon-check" />
                  )}
                </button>
              ))}
            </div>
          </div>
        </>
      )}
    </div>
  );
});

DeviceSelector.displayName = 'DeviceSelector';