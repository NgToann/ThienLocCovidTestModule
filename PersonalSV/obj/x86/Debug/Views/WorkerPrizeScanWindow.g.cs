﻿#pragma checksum "..\..\..\..\Views\WorkerPrizeScanWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "107B2300ABA7E6022A81789A8CF98D32C7D6FFA807ACE4302771F05F96F5B793"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
using System.Windows.Forms.Integration;
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
using TLCovidTest.Views;


namespace TLCovidTest.Views {
    
    
    /// <summary>
    /// WorkerPrizeScanWindow
    /// </summary>
    public partial class WorkerPrizeScanWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblHeader;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtCardId;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border brDisplay;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grDisplay;
        
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
            System.Uri resourceLocater = new System.Uri("/TLCovidTest;component/views/workerprizescanwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
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
            
            #line 7 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
            ((TLCovidTest.Views.WorkerPrizeScanWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblHeader = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.txtCardId = ((System.Windows.Controls.TextBox)(target));
            
            #line 35 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
            this.txtCardId.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.txtCardId_GotKeyboardFocus);
            
            #line default
            #line hidden
            
            #line 36 "..\..\..\..\Views\WorkerPrizeScanWindow.xaml"
            this.txtCardId.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.txtCardId_PreviewKeyUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.brDisplay = ((System.Windows.Controls.Border)(target));
            return;
            case 5:
            this.grDisplay = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
