import React from 'react';
import { observer } from 'mobx-react-lite';
import { useStores } from '../../hooks/useStores';
import { CanvasComponent } from '../Canvas/CanvasComponent';
import { Toolbar } from '../Toolbar';
import { PropertyPanel } from '../PropertyPanel/PropertyPanel';
import { DeviceSelector } from '../DeviceSelector/DeviceSelector';
import { WatchStyleManager } from '../WatchStyleManager/WatchStyleManager';
import { ExportControls } from '../ExportControls/ExportControls';
import { AssetLibrary } from '../AssetLibrary/AssetLibrary';
import './Layout.scss';

interface LayoutProps {
  className?: string;
}

export const Layout: React.FC<LayoutProps> = observer(({ className }) => {
  const { appStore, canvasStore } = useStores();

  const [showAssetLibrary, setShowAssetLibrary] = React.useState(false);
  const [showWatchStyles, setShowWatchStyles] = React.useState(false);
  const [showExportDialog, setShowExportDialog] = React.useState(false);

  const handleOpenAssetLibrary = () => {
    setShowAssetLibrary(true);
  };

  const handleOpenWatchStyles = () => {
    setShowWatchStyles(true);
  };

  const handleOpenExportDialog = () => {
    setShowExportDialog(true);
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    // Handle global keyboard shortcuts
    if (event.ctrlKey || event.metaKey) {
      switch (event.key) {
        case 'z':
          event.preventDefault();
          if (event.shiftKey) {
            canvasStore.redo();
          } else {
            canvasStore.undo();
          }
          break;
        case 'c':
          event.preventDefault();
          canvasStore.copySelectedElements();
          break;
        case 'v':
          event.preventDefault();
          canvasStore.pasteElements();
          break;
        case 'x':
          event.preventDefault();
          canvasStore.cutSelectedElements();
          break;
        case 'a':
          event.preventDefault();
          canvasStore.selectAllElements();
          break;
        case 's':
          event.preventDefault();
          // TODO: Implement save functionality
          console.log('Save project');
          break;
        case 'o':
          event.preventDefault();
          // TODO: Implement open functionality
          console.log('Open project');
          break;
        case 'n':
          event.preventDefault();
          // TODO: Implement new project functionality
          console.log('New project');
          break;
        case 'e':
          event.preventDefault();
          handleOpenExportDialog();
          break;
      }
    } else {
      switch (event.key) {
        case 'Delete':
        case 'Backspace':
          event.preventDefault();
          canvasStore.deleteSelectedElements();
          break;
        case 'Escape':
          event.preventDefault();
          canvasStore.clearSelection();
          canvasStore.setCurrentTool('select');
          break;
        case 'v':
          canvasStore.setCurrentTool('select');
          break;
        case 'h':
          canvasStore.setCurrentTool('pan');
          break;
        case 't':
          canvasStore.setCurrentTool('text');
          break;
        case 'r':
          canvasStore.setCurrentTool('shape');
          break;
        case 'o':
          canvasStore.setCurrentTool('shape');
          break;
        case 'l':
          canvasStore.setCurrentTool('line');
          break;
        case 'i':
          canvasStore.setCurrentTool('image');
          break;
        case '+':
        case '=':
          event.preventDefault();
          canvasStore.zoomIn();
          break;
        case '-':
          event.preventDefault();
          canvasStore.zoomOut();
          break;
        case '0':
          event.preventDefault();
          canvasStore.resetZoom();
          break;
      }
    }
  };

  return (
    <div 
      className={`layout ${className || ''}`}
      onKeyDown={handleKeyDown}
      tabIndex={0}
    >
      {/* Header */}
      <header className="layout-header">
        <div className="header-left">
          <h1 className="app-title">ZhouHai Watch Face Designer</h1>
          <div className="project-info">
            {appStore.currentProject ? (
              <span className="project-name">{appStore.currentProject.name}</span>
            ) : (
              <span className="no-project">No project loaded</span>
            )}
          </div>
        </div>
        
        <div className="header-center">
          <DeviceSelector />
        </div>
        
        <div className="header-right">
          <button 
            className="header-btn"
            onClick={handleOpenWatchStyles}
            title="Watch Styles"
          >
            <i className="icon-palette" />
            Styles
          </button>
          <button 
            className="header-btn"
            onClick={handleOpenAssetLibrary}
            title="Asset Library"
          >
            <i className="icon-folder" />
            Assets
          </button>
          <button 
            className="header-btn export-btn"
            onClick={handleOpenExportDialog}
            title="Export Watch Face"
          >
            <i className="icon-download" />
            Export
          </button>
        </div>
      </header>

      {/* Main content area */}
      <div className="layout-main">
        {/* Left sidebar with toolbar */}
        <aside className="layout-sidebar-left">
          <Toolbar />
        </aside>

        {/* Canvas area */}
        <main className="layout-canvas-area">
          <CanvasComponent />
          
          {/* Canvas status bar */}
          <div className="canvas-status-bar">
            <div className="status-left">
              <span className="zoom-info">
                Zoom: {Math.round(canvasStore.zoomLevel * 100)}%
              </span>
              <span className="grid-info">
                Grid: {canvasStore.showGrid ? 'On' : 'Off'}
              </span>
            </div>
            
            <div className="status-center">
              <span className="canvas-size">
                {canvasStore.canvasSize.width} × {canvasStore.canvasSize.height}
              </span>
            </div>
            
            <div className="status-right">
              <span className="selection-info">
                {canvasStore.selectedElements.length > 0 
                  ? `${canvasStore.selectedElements.length} selected`
                  : 'No selection'
                }
              </span>
              <span className="tool-info">
                Tool: {canvasStore.currentTool}
              </span>
            </div>
          </div>
        </main>

        {/* Right sidebar with properties */}
        <aside className="layout-sidebar-right">
          <PropertyPanel />
        </aside>
      </div>

      {/* Modals and overlays */}
      {showAssetLibrary && (
        <AssetLibrary 
          isOpen={showAssetLibrary}
          onClose={() => setShowAssetLibrary(false)}
        />
      )}
      
      {showWatchStyles && (
        <WatchStyleManager 
          isOpen={showWatchStyles}
          onClose={() => setShowWatchStyles(false)}
        />
      )}
      
      {showExportDialog && (
        <ExportControls 
          isOpen={showExportDialog}
          onClose={() => setShowExportDialog(false)}
        />
      )}

      {/* Loading overlay */}
      {appStore.isLoading && (
        <div className="loading-overlay">
          <div className="loading-spinner">
            <div className="spinner"></div>
            <p>Loading...</p>
          </div>
        </div>
      )}

      {/* Error notification */}
      {appStore.errors.length > 0 && (
        <div className="error-notification">
          <div className="error-content">
            <i className="icon-alert" />
            <span>{appStore.errors[0].message}</span>
            <button 
              className="error-close"
              onClick={() => appStore.clearErrors()}
            >
              ×
            </button>
          </div>
        </div>
      )}
    </div>
  );
});

Layout.displayName = 'Layout';