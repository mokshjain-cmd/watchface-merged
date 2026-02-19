using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace WatchControlLibrary
{
    public abstract class DragImageSource : DragDataBase
    {
        static DragImageSource()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragImageSource), new FrameworkPropertyMetadata(typeof(DragImageSource)));
        }


        public DragImageSource() : base()
        {

        }



        public ObservableCollection<string>? ImageSource
        {
            get { return (ObservableCollection<string>?)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ObservableCollection<string>), typeof(DragDataBase), new PropertyMetadata(default(ObservableCollection<string>), SourceChanged));


        public  override void LoadImages()
        {
          
        }

        public override void SetSize()
        {
            
        }
        public static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DragImageSource dragDataBase)
            {
                dragDataBase.OnFolderSourceChanged(d, e);
            }
        }
        public abstract void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);

    }
}
