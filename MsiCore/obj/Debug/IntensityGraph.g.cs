﻿#pragma checksum "..\..\IntensityGraph.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "34825A20AA2719280DCEFA842A53423E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Visifire.Charts;


namespace Novartis.Msi.Core {
    
    
    /// <summary>
    /// IntensityGraph
    /// </summary>
    public partial class IntensityGraph : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\IntensityGraph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\IntensityGraph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Visifire.Charts.Chart IntensityChart;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MsiCore;component/intensitygraph.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IntensityGraph.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\IntensityGraph.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.CloseButtonClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.IntensityChart = ((Visifire.Charts.Chart)(target));
            
            #line 23 "..\..\IntensityGraph.xaml"
            this.IntensityChart.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.IntensityChartMouseRightButtonDown);
            
            #line default
            #line hidden
            
            #line 23 "..\..\IntensityGraph.xaml"
            this.IntensityChart.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.IntensityChartMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 23 "..\..\IntensityGraph.xaml"
            this.IntensityChart.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.IntensityChartMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

