// Main application store - equivalent to MainViewModel in C#
import { makeAutoObservable, runInAction } from 'mobx';
import type { 
  Project, 
  DeviceProfile,
  User
} from '@/models';
import { UserRole } from '@/models';
import { generateId } from '@/models/ModelHelpers';

export class AppStore {
  // Application state
  isLoading = false;
  currentUser: User | null = null;
  currentProject: Project | null = null;
  availableDevices: DeviceProfile[] = [];
  
  // UI state
  theme: 'light' | 'dark' | 'auto' = 'auto';
  language = 'en';
  sidebarCollapsed = false;
  
  // Navigation state
  currentView: AppView = 'welcome';
  navigationHistory: NavigationEntry[] = [];
  
  // Error handling
  errors: AppError[] = [];
  notifications: AppNotification[] = [];

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
    this.loadInitialData();
  }

  // Navigation methods (equivalent to RegionNavigation in C#)
  navigateTo(view: AppView, params?: Record<string, any>) {
    const entry: NavigationEntry = {
      id: generateId(),
      view,
      params,
      timestamp: new Date(),
    };

    runInAction(() => {
      this.navigationHistory.push(entry);
      this.currentView = view;
    });
  }

  navigateBack() {
    if (this.navigationHistory.length > 1) {
      runInAction(() => {
        this.navigationHistory.pop();
        const previousEntry = this.navigationHistory[this.navigationHistory.length - 1];
        this.currentView = previousEntry.view;
      });
    }
  }

  canNavigateBack() {
    return this.navigationHistory.length > 1;
  }

  // Project management
  async createProject(name: string, deviceType: string, description?: string) {
    this.setLoading(true);
    try {
      const project: Project = {
        id: generateId(),
        createTime: new Date(),
        updateTime: new Date(),
        version: '1.0.0',
        name,
        description,
        watchFaces: [],
        settings: {
          defaultDevice: deviceType,
          autoSave: true,
          autoSaveInterval: 5,
          showGrid: true,
          snapToGrid: true,
          gridSize: 10,
          theme: 'auto',
          language: 'en',
          exportFormats: [
            { type: 'json', enabled: true },
            { type: 'bin', enabled: true },
            { type: 'png', enabled: true },
          ],
        },
        recentFiles: [],
      };

      runInAction(() => {
        this.currentProject = project;
      });

      this.addNotification({
        type: 'success',
        title: 'Project Created',
        message: `Project "${name}" has been created successfully.`,
      });

      return project;
    } catch (error) {
      this.addError('Failed to create project', error);
      throw error;
    } finally {
      this.setLoading(false);
    }
  }

  async loadProject(_projectId: string) {
    this.setLoading(true);
    try {
      // In a real app, this would load from API
      // For now, simulate loading
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // This would be replaced with actual API call
      throw new Error('Project loading not implemented yet');
    } catch (error) {
      this.addError('Failed to load project', error);
      throw error;
    } finally {
      this.setLoading(false);
    }
  }

  // Device management
  async loadDevices() {
    this.setLoading(true);
    try {
      // Load device configurations from JSON
      const response = await fetch('/src/assets/configs/devices.json');
      const deviceData = await response.json();
      
      runInAction(() => {
        this.availableDevices = deviceData.devices;
      });
    } catch (error) {
      this.addError('Failed to load device configurations', error);
    } finally {
      this.setLoading(false);
    }
  }

  getDeviceById(deviceId: string): DeviceProfile | undefined {
    return this.availableDevices.find(device => device.id === deviceId);
  }

  // User management
  async login(username: string, _password: string) {
    this.setLoading(true);
    try {
      // Simulate login - replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const user: User = {
        id: generateId(),
        username,
        email: `${username}@example.com`,
        displayName: username,
        role: UserRole.DESIGNER,
        permissions: [],
        preferences: {
          theme: 'auto',
          language: 'en',
          timezone: 'UTC',
          autoSave: true,
          notifications: {
            email: true,
            push: true,
            inApp: true,
            frequency: 'immediate',
          },
        },
      };

      runInAction(() => {
        this.currentUser = user;
        this.theme = user.preferences.theme;
        this.language = user.preferences.language;
      });

      this.addNotification({
        type: 'success',
        title: 'Welcome!',
        message: `Successfully logged in as ${user.displayName}.`,
      });

      return user;
    } catch (error) {
      this.addError('Login failed', error);
      throw error;
    } finally {
      this.setLoading(false);
    }
  }

  logout() {
    runInAction(() => {
      this.currentUser = null;
      this.currentProject = null;
      this.currentView = 'welcome';
      this.navigationHistory = [];
    });
  }

  // Settings management
  updateTheme(theme: 'light' | 'dark' | 'auto') {
    runInAction(() => {
      this.theme = theme;
      if (this.currentUser) {
        this.currentUser.preferences.theme = theme;
      }
    });
  }

  updateLanguage(language: string) {
    runInAction(() => {
      this.language = language;
      if (this.currentUser) {
        this.currentUser.preferences.language = language;
      }
    });
  }

  toggleSidebar() {
    runInAction(() => {
      this.sidebarCollapsed = !this.sidebarCollapsed;
    });
  }

  // Error handling
  addError(message: string, error?: any) {
    const appError: AppError = {
      id: generateId(),
      message,
      details: error?.message || String(error),
      timestamp: new Date(),
      level: 'error',
    };

    runInAction(() => {
      this.errors.push(appError);
    });
  }

  removeError(errorId: string) {
    runInAction(() => {
      this.errors = this.errors.filter(error => error.id !== errorId);
    });
  }

  clearErrors() {
    runInAction(() => {
      this.errors = [];
    });
  }

  // Notifications
  addNotification(notification: Omit<AppNotification, 'id' | 'timestamp'>) {
    const appNotification: AppNotification = {
      id: generateId(),
      timestamp: new Date(),
      ...notification,
    };

    runInAction(() => {
      this.notifications.push(appNotification);
    });

    // Auto-remove after delay
    setTimeout(() => {
      this.removeNotification(appNotification.id);
    }, notification.duration || 5000);
  }

  removeNotification(notificationId: string) {
    runInAction(() => {
      this.notifications = this.notifications.filter(n => n.id !== notificationId);
    });
  }

  // Loading state
  setLoading(loading: boolean) {
    runInAction(() => {
      this.isLoading = loading;
    });
  }

  // Computed properties
  get isLoggedIn() {
    return this.currentUser !== null;
  }

  get hasProject() {
    return this.currentProject !== null;
  }

  get hasErrors() {
    return this.errors.length > 0;
  }

  get hasNotifications() {
    return this.notifications.length > 0;
  }

  // Initialize app data
  private async loadInitialData() {
    try {
      await this.loadDevices();
    } catch (error) {
      console.error('Failed to load initial data:', error);
    }
  }
}

// Types for the app store
export type AppView = 
  | 'welcome'
  | 'projects'
  | 'design'
  | 'preview'
  | 'export'
  | 'settings'
  | 'templates'
  | 'assets';

export interface NavigationEntry {
  id: string;
  view: AppView;
  params?: Record<string, any>;
  timestamp: Date;
}

export interface AppError {
  id: string;
  message: string;
  details?: string;
  timestamp: Date;
  level: 'error' | 'warning' | 'info';
}

export interface AppNotification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  timestamp: Date;
  duration?: number; // ms
  action?: {
    label: string;
    handler: () => void;
  };
}

// Create singleton instance
export const appStore = new AppStore();