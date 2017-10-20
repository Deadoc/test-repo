﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UPSBatteryController.Views;

namespace UPSBatteryController.Presentation.Controls
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    [Export(typeof(IServerSettingsView)), PartCreationPolicyAttribute(CreationPolicy.NonShared)]
    public partial class ServerSettingsView : IServerSettingsView
    {
        public ServerSettingsView()
        {
            InitializeComponent();
        }
    }
}