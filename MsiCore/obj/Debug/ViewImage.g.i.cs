﻿#pragma checksum "..\..\ViewImage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CB17245EA9EED531709F352D63CA3E9A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Novartis.Msi.Core;
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


namespace Novartis.Msi.Core {
    
    
    /// <summary>
    /// ViewImage
    /// </summary>
    public partial class ViewImage : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Novartis.Msi.Core.ViewImage view2D;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border border;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas drawingArea;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imageCtrl;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu ROIContextMenu;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuNew;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuClose;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuFix;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuDelete;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuEdit;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuViewRoiData;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\ViewImage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuZoom;
        
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
            System.Uri resourceLocater = new System.Uri("/MsiCore;component/viewimage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ViewImage.xaml"
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
            this.view2D = ((Novartis.Msi.Core.ViewImage)(target));
            
            #line 19 "..\..\ViewImage.xaml"
            this.view2D.Loaded += new System.Windows.RoutedEventHandler(this.ViewImageLoaded);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ViewImage.xaml"
            this.view2D.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ViewImageMouseWheel);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ViewImage.xaml"
            this.view2D.KeyDown += new System.Windows.Input.KeyEventHandler(this.ViewImageKeyDown);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ViewImage.xaml"
            this.view2D.KeyUp += new System.Windows.Input.KeyEventHandler(this.ViewImageKeyUp);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ViewImage.xaml"
            this.view2D.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ViewImageLeftMouseDown);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ViewImage.xaml"
            this.view2D.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.WindowPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 24 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdNewRoi);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 25 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdFixRoi);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 26 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdCloseRoi);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 27 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdDeleteRoi);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 28 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdEditRoi);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 29 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdZoom);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 30 "..\..\ViewImage.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnCmdViewRoiDataCommand);
            
            #line default
            #line hidden
            return;
            case 9:
            this.border = ((System.Windows.Controls.Border)(target));
            return;
            case 10:
            this.drawingArea = ((System.Windows.Controls.Canvas)(target));
            
            #line 46 "..\..\ViewImage.xaml"
            this.drawingArea.Loaded += new System.Windows.RoutedEventHandler(this.DrawingAreaLoaded);
            
            #line default
            #line hidden
            
            #line 46 "..\..\ViewImage.xaml"
            this.drawingArea.SizeChanged += new System.Windows.SizeChangedEventHandler(this.DrawingAreaSizeChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.imageCtrl = ((System.Windows.Controls.Image)(target));
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseEnter += new System.Windows.Input.MouseEventHandler(this.ImageCtrlMouseEnter);
            
            #line default
            #line hidden
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseLeave += new System.Windows.Input.MouseEventHandler(this.ImageCtrlMouseLeave);
            
            #line default
            #line hidden
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseMove += new System.Windows.Input.MouseEventHandler(this.ImageCtrlMouseMove);
            
            #line default
            #line hidden
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ImageCtrlMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.ImageCtrlMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 48 "..\..\ViewImage.xaml"
            this.imageCtrl.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ImageCtrlMouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ROIContextMenu = ((System.Windows.Controls.ContextMenu)(target));
            return;
            case 13:
            this.mnuNew = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 14:
            this.mnuClose = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 15:
            this.mnuFix = ((System.Windows.Controls.MenuItem)(target));
            
            #line 53 "..\..\ViewImage.xaml"
            this.mnuFix.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MenuItemMouseEnter);
            
            #line default
            #line hidden
            return;
            case 16:
            this.mnuDelete = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 17:
            this.mnuEdit = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 18:
            this.mnuViewRoiData = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 19:
            this.mnuZoom = ((System.Windows.Controls.MenuItem)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

