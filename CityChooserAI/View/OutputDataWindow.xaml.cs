﻿using CityChooserAI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CityChooserAI.View
{
    /// <summary>
    /// Interaction logic for OutputDataWindow.xaml
    /// </summary>
    public partial class OutputDataWindow : Window
    {
        public OutputDataWindow(City city, string continent)
        {
            InitializeComponent();
            
            //SetInfo(); //TODO
        }
        private string _cityContinent;
        public string cityContinent
        {
            get { return _cityContinent; }
            set { _cityContinent = value; }
        }

    }
}
