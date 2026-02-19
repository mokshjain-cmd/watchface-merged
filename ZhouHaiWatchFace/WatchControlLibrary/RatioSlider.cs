using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WatchControlLibrary
{
    public class RatioSlider : Slider
    {
        static RatioSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatioSlider), new FrameworkPropertyMetadata(typeof(RatioSlider)));
        }

        public RatioSlider()
        {
            this.Maximum = 100;
            this.Minimum = 0;
            this.ValueChanged += RatioSlider_ValueChanged;
        }

        public bool isUpdate = false;
        private void RatioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isUpdate)
            {
                isUpdate = true;
                CurrentNum = (int)(this.Value * 0.01 * TargetValue);
                isUpdate = false;
            }

        }

        public int TargetValue
        {
            get { return (int)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(int), typeof(RatioSlider), new PropertyMetadata(0, ValueOnChanged));




        public int CurrentNum
        {
            get { return (int)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(int), typeof(RatioSlider), new PropertyMetadata(0, ValueOnChanged));




        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var slider = (RatioSlider)d;
                if (slider != null && !slider.isUpdate)
                {
                    if (slider.TargetValue > 0)
                    {
                        slider.isUpdate = true;
                        slider.Value = (int)(slider.CurrentNum*100 / slider.TargetValue);
                        slider.isUpdate = false;
                    }
                }
            }
        }
    }
}
