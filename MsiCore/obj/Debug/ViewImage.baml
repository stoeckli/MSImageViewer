   M S B A M L   `   `   `  ����B  >MsiCore, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Novartis.Msi.CoreNovartis.Msi.Core  5      !    Novartis.Msi.Core.ViewImage   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      1localclr-namespace:Novartis.Msi.Core     5      -   5        $��view2D��5   3   
�������}�.  |�. $��Stretch=�5      $��Stretch=�6#     J� CommandBindings  5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$# local:ViewImage.NewRoiCommand��5      6A     5   
   -   5        $# local:ViewImage.FixRoiCommand��5      6A     5   
   -   5        $% local:ViewImage.CloseRoiCommand��5      6C     5   
   -   5        $&  local:ViewImage.DeleteRoiCommand��5      6D     5   
   -   5        $$ local:ViewImage.EditRoiCommand��5      6B     5   
   -   5        $! local:ViewImage.ZoomCommand��5      6?     5   
   -   5        $( "local:ViewImage.ViewRoiDataCommand��5      6F    � 	Resources 5!       �� MergedDictionaries �� Source�� 5"   
    5#      �� 5$      $ Templates/BasicShape.xaml?�6%   %    5&      .��5)      �� 
�������$��	5,5,10,10q�6     J� BitmapEffect 5*   
   F� 5+       F� Opacity$	 0.7Q�6%    F� 	Direction$	 270Q�63   ._�5-   
   �� -	   5        $��border��5-      ���	#FFF0F8FE}�. ._�5.      �� -
   5        $��drawingArea��5.      }�.  $��Stretch=�6X   $��Stretch=�6t   .+�5/      �� -   5        $��	imageCtrl��5/      $y�Visible=�50      $��Stretch=�61   $��Stretch=�6O   ��5/   )   ��  �� ElementName$ drawingArea��	 �� Path$	 ActualWidth���6e   �� $ drawingArea��$	 ActualHeight�50   .  
 � ContextMenu
 51      �� 52      -   5        $��ROIContextMenu��52   &   .@�53      m� -   5        $��mnuNew��53   '   $��New Roi��66    m� Command$# local:ViewImage.NewRoiCommand��6G   {�.  6o   m� 54      -   5        $��mnuClose��54   '   $��	Close Roi��67   $% local:ViewImage.CloseRoiCommand��6J   {�.  6t   m� 55      -   5        $��mnuFix��55   '   $��Fix Roi��65   $# local:ViewImage.FixRoiCommand��6F   {�.  6�   �� 56      m� 57      -   5        $��	mnuDelete��57   '   $��
Delete Roi��68   $&  local:ViewImage.DeleteRoiCommand��6L   {�.  6w   m� 58      -   5        $��mnuEdit��58   '   $��Edit Roi��66   $$ local:ViewImage.EditRoiCommand��6H   {�.  6q   �� 59      m� 5:      -   5        $��mnuViewRoiData��5:   '   $��View Roi Data��6=   $( "local:ViewImage.ViewRoiDataCommand��6T   {�.  6�   �� 5;      m� 5<      -   5        $��mnuZoom��5<   '   $
��Zoom��66   $! local:ViewImage.ZoomCommand��6D   {�. 6j   5=      5?      5@      5A      5B      