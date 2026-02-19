// Store exports and context setup
export { AppStore, appStore } from './AppStore';
export { CanvasStore, canvasStore } from './CanvasStore';
export { DesignStore, designStore } from './DesignStore';

// Store context for React components
import React from 'react';
import { appStore } from './AppStore';
import { canvasStore } from './CanvasStore';
import { designStore } from './DesignStore';

// Store types
export interface Stores {
  appStore: typeof appStore;
  canvasStore: typeof canvasStore;
  designStore: typeof designStore;
}

// Create stores context
export const StoresContext = React.createContext<Stores | null>(null);

// Hook to use stores in components
export const useStores = (): Stores => {
  const stores = React.useContext(StoresContext);
  if (!stores) {
    throw new Error('useStores must be used within a StoresProvider');
  }
  return stores;
};

// Store instances for provider
export const stores: Stores = {
  appStore,
  canvasStore,
  designStore,
};