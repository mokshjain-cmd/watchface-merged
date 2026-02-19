
using Mapster;
using Microsoft.VisualBasic.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using WatchControlLibrary.Model;
using Control = System.Windows.Controls.Control;
using Image = System.Windows.Controls.Image;
namespace WatchControlLibrary
{
    public abstract class DragDataBase : Control, IDraggable
    {
        static DragDataBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDataBase), new FrameworkPropertyMetadata(typeof(DragDataBase)));
        }

        public Action GroupValueChanged;

        public DraggableBehavior _draggableBehavior { get; set; }
        public DragDataBase()
        {
            this.Width = 0;
            this.Height = 0;

            _draggableBehavior = new DraggableBehavior(this);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            _draggableBehavior.Attach();
        }



        private Guid? dragId;
        public Guid? DragId
        {
            get { return dragId; }
            set
            {
                if (dragId != value)
                {
                    dragId = value;
                    _draggableBehavior._border.Tag = $"drag_{DragId!.Value.ToString("N")}";
                }
            }

        }


        private string? _dragName;

        public string? DragName
        {
            get { return _dragName; }
            set
            {
                if (_dragName != value)
                {
                    _dragName = value;

                }
            }
        }




        public bool? Visable
        {
            get { return (bool?)GetValue(VisableProperty); }
            set { SetValue(VisableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisableProperty =
            DependencyProperty.Register("Visable", typeof(bool?), typeof(DragDataBase), new PropertyMetadata(
                default(bool?),
                ValueOnChanged
                ));



        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragDataBase)d;
                if (group != null)
                {
                    group.Visibility=(group.Visable??false)?Visibility.Visible:Visibility.Collapsed;
                    
                }
            }
        }



        public BindMonitorType? ElementType { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                if (Visable ?? false) 
                {
                    canvas.Visibility = Visibility.Visible;
                    SetSize();
                    LoadImages();
                }
                else 
                {
                    canvas.Visibility = Visibility.Collapsed;
                }
              


            }
        }

        public abstract void SetSize();

        public abstract void LoadImages();

    }

    public abstract class DragBindBase : Snapshoot
    {
        //private readonly IRatioService _ratioService;
        public DragBindBase()
        {
            //Left = 0;
            //Top = 0;

            //_ratioService = ratioService;

          
            //var height = _ratioService.CurrentRatio.ratio_height;
            //var width = _ratioService.CurrentRatio.ratio_width;
            Width = 0;
            Height = 0;
            DragId = Guid.NewGuid();
            SubItems = new ObservableCollection<DragBindBase>();
            Visable = true;
        }
        public string? dragName;
        public string? DragName
        {
            get { return dragName; }
            set { SetProperty(ref dragName, value); }
        }
        private double? width;

        public double? Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }


        public double? height;

        public double? left;

        public double? top;

        public Align align;

        public BindMonitorType? elementType;


        private Guid? dragId;
        public Guid? DragId
        {
            get { return dragId; }
            set { SetProperty(ref dragId, value); }
        }


        public double? Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        public double? Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        public double? Top
        {
            get { return top; }
            set { SetProperty(ref top, value); }
        }

        public Align Align
        {
            get { return align; }
            set { SetProperty(ref align, value); }
        }

        public BindMonitorType? ElementType
        {
            get { return elementType; }
            set { SetProperty(ref elementType, value); }
        }

        private double maxNum;

        public double MaxNum
        {
            get { return maxNum; }
            set { SetProperty(ref maxNum, value); }
        }

        private double minNum;

        public double MinNum
        {
            get { return minNum; }
            set { SetProperty(ref minNum, value); }
        }

        private ObservableCollection<DragBindBase>? subItems;

        public ObservableCollection<DragBindBase>? SubItems
        {
            get { return subItems; }
            set { SetProperty(ref subItems, value); }
        }

        private bool? visable;

        public bool? Visable
        {
            get { return visable; }
            set { SetProperty(ref visable, value); }
        }



        public abstract IEnumerable<string?>? GetAllImages();

        public string? ItemName { get; set; }

        public double? DefaultNum { get; set; }

        [JsonIgnore]
        public bool VerifyNullNum => DataItemTypeHelper.VerifyNullNum.Contains(ItemName);

        public abstract DragDataBaseXml GetOutXml();

        public int GetLeft()
        {
            if (Align == Align.center)
            {
                return (int)(Left - (Width / 2));
            }
            else if (Align == Align.right)
            {
                return (int)(Left - Width);
            }
            return (int)Left;
        }

    }


   




}
