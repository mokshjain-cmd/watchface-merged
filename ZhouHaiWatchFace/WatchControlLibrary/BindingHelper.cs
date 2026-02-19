using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Binding = System.Windows.Data.Binding;
using System.Windows.Markup.Primitives;
using System.Xml.Linq;
using WatchControlLibrary.Model;
using System.Windows.Forms;
using System.ComponentModel;
using Prism.Mvvm;
using Control = System.Windows.Controls.Control;

namespace WatchControlLibrary
{
    public class BindingHelper
    {
        /// <summary>
        /// 自动绑定依赖属性
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="targetObject"></param>
        public static void AutoBindDependencyProperties(object sourceObject, System.Windows.Controls.Control? targetObject)
        {
            Type sourceType = sourceObject.GetType();
            Type? targetType = targetObject?.GetType();
            Type attchType = typeof(DraggableBehavior); // 附加属性的拥有者类型
            foreach (PropertyInfo sourceProperty in sourceType.GetProperties())
            {
                if (sourceProperty.Name == "Width" || sourceProperty.Name == "Height") //这两个属性在父类
                {
                    var fram = targetObject as DragDataBase;
                    DependencyProperty frameworkProperty = sourceProperty.Name == "Width" ? Control.WidthProperty : Control.HeightProperty;
                    Binding binding = new Binding(sourceProperty.Name)
                    {
                        Source = sourceObject,
                        Mode = BindingMode.TwoWay,
                    };
                    targetObject.SetBinding(frameworkProperty, binding);
                    // BindingOperations.SetBinding(targetObject, frameworkProperty, binding);
                }

                else
                {
                    string dependencyPropertyName = sourceProperty.Name + "Property";
                    FieldInfo? dependencyPropertyField = targetType.GetField(dependencyPropertyName, BindingFlags.Static | BindingFlags.Public);
                    if (dependencyPropertyField == null) 
                    {
                        dependencyPropertyField = targetType.BaseType.GetField(dependencyPropertyName, BindingFlags.Static | BindingFlags.Public);
                    }
                    if (dependencyPropertyField == null)
                    {
                        dependencyPropertyField = attchType.GetField(dependencyPropertyName, BindingFlags.Static | BindingFlags.Public);
                    }
                    if (dependencyPropertyField != null)
                    {
                        DependencyProperty? dependencyProperty = dependencyPropertyField?.GetValue(null) as DependencyProperty;
                        if (dependencyProperty != null)
                        {
                            // 创建 Binding 对象
                            Binding binding = new Binding(sourceProperty.Name)
                            {
                                Source = sourceObject,
                                Mode = BindingMode.TwoWay,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            };
                            
                            // 设置绑定
                            BindingOperations.SetBinding(targetObject, dependencyProperty, binding);
                        }

                    }
                }
            }
        }

        public static void BindValue(MonitorItem item, DragBindBase dragBind, string SourceName, string BindName)
        {
            var currentNumProperty = dragBind.GetType().GetProperty(BindName);
            if (currentNumProperty != null)
            {
                var numProperty = item.GetType().GetProperty(SourceName);
                if (numProperty != null)
                {
                    currentNumProperty.SetValue(dragBind, numProperty.GetValue(item));
                }
            }
        }

       
        public static void MonitorValueBind(MonitorItem item, DragBindBase dragBind)
        {
            BindValue(item, dragBind, "CurrentDateTime", "SetDateTime");

            item.PropertyChanged += (s, e) =>
            {
                if (e?.PropertyName == "CurrentDateTime")
                {
                    BindValue(item, dragBind, e.PropertyName!, "SetDateTime");
                }
            };
        }

    }

}
