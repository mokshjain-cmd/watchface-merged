import React from 'react';
import { observer } from 'mobx-react-lite';
import { useStores } from '../../hooks/useStores';
import type { CanvasTool } from '../../stores/CanvasStore';
import { ElementType } from '../../models';
import './Toolbar.scss';

interface ToolbarProps {
  className?: string;
}

interface ToolItem {
  id: CanvasTool;
  icon: string;
  label: string;
  shortcut?: string;
}

const tools: ToolItem[] = [
  { id: 'select', icon: '↖', label: 'Select', shortcut: 'V' },
  { id: 'pan', icon: '✋', label: 'Pan', shortcut: 'H' },
  { id: 'zoom', icon: '🔍', label: 'Zoom', shortcut: 'Z' },
  { id: 'text', icon: 'T', label: 'Text', shortcut: 'T' },
  { id: 'shape', icon: '⬜', label: 'Shape', shortcut: 'R' },
  { id: 'image', icon: '🖼', label: 'Image', shortcut: 'I' },
  { id: 'line', icon: '📏', label: 'Line', shortcut: 'L' },
];

export const Toolbar: React.FC<ToolbarProps> = observer(({ className = '' }) => {
  const { canvasStore } = useStores();

  const handleToolSelect = (toolId: CanvasTool) => {
    canvasStore.setCurrentTool(toolId);
  };

  const handleUndo = () => {
    canvasStore.undo();
  };

  const handleRedo = () => {
    canvasStore.redo();
  };

  const handleCopy = () => {
    canvasStore.copySelectedElements();
  };

  const handlePaste = () => {
    canvasStore.pasteElements();
  };

  const handleDelete = () => {
    canvasStore.deleteSelectedElements();
  };

  const handleDuplicate = () => {
    canvasStore.copySelectedElements();
    canvasStore.pasteElements();
  };

  // Image import functionality
  const handleImageImport = () => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.multiple = true;
    
    input.onchange = (event) => {
      const files = (event.target as HTMLInputElement).files;
      if (files) {
        Array.from(files).forEach(file => {
          const reader = new FileReader();
          reader.onload = (e) => {
            const imageUrl = e.target?.result as string;
            // Add image element to canvas
            canvasStore.addElement(ElementType.IMAGE, {
              src: imageUrl,
              alt: file.name,
            });
          };
          reader.readAsDataURL(file);
        });
      }
    };
    
    input.click();
  };

  const hasSelection = canvasStore.selectedElements.length > 0;
  const canUndo = canvasStore.history.length > 0 && canvasStore.historyIndex > -1;
  const canRedo = canvasStore.historyIndex < canvasStore.history.length - 1;

  return (
    <div className={`toolbar ${className}`}>
      {/* Tools Section */}
      <div className="toolbar-section">
        <h4 className="section-title">Tools</h4>
        <div className="tool-group">
          {tools.map((tool) => (
            <button
              key={tool.id}
              data-tool={tool.id}
              className={`tool-btn ${canvasStore.currentTool === tool.id ? 'active' : ''}`}
              onClick={() => handleToolSelect(tool.id)}
              title={`${tool.label} (${tool.shortcut})`}
            >
              <span className="tool-icon">{tool.icon}</span>
              <span className="tool-label">{tool.label}</span>
            </button>
          ))}
        </div>
      </div>

      {/* History Section */}
      <div className="toolbar-section">
        <h4 className="section-title">History</h4>
        <div className="tool-group horizontal">
          <button
            className={`tool-btn ${!canUndo ? 'disabled' : ''}`}
            onClick={handleUndo}
            disabled={!canUndo}
            title="Undo (Ctrl+Z)"
          >
            <span className="tool-icon">↶</span>
            <span className="tool-label">Undo</span>
          </button>
          <button
            className={`tool-btn ${!canRedo ? 'disabled' : ''}`}
            onClick={handleRedo}
            disabled={!canRedo}
            title="Redo (Ctrl+Shift+Z)"
          >
            <span className="tool-icon">↷</span>
            <span className="tool-label">Redo</span>
          </button>
        </div>
      </div>

      {/* Clipboard Section */}
      <div className="toolbar-section">
        <h4 className="section-title">Clipboard</h4>
        <div className="tool-group">
          <button
            className={`tool-btn ${!hasSelection ? 'disabled' : ''}`}
            onClick={handleCopy}
            disabled={!hasSelection}
            title="Copy (Ctrl+C)"
          >
            <span className="tool-icon">📋</span>
            <span className="tool-label">Copy</span>
          </button>
          <button
            className={`tool-btn ${!canvasStore.clipboard ? 'disabled' : ''}`}
            onClick={handlePaste}
            disabled={!canvasStore.clipboard}
            title="Paste (Ctrl+V)"
          >
            <span className="tool-icon">📄</span>
            <span className="tool-label">Paste</span>
          </button>
          <button
            className={`tool-btn ${!hasSelection ? 'disabled' : ''}`}
            onClick={handleDelete}
            disabled={!hasSelection}
            title="Delete (Del)"
          >
            <span className="tool-icon">🗑</span>
            <span className="tool-label">Delete</span>
          </button>
          <button
            className={`tool-btn ${!hasSelection ? 'disabled' : ''}`}
            onClick={handleDuplicate}
            disabled={!hasSelection}
            title="Duplicate"
          >
            <span className="tool-icon">⧉</span>
            <span className="tool-label">Duplicate</span>
          </button>
        </div>
      </div>

      {/* Import Section */}
      <div className="toolbar-section">
        <h4 className="section-title">Import</h4>
        <div className="tool-group">
          <button
            className="tool-btn"
            onClick={handleImageImport}
            title="Import Images"
          >
            <span className="tool-icon">📁</span>
            <span className="tool-label">Images</span>
          </button>
        </div>
      </div>
    </div>
  );
});

Toolbar.displayName = 'Toolbar';