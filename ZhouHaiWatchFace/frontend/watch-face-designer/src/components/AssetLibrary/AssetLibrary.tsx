import React from 'react';
import './AssetLibrary.scss';

interface AssetLibraryProps {
  isOpen: boolean;
  onClose: () => void;
}

export const AssetLibrary: React.FC<AssetLibraryProps> = ({ isOpen, onClose }) => {
  if (!isOpen) return null;

  const handleAssetSelect = (assetType: string) => {
    console.log(`Selected asset type: ${assetType}`);
    // TODO: Implement asset selection and import
  };

  return (
    <div className="asset-library-overlay">
      <div className="asset-library">
        <div className="modal-header">
          <h3>Asset Library</h3>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>
        
        <div className="modal-content">
          <div className="asset-categories">
            <div className="category-section">
              <h4>Watch Elements</h4>
              <div className="asset-grid">
                <button className="asset-item" onClick={() => handleAssetSelect('hands')}>
                  <i className="icon-clock" />
                  <span>Watch Hands</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('numbers')}>
                  <i className="icon-numbers" />
                  <span>Numbers</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('indices')}>
                  <i className="icon-target" />
                  <span>Indices</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('complications')}>
                  <i className="icon-gauge" />
                  <span>Complications</span>
                </button>
              </div>
            </div>

            <div className="category-section">
              <h4>Backgrounds</h4>
              <div className="asset-grid">
                <button className="asset-item" onClick={() => handleAssetSelect('gradients')}>
                  <i className="icon-gradient" />
                  <span>Gradients</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('textures')}>
                  <i className="icon-texture" />
                  <span>Textures</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('patterns')}>
                  <i className="icon-pattern" />
                  <span>Patterns</span>
                </button>
              </div>
            </div>

            <div className="category-section">
              <h4>Icons & Graphics</h4>
              <div className="asset-grid">
                <button className="asset-item" onClick={() => handleAssetSelect('weather')}>
                  <i className="icon-cloud" />
                  <span>Weather</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('fitness')}>
                  <i className="icon-heart" />
                  <span>Fitness</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('battery')}>
                  <i className="icon-battery" />
                  <span>Battery</span>
                </button>
                <button className="asset-item" onClick={() => handleAssetSelect('notifications')}>
                  <i className="icon-bell" />
                  <span>Notifications</span>
                </button>
              </div>
            </div>

            <div className="category-section">
              <h4>Import Assets</h4>
              <div className="import-section">
                <button className="import-btn">
                  <i className="icon-upload" />
                  Import Images
                </button>
                <button className="import-btn">
                  <i className="icon-folder" />
                  Browse Library
                </button>
              </div>
            </div>
          </div>
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

AssetLibrary.displayName = 'AssetLibrary';