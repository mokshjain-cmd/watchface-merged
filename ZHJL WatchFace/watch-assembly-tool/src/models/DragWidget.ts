import { DragDataBase } from './DragDataBase';

/**
 * Orientation enumeration
 */
export enum Orientation {
  Horizontal = 'Horizontal',
  Vertical = 'Vertical'
}

/**
 * Direction enumeration for layout positioning
 */
export enum Direction {
  Center = 'Center',           // 中心
  TopLeft = 'TopLeft',         // 左上
  Top = 'Top',                 // 上
  TopRight = 'TopRight',       // 右上
  Left = 'Left',               // 左
  Right = 'Right',             // 右
  BottomLeft = 'BottomLeft',   // 左下
  Bottom = 'Bottom',           // 下
  BottomRight = 'BottomRight'  // 右下
}

/**
 * Widget container component equivalent to C# DragWidget
 * Handles layout of child components with various orientations and directions
 */
export class DragWidget extends DragDataBase {
  public children: DragDataBase[] = [];
  public orientation: Orientation = Orientation.Horizontal;
  public direction: Direction = Direction.TopLeft;
  public spacing: number = 0;

  constructor() {
    super();
  }

  /**
   * Set component size based on children
   */
  public setSize(): void {
    if (this.children.length === 0) {
      this.width = 0;
      this.height = 0;
      return;
    }

    let totalWidth = 0;
    let totalHeight = 0;
    let maxWidth = 0;
    let maxHeight = 0;

    // Calculate dimensions based on orientation
    for (const child of this.children) {
      if (this.orientation === Orientation.Horizontal) {
        totalWidth += child.width || 0;
        maxHeight = Math.max(maxHeight, child.height || 0);
      } else {
        totalHeight += child.height || 0;
        maxWidth = Math.max(maxWidth, child.width || 0);
      }
    }

    // Add spacing
    const spacingTotal = (this.children.length - 1) * this.spacing;

    if (this.orientation === Orientation.Horizontal) {
      this.width = totalWidth + spacingTotal;
      this.height = maxHeight;
    } else {
      this.width = maxWidth;
      this.height = totalHeight + spacingTotal;
    }
  }

  /**
   * Load and layout child components
   */
  public loadImages(): void {
    this.layoutChildren();
    this.setSize();
  }

  /**
   * Get all images from child components
   */
  public getAllImages(): (string | undefined)[] {
    const images: (string | undefined)[] = [];
    for (const child of this.children) {
      if ('getAllImages' in child && typeof child.getAllImages === 'function') {
        images.push(...child.getAllImages());
      }
    }
    return images;
  }

  /**
   * Layout child components according to orientation and direction
   */
  private layoutChildren(): void {
    if (this.children.length === 0) return;

    let currentX = 0;
    let currentY = 0;

    if (this.orientation === Orientation.Horizontal) {
      // Calculate total height for alignment
      const maxHeight = Math.max(...this.children.map(c => c.height || 0));

      for (const child of this.children) {
        // Position horizontally
        child.x = currentX;
        
        // Position vertically based on direction
        child.y = this.getHorizontalAlignment(maxHeight, child, this.direction);

        currentX += (child.width || 0) + this.spacing;
      }
    } else {
      // Vertical orientation
      const maxWidth = Math.max(...this.children.map(c => c.width || 0));

      for (const child of this.children) {
        // Position vertically
        child.y = currentY;
        
        // Position horizontally based on direction
        child.x = this.getVerticalAlignment(maxWidth, child, this.direction);

        currentY += (child.height || 0) + this.spacing;
      }
    }
  }

  /**
   * Get horizontal alignment position
   */
  private getHorizontalAlignment(maxHeight: number, element: DragDataBase, direction: Direction): number {
    const height = element.height || 0;
    
    switch (direction) {
      case Direction.Center:
      case Direction.Left:
      case Direction.Right:
        return (maxHeight - height) / 2;
      
      case Direction.TopLeft:
      case Direction.Top:
      case Direction.TopRight:
        return 0;
      
      case Direction.BottomLeft:
      case Direction.Bottom:
      case Direction.BottomRight:
        return maxHeight - height;
      
      default:
        return 0;
    }
  }

  /**
   * Get vertical alignment position
   */
  private getVerticalAlignment(maxWidth: number, element: DragDataBase, direction: Direction): number {
    const width = element.width || 0;
    
    switch (direction) {
      case Direction.Center:
      case Direction.Top:
      case Direction.Bottom:
        return (maxWidth - width) / 2;
      
      case Direction.TopLeft:
      case Direction.Left:
      case Direction.BottomLeft:
        return 0;
      
      case Direction.TopRight:
      case Direction.Right:
      case Direction.BottomRight:
        return maxWidth - width;
      
      default:
        return 0;
    }
  }

  /**
   * Add child component
   */
  public addChild(child: DragDataBase): void {
    this.children.push(child);
    this.loadImages();
    this.notifyPropertyChanged('children');
  }

