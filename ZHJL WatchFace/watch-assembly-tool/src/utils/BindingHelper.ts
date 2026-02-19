// TypeScript equivalent of BindingHelper.cs
export class BindingHelper {
  /**
   * Auto bind dependency properties
   * @param sourceObject Source object to bind from
   * @param targetObject Target object to bind to
   */
  public static autoBindDependencyProperties(sourceObject: any, targetObject: any): void {
    const sourceType = sourceObject.constructor;
    const targetType = targetObject.constructor;
    
    // Get all properties from source object
    for (const sourcePropertyName in sourceObject) {
      if (sourceObject.hasOwnProperty(sourcePropertyName)) {
        if (sourcePropertyName === 'width' || sourcePropertyName === 'height') {
          // Handle width and height properties specially
          const binding = this.createBinding(sourceObject, sourcePropertyName, 'TwoWay');
          this.setBinding(targetObject, sourcePropertyName, binding);
        } else {
          // Handle other properties
          const dependencyPropertyName = sourcePropertyName + 'Property';
          
          // Check if target has this dependency property
          if (targetType.hasOwnProperty(dependencyPropertyName) || 
              targetType.prototype.hasOwnProperty(dependencyPropertyName)) {
            
            const binding = this.createBinding(sourceObject, sourcePropertyName, 'TwoWay');
            this.setBinding(targetObject, sourcePropertyName, binding);
          }
        }
      }
    }
  }

  /**
   * Bind a specific value from source to target
   * @param item Monitor item (source)
   * @param dragBind Drag bind object (target)
   * @param sourceName Property name on source
   * @param bindName Property name on target
   */
  public static bindValue(item: any, dragBind: any, sourceName: string, bindName: string): void {
    if (dragBind.hasOwnProperty(bindName) && item.hasOwnProperty(sourceName)) {
      dragBind[bindName] = item[sourceName];
    }
  }

  /**
   * Monitor value binding with property change notifications
   * @param item Monitor item
   * @param dragBind Drag bind object
   */
  public static monitorValueBind(item: any, dragBind: any): void {
    // Initial binding
    this.bindValue(item, dragBind, 'currentDateTime', 'setDateTime');

    // Set up property change listener
    if (item.addPropertyChangeListener) {
      item.addPropertyChangeListener((propertyName: string) => {
        if (propertyName === 'currentDateTime') {
          this.bindValue(item, dragBind, propertyName, 'setDateTime');
        }
      });
    }
  }

  /**
   * Create a binding object
   * @param source Source object
   * @param propertyName Property name to bind
   * @param mode Binding mode
   * @returns Binding configuration
   */
  private static createBinding(source: any, propertyName: string, mode: 'OneWay' | 'TwoWay' | 'OneTime' = 'OneWay') {
    return {
      source: source,
      propertyName: propertyName,
      mode: mode,
      updateTrigger: 'PropertyChanged'
    };
  }

  /**
   * Set binding on target object
   * @param target Target object
   * @param propertyName Property name
   * @param binding Binding configuration
   */
  private static setBinding(target: any, propertyName: string, binding: any): void {
    // In web environment, we can set up reactive properties or use Vue/React reactive systems
    Object.defineProperty(target, propertyName, {
      get: () => binding.source[binding.propertyName],
      set: (value) => {
        if (binding.mode === 'TwoWay') {
          binding.source[binding.propertyName] = value;
        }
      },
      enumerable: true,
      configurable: true
    });
  }
}