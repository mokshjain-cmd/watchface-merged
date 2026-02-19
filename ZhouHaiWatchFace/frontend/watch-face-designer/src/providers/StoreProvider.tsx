import React from 'react';
import { StoresContext } from '../hooks/useStores';
import { AppStore } from '../stores/AppStore';
import { CanvasStore } from '../stores/CanvasStore';
import { DesignStore } from '../stores/DesignStore';

interface StoreProviderProps {
  children: React.ReactNode;
}

// Create singleton store instances
const appStore = new AppStore();
const canvasStore = new CanvasStore();
const designStore = new DesignStore();

// Set up cross-store references if needed
// For example, if DesignStore needs access to CanvasStore
(designStore as any).canvasStore = canvasStore;
(canvasStore as any).designStore = designStore;

const stores = {
  appStore,
  canvasStore,
  designStore,
};

export const StoreProvider: React.FC<StoreProviderProps> = ({ children }) => {
  return (
    <StoresContext.Provider value={stores}>
      {children}
    </StoresContext.Provider>
  );
};

export { stores };