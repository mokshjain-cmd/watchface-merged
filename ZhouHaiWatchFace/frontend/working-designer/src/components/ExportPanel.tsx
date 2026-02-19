import React, { useState } from 'react';
import { Download, FileText, Image as ImageIcon, Folder, Package, Send } from 'lucide-react';
import { WatchFaceData } from './WatchFaceDesigner';
import './ExportPanel.css';

interface ExportPanelProps {
  watchData: WatchFaceData;
  canvas: any;
  onExport: () => WatchFaceData;
}

export const ExportPanel: React.FC<ExportPanelProps> = ({ 
  watchData, 
  canvas, 
  onExport 
}) => {
  const [isExporting, setIsExporting] = useState(false);
  const [exportStatus, setExportStatus] = useState('');
  const [showExportDialog, setShowExportDialog] = useState(false);

  const downloadJSON = () => {
    setIsExporting(true);
    setExportStatus('Generating JSON configuration...');

    try {
      const data = onExport();
      
      // Create comprehensive JSON matching original app format
      const exportData = {
        ...data,
        exportedAt: new Date().toISOString(),
        format: 'watch-face-config',
        version: '1.0',
        metadata: {
          totalElements: data.elements.length,
          elementTypes: [...new Set(data.elements.map(el => el.type))],
          canvasSize: {
            width: data.width,
            height: data.height
          },
          hasAnalogElements: data.elements.some(el => el.type === 'analogHand'),
          hasDigitalElements: data.elements.some(el => el.type === 'time' || el.type === 'date'),
          hasHealthData: data.elements.some(el => ['battery', 'steps', 'heartRate', 'calories'].includes(el.type)),
        },
        // Add binary generation hints for backend
        binaryHints: {
          requiresImageProcessing: data.elements.some(el => el.type === 'image'),
          requiresAnalogCalculation: data.elements.some(el => el.type === 'analogHand'),
          requiresDataBinding: data.elements.some(el => el.properties?.dataSource),
        }
      };

      const jsonString = JSON.stringify(exportData, null, 2);
      const blob = new Blob([jsonString], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = `${data.name.replace(/[^a-zA-Z0-9]/g, '_')}_config.json`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);

      setExportStatus('JSON configuration exported successfully!');
    } catch (error) {
      console.error('Export failed:', error);
      setExportStatus('Export failed. Please try again.');
    } finally {
      setIsExporting(false);
      setTimeout(() => setExportStatus(''), 3000);
    }
  };

  const downloadPreview = () => {
    if (!canvas) return;

    setIsExporting(true);
    setExportStatus('Generating high-resolution preview...');

    try {
      const dataURL = canvas.toDataURL({
        format: 'png',
        quality: 1,
        multiplier: 3 // High resolution for preview
      });

      const link = document.createElement('a');
      link.href = dataURL;
      link.download = `${watchData.name.replace(/[^a-zA-Z0-9]/g, '_')}_preview.png`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      setExportStatus('Preview image exported successfully!');
    } catch (error) {
      console.error('Preview export failed:', error);
      setExportStatus('Preview export failed. Please try again.');
    } finally {
      setIsExporting(false);
      setTimeout(() => setExportStatus(''), 3000);
    }
  };

  const downloadProjectPackage = () => {
    setIsExporting(true);
    setExportStatus('Packaging project files...');

    try {
      const data = onExport();
      
      // Create a comprehensive project package
      const projectPackage = {
        config: data,
        assets: {
          // This would include all image assets in a real implementation
          images: data.elements.filter(el => el.type === 'image').map(el => ({
            id: el.id,
            path: `assets/images/${el.id}.png`,
            properties: el.properties
          }))
        },
        manifest: {
          name: data.name,
          version: data.version,
          description: `Watch face created on ${new Date().toLocaleDateString()}`,
          compatibility: ['HonorWatch', 'GeneralSmartwatch'],
          requirements: {
            minWatchOS: '1.0',
            supportedResolutions: ['450x450', '360x360', '320x320']
          }
        },
        buildInstructions: {
          steps: [
            'Process JSON configuration',
            'Generate binary watch face file',
            'Package with assets',
            'Create installation package'
          ],
          outputFormat: '.bin',
          targetDevice: 'smartwatch'
        }
      };

      const packageString = JSON.stringify(projectPackage, null, 2);
      const blob = new Blob([packageString], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = `${data.name.replace(/[^a-zA-Z0-9]/g, '_')}_project.json`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);

      setExportStatus('Project package exported successfully!');
    } catch (error) {
      console.error('Package export failed:', error);
      setExportStatus('Package export failed. Please try again.');
    } finally {
      setIsExporting(false);
      setTimeout(() => setExportStatus(''), 3000);
    }
  };

  const sendToBackend = async () => {
    setIsExporting(true);
    setExportStatus('Sending to backend for binary processing...');

    try {
      const data = onExport();
      
      // Enhanced data for backend processing
      const backendPayload = {
        ...data,
        processingOptions: {
          generateBinary: true,
          includeAssets: true,
          compressionLevel: 'high',
          targetFormat: 'bin',
          optimization: 'size'
        },
        clientInfo: {
          userAgent: navigator.userAgent,
          timestamp: new Date().toISOString(),
          version: '1.0.0'
        }
      };
      
      // This would typically send to your backend API
      const response = await fetch('/api/watchface/process', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(backendPayload),
      });

      if (response.ok) {
        const result = await response.json();
        setExportStatus('Successfully sent to backend! Binary generation in progress...');
        console.log('Backend response:', result);
        
        // If backend returns binary file, download it
        if (result.binaryUrl) {
          const link = document.createElement('a');
          link.href = result.binaryUrl;
          link.download = `${data.name.replace(/[^a-zA-Z0-9]/g, '_')}.bin`;
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        }
      } else {
        throw new Error('Backend request failed');
      }
    } catch (error) {
      console.error('Backend send failed:', error);
      setExportStatus('Backend unavailable. Downloading JSON config instead...');
      // Fallback to local JSON download
      setTimeout(() => downloadJSON(), 1000);
    } finally {
      setIsExporting(false);
      setTimeout(() => setExportStatus(''), 5000);
    }
  };

  const exportAssets = () => {
    setIsExporting(true);
    setExportStatus('Exporting all assets...');

    try {
      const data = onExport();
      
      // Export individual elements as separate files
      const assetExports = [];
      
      if (canvas) {
        data.elements.forEach((element, index) => {
          // Create individual element exports
          const elementData = {
            element,
            exportTime: new Date().toISOString(),
            index
          };
          assetExports.push(elementData);
        });
      }

      const assetsPackage = {
        watchFace: data,
        assets: assetExports,
        exportInfo: {
          totalAssets: assetExports.length,
          exportedAt: new Date().toISOString(),
          format: 'asset-package'
        }
      };

      const assetsString = JSON.stringify(assetsPackage, null, 2);
      const blob = new Blob([assetsString], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = `${data.name.replace(/[^a-zA-Z0-9]/g, '_')}_assets.json`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);

      setExportStatus('Assets exported successfully!');
    } catch (error) {
      console.error('Assets export failed:', error);
      setExportStatus('Assets export failed. Please try again.');
    } finally {
      setIsExporting(false);
      setTimeout(() => setExportStatus(''), 3000);
    }
  };

  return (
    <div className="export-panel">
      <button 
        onClick={() => setShowExportDialog(!showExportDialog)}
        className="btn btn-primary"
        disabled={isExporting}
      >
        <Download size={16} />
        Export Watch Face
      </button>

      {showExportDialog && (
        <div className="export-dialog">
          <div className="export-dialog-content">
            <div className="export-header">
              <h3>Export Watch Face</h3>
              <button 
                onClick={() => setShowExportDialog(false)}
                className="close-btn"
              >
                ×
              </button>
            </div>

            <div className="export-options">
              <div className="export-option-grid">
                <button 
                  onClick={downloadJSON}
                  disabled={isExporting}
                  className="export-option-btn"
                  title="Download JSON configuration file"
                >
                  <FileText size={24} />
                  <div className="option-content">
                    <div className="option-title">JSON Config</div>
                    <div className="option-desc">Watch face configuration</div>
                  </div>
                </button>

                <button 
                  onClick={downloadPreview}
                  disabled={isExporting || !canvas}
                  className="export-option-btn"
                  title="Download high-resolution preview image"
                >
                  <ImageIcon size={24} />
                  <div className="option-content">
                    <div className="option-title">Preview Image</div>
                    <div className="option-desc">High-resolution PNG</div>
                  </div>
                </button>

                <button 
                  onClick={downloadProjectPackage}
                  disabled={isExporting}
                  className="export-option-btn"
                  title="Download complete project package"
                >
                  <Package size={24} />
                  <div className="option-content">
                    <div className="option-title">Project Package</div>
                    <div className="option-desc">Complete project files</div>
                  </div>
                </button>

                <button 
                  onClick={exportAssets}
                  disabled={isExporting}
                  className="export-option-btn"
                  title="Export all assets separately"
                >
                  <Folder size={24} />
                  <div className="option-content">
                    <div className="option-title">Assets Package</div>
                    <div className="option-desc">Individual assets</div>
                  </div>
                </button>

                <button 
                  onClick={sendToBackend}
                  disabled={isExporting}
                  className="export-option-btn primary"
                  title="Send to backend for binary generation"
                >
                  <Send size={24} />
                  <div className="option-content">
                    <div className="option-title">Generate Binary</div>
                    <div className="option-desc">Send to backend</div>
                  </div>
                </button>
              </div>
            </div>

            {exportStatus && (
              <div className="export-status">
                {exportStatus}
              </div>
            )}

            <div className="watch-info">
              <div className="info-grid">
                <div className="info-item">
                  <span className="label">Elements:</span>
                  <span className="value">{watchData.elements.length}</span>
                </div>
                <div className="info-item">
                  <span className="label">Size:</span>
                  <span className="value">{watchData.width}×{watchData.height}</span>
                </div>
                <div className="info-item">
                  <span className="label">Has Analog:</span>
                  <span className="value">
                    {watchData.elements.some(el => el.type === 'analogHand') ? 'Yes' : 'No'}
                  </span>
                </div>
                <div className="info-item">
                  <span className="label">Has Health Data:</span>
                  <span className="value">
                    {watchData.elements.some(el => ['battery', 'steps', 'heartRate'].includes(el.type)) ? 'Yes' : 'No'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};