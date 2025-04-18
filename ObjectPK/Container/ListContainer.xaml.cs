﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for ListContainer.xaml
    /// </summary>
    public partial class ListContainer : UserControl
    {
        public ObjectInfo objectInfo { get; private set; }
        public ListContainer(ObjectInfo objectInfo)
        {
            DataContext = this;
            this.objectInfo = objectInfo;
            InitializeComponent();
        }
    }
}