  /**
   * Remove child component
   */
  public removeChild(child: DragDataBase): void {
    const index = this.children.indexOf(child);
    if (index > -1) {
      this.children.splice(index, 1);
      this.loadImages();
      this.notifyPropertyChanged('children');
    }
  }

  /**
   * Remove child at index
   */
  public removeChildAt(index: number): void {
    if (index >= 0 && index < this.children.length) {
      this.children.splice(index, 1);
      this.loadImages();
      this.notifyPropertyChanged('children');
    }
  }

  /**
   * Clear all children
   */
  public clearChildren(): void {
    this.children = [];
    this.loadImages();
    this.notifyPropertyChanged('children');
  }

  /**
   * Set orientation and relayout
   */
  public setOrientation(orientation: Orientation): void {
    this.orientation = orientation;
    this.loadImages();
    this.notifyPropertyChanged('orientation');
  }

  /**
   * Set direction and relayout
   */
  public setDirection(direction: Direction): void {
    this.direction = direction;
    this.loadImages();
    this.notifyPropertyChanged('direction');
  }

  /**
   * Set spacing and relayout
   */
  public setSpacing(spacing: number): void {
    this.spacing = Math.max(0, spacing);
    this.loadImages();
    this.notifyPropertyChanged('spacing');
  }

  /**
   * Get child count
   */
  public getChildCount(): number {
    return this.children.length;
  }

  /**
   * Get child at index
   */
  public getChildAt(index: number): DragDataBase | undefined {
    return this.children[index];
  }

  /**
   * Find child by drag ID
   */
  public findChildById(dragId: string): DragDataBase | undefined {
    return this.children.find(child => child.dragId === dragId);
  }

  /**
   * Get all descendant components (recursive)
   */
  public getAllDescendants(): DragDataBase[] {
    const descendants: DragDataBase[] = [];
    
    for (const child of this.children) {
      descendants.push(child);
      
      // If child is also a widget, get its descendants
      if (child instanceof DragWidget) {
        descendants.push(...child.getAllDescendants());
      }
    }
    
    return descendants;
  }

  /**
   * Get bounding box of all children
   */
  public getChildrenBounds(): { x: number; y: number; width: number; height: number } {
    if (this.children.length === 0) {
      return { x: 0, y: 0, width: 0, height: 0 };
    }

    let minX = Infinity;
    let minY = Infinity;
    let maxX = -Infinity;
    let maxY = -Infinity;

    for (const child of this.children) {
      const childX = child.x || 0;
      const childY = child.y || 0;
      const childWidth = child.width || 0;
      const childHeight = child.height || 0;

      minX = Math.min(minX, childX);
      minY = Math.min(minY, childY);
      maxX = Math.max(maxX, childX + childWidth);
      maxY = Math.max(maxY, childY + childHeight);
    }

    return {
      x: minX,
      y: minY,
      width: maxX - minX,
      height: maxY - minY
    };
  }

  /**
   * Get XML output for widget
   */
  public getOutXml(): any {
    return {
      Widget: {
        '@_X': this.x || 0,
        '@_Y': this.y || 0,
        '@_Width': this.width,
        '@_Height': this.height,
        '@_Orientation': this.orientation,
        '@_Direction': this.direction,
        '@_Spacing': this.spacing,
        Children: this.children.map(child => {
          if ('getOutXml' in child && typeof child.getOutXml === 'function') {
            return child.getOutXml();
          }
          return {};
        })
      }
    };
  }

  /**
   * Clone widget with all children
   */
  public clone(): DragWidget {
    const cloned = new DragWidget();
    cloned.dragName = this.dragName;
    cloned.width = this.width;
    cloned.height = this.height;
    cloned.x = this.x;
    cloned.y = this.y;
    cloned.visible = this.visible;
    cloned.orientation = this.orientation;
    cloned.direction = this.direction;
    cloned.spacing = this.spacing;

    // Note: Deep cloning children would require implementing clone on all child types
    cloned.children = [...this.children];
    
    return cloned;
  }

  /**
   * Validate widget configuration
   */
  public validate(): string[] {
    const errors: string[] = [];

    if (this.spacing < 0) {
      errors.push('Spacing cannot be negative');
    }

    // Validate children
    for (let i = 0; i < this.children.length; i++) {
      const child = this.children[i];
      if (!child.dragId) {
        errors.push(`Child at index ${i} missing dragId`);
      }
    }

    return errors;
  }

  /**
   * Get layout info for debugging
   */
  public getLayoutInfo(): any {
    return {
      widget: {
        id: this.dragId,
        name: this.dragName,
        size: { width: this.width, height: this.height },
        position: { x: this.x, y: this.y },
        orientation: this.orientation,
        direction: this.direction,
        spacing: this.spacing
      },
      children: this.children.map(child => ({
        id: child.dragId,
        name: child.dragName,
        size: { width: child.width, height: child.height },
        position: { x: child.x, y: child.y }
      }))
    };
  }
}