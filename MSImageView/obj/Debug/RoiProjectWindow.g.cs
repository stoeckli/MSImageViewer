﻿#pragma checksum "..\..\RoiProjectWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A173E9DB6C32A75F7F266F3CDEAFE447"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonDock;
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


namespace Novartis.Msi.MSImageView {
    
    
    /// <summary>
    /// RoiProjectWindow
    /// </summary>
    public partial class RoiProjectWindow : AvalonDock.DockableContent, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Novartis.Msi.MSImageView.RoiProjectWindow roiprojectwindow;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView RoiProjectTreeView;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox RoiPropertiesGroupBox;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ImagePathlbl;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Arealbl;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MeanIntensitylbl;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ImagePathTB;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AreaTB;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MeanIntTB;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImagePathBtn;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\RoiProjectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ViewImagesAllBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/MSImageView;component/roiprojectwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\RoiProjectWindow.xaml"
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
            this.roiprojectwindow = ((Novartis.Msi.MSImageView.RoiProjectWindow)(target));
            
            #line 21 "..\..\RoiProjectWindow.xaml"
            this.roiprojectwindow.Loaded += new System.Windows.RoutedEventHandler(this.RoiProjectWindowLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.RoiProjectTreeView = ((System.Windows.Controls.TreeView)(target));
            
            #line 24 "..\..\RoiProjectWindow.xaml"
            this.RoiProjectTreeView.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.RoiProjectTreeViewSelectedItemChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.RoiPropertiesGroupBox = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 5:
            this.ImagePathlbl = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.Arealbl = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.MeanIntensitylbl = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.ImagePathTB = ((System.Windows.Controls.TextBox)(target));
            
            #line 30 "..\..\RoiProjectWindow.xaml"
            this.ImagePathTB.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ImagePathTbTextChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.AreaTB = ((System.Windows.Controls.TextBox)(target));
            
            #line 31 "..\..\RoiProjectWindow.xaml"
            this.AreaTB.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AreaTbTextChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.MeanIntTB = ((System.Windows.Controls.TextBox)(target));
            
            #line 32 "..\..\RoiProjectWindow.xaml"
            this.MeanIntTB.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.MeanIntTbTextChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.ImagePathBtn = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\RoiProjectWindow.xaml"
            this.ImagePathBtn.Click += new System.Windows.RoutedEventHandler(this.ImagePathBtnClick);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ViewImagesAllBtn = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\RoiProjectWindow.xaml"
            this.ViewImagesAllBtn.Click += new System.Windows.RoutedEventHandler(this.ViewAllImagesBtnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

