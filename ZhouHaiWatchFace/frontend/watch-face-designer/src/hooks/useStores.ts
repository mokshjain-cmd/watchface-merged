import { createContext, useContext } from 'react';
import { AppStore } from '../stores/AppStore';
import { CanvasStore } from '../stores/CanvasStore';
import { DesignStore } from '../stores/DesignStore';

interface RootStore {
  appStore: AppStore;
  canvasStore: CanvasStore;
  designStore: DesignStore;
}

export const StoresContext = createContext<RootStore | null>(null);

export const useStores = (): RootStore => {
  const stores = useContext(StoresContext);
  if (!stores) {
    throw new Error('useStores must be used within a StoresProvider');
  }
  return stores;
};