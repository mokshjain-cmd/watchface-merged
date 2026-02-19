using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using WatchControlLibrary.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Prism.Mvvm;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Text.Json.Serialization;

namespace WatchControlLibrary
{

    public class DragSlot : DragDataBase
    {
        static DragSlot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSlot), new FrameworkPropertyMetadata(typeof(DragSlot)));
        }
        //private readonly IRatioService _ratioService;
        //public DragSlot(IRatioService ratioService) : base()
        //{
        //    _ratioService = ratioService;

        //    // 访问 ratio 值
        //    var height = _ratioService.CurrentRatio.ratio_height;
        //    var width = _ratioService.CurrentRatio.ratio_width;
        //}
        public DragSlot() : base()
        { 
        }

        public string FolderName
        {
            get { return (string)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WatchFace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderNameProperty =
            DependencyProperty.Register("FolderName", typeof(string), typeof(DragSlot), new PropertyMetadata(null));



        public ObservableCollection<DragDataBase>? Widgets
        {
            get { return (ObservableCollection<DragDataBase>)GetValue(WidgetsProperty); }
            set { SetValue(WidgetsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Widgets.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidgetsProperty =
            DependencyProperty.Register("Widgets", typeof(ObservableCollection<DragDataBase>), typeof(DragSlot), new PropertyMetadata(null));


        public ObservableCollection<SlotPosition>? Positions
        {
            get { return (ObservableCollection<SlotPosition>?)GetValue(PositionsProperty); }
            set { SetValue(PositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Positions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionsProperty =
            DependencyProperty.Register("Positions", typeof(ObservableCollection<SlotPosition>), typeof(DragSlot), new PropertyMetadata(null, OnValueChange));


        public bool HasNull
        {
            get { return (bool)GetValue(HasNullProperty); }
            set { SetValue(HasNullProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasNull.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasNullProperty =
            DependencyProperty.Register("HasNull", typeof(bool), typeof(DragSlot), new PropertyMetadata(false, AddNullWidget));

        private static void AddNullWidget(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DragSlot slot)
            {
                if ((bool)e.NewValue)
                {
                    var emptyPath = CommonHelper.CurrentPath(slot.FolderName) + "_emptyImage.png";
                    if (!File.Exists(emptyPath))
                    {
                        RenderTargetBitmap renderBitmap =
                        new RenderTargetBitmap(
                        (int)slot.Width,
                        (int)slot.Height,
                        96d,
                        96d,
                        PixelFormats.Pbgra32);
                        renderBitmap.Render(new Canvas());

                        using (FileStream outStream = new FileStream(emptyPath, FileMode.OpenOrCreate))
                        {
                            PngBitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                            encoder.Save(outStream);
                        }
                    }
                    //if ((bool)e.NewValue)
                    slot.Widgets?.Add(new DragWidget
                    {
                        DragName = "无",
                        Children = new ObservableCollection<DragDataBase>()
                        {
                            new DragImage
                            {
                                Source =  $".\\ZHWatch\\{slot.FolderName}\\_emptyImage.png"
                            }
                        }
                    });
                }
                else
                {
                    slot.Widgets?.RemoveAt(slot.Widgets.Count - 1);
                }
            }

        }

        public bool IsCustomPos
        {
            get { return (bool)GetValue(IsCustomPosProperty); }
            set { SetValue(IsCustomPosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCustomPos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCustomPosProperty =
            DependencyProperty.Register("IsCustomPos", typeof(bool), typeof(DragSlot), new PropertyMetadata(false));


        public int ShowPosIndex
        {
            get { return (int)GetValue(ShowPosIndexProperty); }
            set { SetValue(ShowPosIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPosIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPosIndexProperty =
            DependencyProperty.Register("ShowPosIndex", typeof(int), typeof(DragSlot), new PropertyMetadata(0, OnValueChange));



        public int ShowItemIndex
        {
            get { return (int)GetValue(ShowItemIndexProperty); }
            set { SetValue(ShowItemIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowItemIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowItemIndexProperty =
            DependencyProperty.Register("ShowItemIndex", typeof(int), typeof(DragSlot), new PropertyMetadata(0, OnValueChange));


        private static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DragSlot slot)
            {
                slot.LoadImages();
                if (slot.Positions.Count > 0 && slot.Positions.Select(p => p.Parent is DragSlot).Count(x => x) != slot.Positions.Count)
                    foreach (var position in slot.Positions)
                        position.Parent = slot;
            }
        }

        public override void LoadImages()
        {
            if (Widgets == null || Widgets.Count == 0) return;
            if (GetTemplateChild("PART_Canvas") is not Canvas canvas) return;
            canvas.Children.Clear();
            if (ShowItemIndex + 1 <= Widgets.Count) 
            {
                DragWidget widget = Widgets[ShowItemIndex] as DragWidget;
                canvas.Children.Add(widget);
                DraggableBehavior.SetLeft(widget, 0);
                DraggableBehavior.SetTop(widget, 0);
                DraggableBehavior.SetIsDraggable(widget, false);
                widget._draggableBehavior.Detach();
                widget.LoadImages();
                if (Positions?.Count > 0 && ShowPosIndex > -1)
                {
                    DraggableBehavior.SetLeft(widget, Positions[ShowPosIndex].X - DraggableBehavior.GetLeft(this));
                    DraggableBehavior.SetTop(widget, Positions[ShowPosIndex].Y - DraggableBehavior.GetTop(this));
                }
            }
           
            //if (widget.IsAutoLayout ?? false)
            //{
            //    widget.AutoLayout(canvas, widget.Children, widget.Orientation, widget.Direction, widget.Spacing);
            //}
            //else
            //{
            //    widget.NoramlLayout(canvas, widget.Children);
            //}
           
            //foreach (var item in widget.Children)
            //{
            //    item._draggableBehavior.Detach();
            //    //DraggableBehavior.SetIsDraggable(item, false);
            //}
           
        }

        public override void SetSize()
        {
            if (Widgets == null || Widgets.Count == 0) return;
            //var widgets = Widgets;
            //this.Width = widgets.First().Width;
            //this.Height = widgets.First().Height;
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            foreach (var widget in Widgets)
            {
                if (widget.Width > this.Width) this.Width = widget.Width;
                if (widget.Height > this.Height) this.Height = widget.Height;
                canvas?.Children.Add(widget);
            }
        }
    }

    public class DragBindSlot : DragBindBase
    {
        public DragBindSlot()
        {
            IsAuto = true;
        }

        public bool IsAuto { get; set; }
        public static Dictionary<string, string> SlotTypeData => new()
        {
            {"数据项", "widget" },
            {"Time", "time" },
            {"刻度盘", "dial" },
            {"字体样式", "font" },
            {"Dual Time Zone", "dualTime" },
        };
        private string? folderName;
        public string? FolderName
        {
            get { return folderName; }
            set { SetProperty(ref folderName, value); }
        }
        private string? slotType;

        public string? SlotType
        {
            get { return slotType; }
            set { SetProperty(ref slotType, value); }
        }

        private bool hasNull;

        public bool HasNull
        {
            get { return hasNull; }
            set { SetProperty(ref hasNull, value); AddItem(value); }
        }

        private void AddItem(bool value) //暂时不用
        {
            if (value)
            {
                if (SubItems.Count == 0 || SubItems.FirstOrDefault(w => w.DragName == "无") != null)
                {
                    return;
                }
                var w = new DragBindWidget
                {
                    DragName = "无",
                    Left = 0,
                    Top = 0,
                };
                w.SubItems.Add(new DragBindImage
                {
                    DragName = "空",
                    Source = $".\\ZHWatch\\{FolderName}\\_emptyImage.png"
                });
                SubItems.Add(w);
            }
            else
            {
                //if (SubItems?.Count != 0)
                //    SubItems?.RemoveAt(SubItems.Count - 1);
            }
        }



        private bool isCustomPos;

        public bool IsCustomPos
        {
            get { return isCustomPos; }
            set
            {
                SetProperty(ref isCustomPos, value);
                if (!IsAuto)
                    PosChange(value);
                IsAuto = false;
            }
        }

        public ObservableCollection<SlotPosition>? Positions
        {
            get; set;
        } = new ObservableCollection<SlotPosition>();

        private void PosChange(bool value)
        {
            if (value)
            {
                if (SubItems.Count == 0)
                {
                    return;
                }

                Positions?.Add(new SlotPosition
                {
                    CurSlotPosition = true,
                    X = Left.Value,
                    Y = Top.Value,
                });
                ShowPosIndex = 0;
            }
            else
            {
                Positions?.Clear();
            }
        }

        private int showPosIndex = -1;

        public int ShowPosIndex
        {
            get { return showPosIndex; }
            set { ChangeShowPos(showPosIndex, value); SetProperty(ref showPosIndex, value); }
        }

        private int showItemIndex;

        public int ShowItemIndex
        {
            get { return showItemIndex; }
            set { SetProperty(ref showItemIndex, value); }
        }

        private void ChangeShowPos(int oldindex, int newindex)
        {
            if (Positions == null || Positions.Count == 0 || newindex == -1) return;
            if (oldindex != -1) Positions[oldindex].CurSlotPosition = false;
            if (newindex < Positions.Count)
                Positions[newindex].CurSlotPosition = true;
        }


        public override IEnumerable<string?>? GetAllImages()
        {
            return null;
        }

        public override DragDataBaseXml GetOutXml()
        {


            var outXml = new DragDataBaseXml();
            foreach (var widget in this.SubItems!.Where(x => x is DragBindWidget))
            {
                widget.Left = 0;
                widget.Top = 0;
            }
            var widgetout = SubItems!.Select(s => s.GetOutXml()).ToList();
            outXml.Images!.AddRange(widgetout.SelectMany(w => w.Images!));
            outXml.ImageArrays!.AddRange(widgetout.SelectMany(w => w.ImageArrays!));
            outXml.DataItemImageNumbers!.AddRange(widgetout.SelectMany(w => w.DataItemImageNumbers!));
            outXml.DataItemImageValues!.AddRange(widgetout.SelectMany(w => w.DataItemImageValues!));
            outXml.Widgets!.AddRange(widgetout.SelectMany(w => w.Widgets!));
            // this.SubItems
            Slot slot = new()
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Type = SlotType,
                //Items = this.SubItems!.Where(x => x is DragBindWidget).Select(x => new Model.Item
                //{
                //    Ref = $@"@{OutXmlHelper.WidgetTable[x.DragId!.Value]}"

                //}).ToList(),
                Items= outXml.Widgets.Where(x=>this.SubItems!.Select(s=>s.DragId).Contains(x.DragId)).Select(x => new Model.Item 
                {
                    Ref = $@"@{x.Name}"
                }).ToList(),
            };
            if (!Directory.Exists(CommonHelper.Widget(FolderName!)))
            {
                Directory.CreateDirectory(CommonHelper.Widget(FolderName!));
            }

            for (int i = 0; i < SubItems!.Count; i++)
            {
                if (SubItems[i] is DragBindWidget widget)
                {
                    //editBox
                    if (string.IsNullOrWhiteSpace(widget.EditBox))
                    {
                        throw new ArgumentNullException("请选择组件边框");
                    }
                    var editBox = OutXmlHelper.GetWatchElementName();
                    outXml.Images.Add(new WatchImage
                    {
                        Name = editBox,
                        Src = widget.EditBox,

                    });
                   

                    //preview
                    if (string.IsNullOrWhiteSpace(widget.Preview))
                    {
                        throw new ArgumentNullException("请选择组件预览图");
                    }
                    var preview = OutXmlHelper.GetWatchElementName();

                    outXml.Images.Add(new WatchImage
                    {
                        Name = preview,
                        Src = widget.Preview,
                    });
                   
                    //Translations
                    var translationName = OutXmlHelper.GetWatchElementNameByPx("Translation");

                    var tranlastion = new Translation
                    {
                        Name = translationName,
                        Items = CommonHelper.Languages.Select(l => new TranslationItem
                        {
                            Language = l,
                            Str = widget.DragName
                        }).ToList(),

                    };
                    outXml.Translations.Add(tranlastion);

                    var xmlWidget = outXml.Widgets.FirstOrDefault(x => x.DragId == widget.DragId);
                    if (xmlWidget != null)
                    {
                        xmlWidget.EditBox = $"@{editBox}";
                        xmlWidget.Preview = $"@{preview}";
                        xmlWidget.WidgetName = $@"@{translationName}";
                    }
                    //if (SubItems[i].DragName == "无")//暂时不用
                    //{
                    //    outXml.Widgets[i].Preview = outXml.Widgets[i].Items[0].Ref;
                    //    outXml.Widgets[i].GroupType = "none";
                    //}
                }

            }
            if (IsCustomPos)
            {
                slot.Movable = true;
                slot.Positions = Positions.Select(p => new Position { X = (int)p.X, Y = (int)p.Y, CurSlotPosition = p.CurSlotPosition }).ToList();
            }
            outXml.Slots.Add(slot);
            outXml.Layout = new Layout
            {
                Ref = $"@{slot.Name}",
                X = (int)this.Left,
                Y = (int)this.Top,
            };
            return outXml;
        }
    }
    public class SlotPosition : Snapshoot
    {
        private double x;
        private double y;
        private bool curSlotPosition;

        [Newtonsoft.Json.JsonIgnore]
        public object? Parent { get; set; }

        public double X
        {
            get { return x; }
            set { SetProperty(ref x, value); RaisePropertyChangedEvent("X"); }
        }


        public double Y
        {
            get { return y; }
            set { SetProperty(ref y, value); RaisePropertyChangedEvent("Y"); }
        }


        public bool CurSlotPosition
        {
            get { return curSlotPosition; }
            set { SetProperty(ref curSlotPosition, value); }
        }

        private void RaisePropertyChangedEvent(string name)
        {
            if (Parent != null && CurSlotPosition)
            {
                var slot = Parent as DragSlot;
                var widget = slot.Widgets[slot.ShowItemIndex] as DragWidget;
                switch (name)
                {
                    case "X": DraggableBehavior.SetLeft(widget, this.X - DraggableBehavior.GetLeft(slot)); break;  // Canvas.SetLeft ??
                    case "Y": DraggableBehavior.SetTop(widget, this.Y - DraggableBehavior.GetTop(slot)); break;
                    default: break;
                }
            }

        }
    }
}
