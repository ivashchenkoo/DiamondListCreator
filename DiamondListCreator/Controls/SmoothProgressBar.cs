﻿using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DiamondListCreator.Controls
{
    public partial class SmoothProgressBarBehavior : Behavior<ProgressBar>
    {
        private bool _IsAnimating = false;
        private double oldValue, newValue;

        protected override void OnAttached() => AssociatedObject.ValueChanged += ProgressBar_ValueChanged;

        protected override void OnDetaching() => AssociatedObject.ValueChanged -= ProgressBar_ValueChanged;

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue.ToString().Contains(".") || e.OldValue.ToString().Contains("."))
            {
                return;
            }

            if (_IsAnimating && oldValue == e.NewValue && newValue == e.OldValue)
            {
                return;
            }
            
            _IsAnimating = true;
            DoubleAnimation animation = new DoubleAnimation(e.OldValue, e.NewValue, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            oldValue = e.OldValue;
            newValue = e.NewValue;
            animation.Completed += Db_Completed;
            AssociatedObject.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animation);
            e.Handled = true;
        }

        private void Db_Completed(object sender, EventArgs e)
        {
            _IsAnimating = false;
        }
    }
}