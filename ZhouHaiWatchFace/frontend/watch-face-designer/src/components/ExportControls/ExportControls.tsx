import React from 'react';
import { useStores } from '../../hooks/useStores';
import { ExportService } from '../../services/ExportService';
import './ExportControls.scss';

interface ExportControlsProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ExportControls: React.FC<ExportControlsProps> = ({ isOpen, onClose }) => {
  const { canvasStore, appStore } = useStores();
  
  if (!isOpen) return null;

  const handleExport = async (format: string) => {
    try {
      const exportService = new ExportService(canvasStore, appStore);
      
      switch (format) {
        case 'json':
          const jsonData = exportService.exportToJSON();
          // Download JSON file
          const jsonBlob = new Blob([JSON.stringify(jsonData, null, 2)], { type: 'application/json' });
          const jsonUrl = URL.createObjectURL(jsonBlob);
          const jsonLink = document.createElement('a');
          jsonLink.href = jsonUrl;
          jsonLink.download = 'watch-face-config.json';
          document.body.appendChild(jsonLink);
          jsonLink.click();
          document.body.removeChild(jsonLink);
          URL.revokeObjectURL(jsonUrl);
          break;
          
        case 'bin':
          console.log('Binary export would require server-side processing...');
          const configData = exportService.exportToJSON();
          console.log('Watch face configuration:', configData);
          // In the real app, this would send the JSON to the backend for binary conversion
          break;
          
        case 'png':
          // Export canvas as PNG
          if (canvasStore.fabricCanvas) {
            const dataURL = canvasStore.fabricCanvas.toDataURL({
              format: 'png',
              quality: 1,
              multiplier: 2 // Higher resolution
            });
            const link = document.createElement('a');
            link.href = dataURL;
            link.download = 'watch-face-preview.png';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
          }
          break;
      }
      
      onClose();
    } catch (error) {
      console.error('Export failed:', error);
      alert('Export failed. Please check the console for details.');
    }
  };

  return (
    <div className="export-controls-overlay">
      <div className="export-controls">
        <div className="modal-header">
          <h3>Export Watch Face</h3>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>
        
        <div className="modal-content">
          <div className="export-section">
            <h4>Export Formats</h4>
            <div className="export-options">
              <button 
                className="export-option"
                onClick={() => handleExport('bin')}
              >
                <i className="icon-file-binary" />
                <div className="option-info">
                  <div className="option-title">Watch Face Binary (.bin)</div>
                  <div className="option-desc">Native watch face format</div>
                </div>
              </button>
              
              <button 
                className="export-option"
                onClick={() => handleExport('png')}
              >
                <i className="icon-image" />
                <div className="option-info">
                  <div className="option-title">Preview Image (.png)</div>
                  <div className="option-desc">High-resolution preview</div>
                </div>
              </button>
              
              <button 
                className="export-option"
                onClick={() => handleExport('json')}
              >
                <i className="icon-code" />
                <div className="option-info">
                  <div className="option-title">Configuration (.json)</div>
                  <div className="option-desc">Watch face configuration data</div>
                </div>
              </button>
            </div>
          </div>

          <div className="export-section">
            <h4>Export Settings</h4>
            <div className="settings-grid">
              <label>
                <input type="checkbox" defaultChecked />
                Include assets
              </label>
              <label>
                <input type="checkbox" defaultChecked />
                Optimize for size
              </label>
              <label>
                <input type="checkbox" />
                Generate preview images
              </label>
              <label>
                <input type="checkbox" />
                Include source files
              </label>
            </div>
          </div>
        </div>
        
        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

ExportControls.displayName = 'ExportControls';