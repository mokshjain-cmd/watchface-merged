import React from 'react';
import './WatchStyleManager.scss';

interface WatchStyleManagerProps {
  isOpen: boolean;
  onClose: () => void;
}

export const WatchStyleManager: React.FC<WatchStyleManagerProps> = ({ isOpen, onClose }) => {
  if (!isOpen) return null;

  return (
    <div className="watch-style-manager-overlay">
      <div className="watch-style-manager">
        <div className="modal-header">
          <h3>Watch Styles Manager</h3>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>
        
        <div className="modal-content">
          <p>Watch styles management coming soon...</p>
          <p>This will include:</p>
          <ul>
            <li>Predefined watch face templates</li>
            <li>Style presets and themes</li>
            <li>Color scheme management</li>
            <li>Component libraries</li>
          </ul>
        </div>
        
        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

WatchStyleManager.displayName = 'WatchStyleManager';