// API request and response models
import type { UUID } from '@/types';
import type { WatchFace } from './WatchFace';
import type { Project, DeviceProfile } from './Project';

// Base API types
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
  errors?: ValidationError[];
  timestamp: Date;
}

export interface ApiRequest<T = any> {
  data: T;
  metadata?: RequestMetadata;
}

export interface RequestMetadata {
  userId?: UUID;
  sessionId?: UUID;
  timestamp: Date;
  version: string;
  clientInfo?: ClientInfo;
}

export interface ClientInfo {
  userAgent: string;
  platform: string;
  version: string;
  features: string[];
}

export interface ValidationError {
  field: string;
  message: string;
  code: string;
  value?: any;
}

// Authentication models
export interface LoginRequest {
  username: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
  expiresIn: number;
}

export interface User {
  id: UUID;
  username: string;
  email: string;
  displayName: string;
  avatar?: string;
  role: UserRole;
  permissions: Permission[];
  preferences: UserPreferences;
}

export enum UserRole {
  ADMIN = 'admin',
  DESIGNER = 'designer',
  VIEWER = 'viewer',
  GUEST = 'guest',
}

export interface Permission {
  resource: string;
  actions: string[];
}

export interface UserPreferences {
  theme: 'light' | 'dark' | 'auto';
  language: string;
  timezone: string;
  autoSave: boolean;
  notifications: NotificationSettings;
}

export interface NotificationSettings {
  email: boolean;
  push: boolean;
  inApp: boolean;
  frequency: 'immediate' | 'daily' | 'weekly' | 'never';
}

// Project API models
export interface ProjectCreateRequest {
  name: string;
  description?: string;
  deviceType: string;
  templateId?: UUID;
}

export interface ProjectUpdateRequest {
  id: UUID;
  name?: string;
  description?: string;
  settings?: Partial<Project['settings']>;
}

export interface ProjectListResponse {
  projects: ProjectSummary[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ProjectSummary {
  id: UUID;
  name: string;
  description?: string;
  thumbnail?: string;
  lastModified: Date;
  watchFaceCount: number;
  deviceType: string;
  isShared: boolean;
}

// Watch Face API models
export interface WatchFaceCreateRequest {
  projectId: UUID;
  name: string;
  deviceType: string;
  templateId?: UUID;
  width?: number;
  height?: number;
}

export interface WatchFaceUpdateRequest {
  id: UUID;
  watchFace: Partial<WatchFace>;
}

export interface WatchFaceDuplicateRequest {
  id: UUID;
  newName: string;
  projectId?: UUID;
}

export interface WatchFaceExportRequest {
  id: UUID;
  format: 'json' | 'bin' | 'png' | 'zip';
  settings?: ExportSettings;
}

export interface ExportSettings {
  includeAssets: boolean;
  compression: boolean;
  quality?: number;
  scale?: number;
}

export interface WatchFaceImportRequest {
  projectId: UUID;
  file: File;
  settings?: ImportSettings;
}

export interface ImportSettings {
  overwriteExisting: boolean;
  preserveIds: boolean;
  importAssets: boolean;
}

// Asset API models
export interface AssetUploadRequest {
  projectId: UUID;
  files: File[];
  category?: AssetCategory;
}

export enum AssetCategory {
  IMAGES = 'images',
  FONTS = 'fonts',
  ANIMATIONS = 'animations',
  ICONS = 'icons',
  BACKGROUNDS = 'backgrounds',
}

export interface AssetUploadResponse {
  assets: AssetInfo[];
  failed: FailedUpload[];
}

export interface AssetInfo {
  id: UUID;
  name: string;
  originalName: string;
  url: string;
  thumbnailUrl?: string;
  size: number;
  mimeType: string;
  category: AssetCategory;
  metadata: AssetMetadata;
  uploadedAt: Date;
}

export interface AssetMetadata {
  width?: number;
  height?: number;
  duration?: number; // for animations
  colorDepth?: number;
  hasTransparency?: boolean;
  format?: string;
}

export interface FailedUpload {
  fileName: string;
  error: string;
}

export interface AssetListRequest {
  projectId?: UUID;
  category?: AssetCategory;
  search?: string;
  page?: number;
  pageSize?: number;
  sortBy?: 'name' | 'size' | 'uploadedAt';
  sortOrder?: 'asc' | 'desc';
}

export interface AssetListResponse {
  assets: AssetInfo[];
  total: number;
  page: number;
  pageSize: number;
}

// Device API models
export interface DeviceListResponse {
  devices: DeviceProfile[];
  categories: DeviceCategory[];
}

export interface DeviceCategory {
  id: string;
  name: string;
  description: string;
  deviceIds: string[];
}

// Template API models
export interface TemplateListRequest {
  deviceType?: string;
  category?: TemplateCategory;
  search?: string;
  featured?: boolean;
}

export enum TemplateCategory {
  ANALOG = 'analog',
  DIGITAL = 'digital',
  HYBRID = 'hybrid',
  SPORTS = 'sports',
  BUSINESS = 'business',
  CASUAL = 'casual',
  ARTISTIC = 'artistic',
}

export interface TemplateInfo {
  id: UUID;
  name: string;
  description: string;
  thumbnail: string;
  previewUrl: string;
  category: TemplateCategory;
  deviceTypes: string[];
  isFeatured: boolean;
  isPremium: boolean;
  downloadCount: number;
  rating: number;
  tags: string[];
  createdBy: string;
  createdAt: Date;
}

export interface TemplateListResponse {
  templates: TemplateInfo[];
  total: number;
  categories: TemplateCategory[];
}

// Sharing and collaboration models
export interface ShareProjectRequest {
  projectId: UUID;
  shareType: ShareType;
  permissions: SharePermission[];
  expiresAt?: Date;
  message?: string;
}

export enum ShareType {
  PUBLIC = 'public',
  PRIVATE = 'private',
  LINK = 'link',
}

export interface SharePermission {
  userId?: UUID;
  email?: string;
  role: 'viewer' | 'editor' | 'admin';
}

export interface ShareInfo {
  id: UUID;
  shareUrl: string;
  expiresAt?: Date;
  permissions: SharePermission[];
  createdAt: Date;
}

// Feedback and analytics models
export interface FeedbackRequest {
  type: 'bug' | 'feature' | 'general';
  subject: string;
  description: string;
  priority: 'low' | 'medium' | 'high';
  attachments?: File[];
  userAgent?: string;
  url?: string;
}

export interface AnalyticsEvent {
  event: string;
  properties: Record<string, any>;
  userId?: UUID;
  sessionId: UUID;
  timestamp: Date;
}

// Health check and system status
export interface HealthCheckResponse {
  status: 'healthy' | 'degraded' | 'unhealthy';
  version: string;
  uptime: number;
  services: ServiceStatus[];
  timestamp: Date;
}

export interface ServiceStatus {
  name: string;
  status: 'up' | 'down' | 'degraded';
  responseTime?: number;
  lastCheck: Date;
  message?: string;
}