﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace LoadResourceWpf
{
    public class ResourcePicker : Control
    {
        private ComboBox ComboBox;

        public override void OnApplyTemplate()
        {
            ComboBox = this.GetTemplateChild("ComboBox") as ComboBox;
            ComboBox.SelectionChanged += ComboBox_SelectionChanged;
            a();
        }


        static ResourcePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResourcePicker), new FrameworkPropertyMetadata(typeof(ResourcePicker)));
   
        }

        public ResourcePicker()
        {

        }
        
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }


        public static readonly DependencyProperty PathProperty =            DependencyProperty.Register("Path", typeof(string), typeof(ResourcePicker), new PropertyMetadata(null,PathChanged));

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResourcePicker).a();
        }

        private void a()
        {
            if (ComboBox != null)
                this.Dispatcher.InvokeAsync(() =>
            ComboBox.ItemsSource = System.IO.Directory.GetFiles(Path).Select(_=>System.IO.Path.GetFileNameWithoutExtension(_)), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fileName = System.IO.Path.Combine(Path,ComboBox.SelectedItem.ToString()+".xaml");
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    Application.Current.Resources.BeginInit();
                    // Read in ResourceDictionary File
                    ResourceDictionary dic = (ResourceDictionary)XamlReader.Load(fs);
                    // Clear any previous dictionaries loaded
                    Application.Current.Resources.MergedDictionaries.Clear();
                    // Add in newly loaded Resource Dictionary
                    Application.Current.Resources.MergedDictionaries.Add(dic);
                    Application.Current.Resources.EndInit();
                }
            }
        }
  


    }
}
