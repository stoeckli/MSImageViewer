   M S B A M L   `   `   `  ����F  BMSImageView, Version=2.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Novartis.Msi.MSImageViewNovartis.Msi.MSImageView  5   	   T PAvalonDock, Version=1.3.3571.0, Culture=neutral, PublicKeyToken=85a1e0ada7ec13e4;,clr-namespace:AvalonDock;assembly=AvalonDock
AvalonDock 5   	   G CExtResources, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null_?clr-namespace:Novartis.Utils.ExtResources;assembly=ExtResourcesNovartis.Utils.ExtResources 5   	   )    #Novartis.Msi.MSImageView.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   8local&clr-namespace:Novartis.Msi.MSImageView     5   	   ;ad,clr-namespace:AvalonDock;assembly=AvalonDock     5   	   Rextres?clr-namespace:Novartis.Utils.ExtResources;assembly=ExtResources     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          � Title$  MSImageView��5   	    � Icon$! Images\MS ImageViewer32.png��6   $
��1080��6@    � 
ResizeMode$ CanResizeWithGrip=�5   	    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$ ApplicationCommands.Open��5      6<     5   
   -   5        $  ApplicationCommands.SaveAs��5      6U     5   
   -   5        $" local:MainWindow.ExitCommand��5      6@     5   
   -   5        $# local:MainWindow.AboutCommand��5      6A     5   
   -   5        $, &local:MainWindow.ToggleToolTipsCommand��5      6J     5   
   -   5        $. (local:MainWindow.UseApproximationCommand��5      6L     5    
   -   5        $$ local:MainWindow.NewRoiCommand��5       6B     5!   
   -	   5        $% local:MainWindow.OpenRoiCommand��5!      6C     5"   
   -
   5        $% local:MainWindow.SaveRoiCommand��5"      6C     5#   
   -   5        $' !local:MainWindow.SaveRoiAsCommand��5#      6E     5$   
   -   5        $&  local:MainWindow.CloseRoiCommand��5$      6D    � 	Resources 5(      .  (Novartis.Utils.ExtResources.ExtResources   ExtResources��%   &	          5*   
   6   5+       � DataContext 5.      �� 50   
    �� Source# [  6   .��53      � -   5         �Name$ 
LayoutRoot��53      	 � RowDefinitions	 54   
   �� 55      $
��Auto �6   �� 56      $
��Auto �6   �� 57      �� 58      $
��Auto �6   .+�5:   
   o� $	��Top=�6   $��0��6'   .@�5;      m� -   5        
 m�Name$
 FileMenu��5;   &   $��_File��6   .@�5<      m� -   5        $
 FileOpen��5<   -   $��_Open...��6    m� Command$ ApplicationCommands.Open��6?    m� InputGestureText$ Ctrl+ O��6b   m� 5=      -   5        $
 
FileSaveAs��5=   0   $��Save _As...��6   $  ApplicationCommands.SaveAs��6D   $ Ctrl+ S��6i   �� 5>      m� 5?      -   5        $
 FileExit��5?   *   $��_Exit��6   $" local:MainWindow.ExitCommand��6<   $ Alt+ F4��6c   5@      m� 5A      -   5        $
 ROIMenu��5A   %   $
��_Roi��6   .@�5B      m� -   5        $
 RoiNew��5B   0   $��_New Roi...��6   $$ local:MainWindow.NewRoiCommand��6@   $ Ctrl+ N��6i   m� 5C      -   5        $
 RoiOpen��5C   1   $��_Open Roi...��6   $% local:MainWindow.OpenRoiCommand��6B   $ Ctrl+ O��6l   �� 5D      m� 5E      -   5        $
 RoiSave��5E   1   $��_Save Roi...��6   $% local:MainWindow.SaveRoiCommand��6B   $ Ctrl+ S��6l   m� 5F      -   5        $
 	RoiSaveAs��5F   5   $��_Save Roi _As...��6   $' !local:MainWindow.SaveRoiAsCommand��6H   $ Ctrl+ S��6t   �� 5G      m� 5H      -   5        $
 RoiClose��5H   .   $��	_Close...��6   $&  local:MainWindow.CloseRoiCommand��6@   $ Ctrl+ C��6k   �� 5I      5J      m� 5K      -   5        $
 ViewMenu��5K   &   $��_View��6   .@�5L      m� -   5        $
 ShowToolTips��5L   3   $��Show _ToolTips��6   $, &local:MainWindow.ToggleToolTipsCommand��6I    m� IsCheckable .  m� 	IsChecked .  6�   m� 5M      -   5        $
 ShowStatusBar��5M   4   $��Show _Statusbar��6    .  .  6p   m� 5N      -   5        $
 ViewIntensityGraph��5N   9   $��View Intensity Graph��6    .  .  6z   5O      m� 5P      -   5        $
 SettingsMenu��5P   *   $��	_Settings��6   .@�5Q      m� -   5        $
 UseApproximation��5Q   J   $+��%Use _Approximation during Wiff-Import��6   $. (local:MainWindow.UseApproximationCommand��6d    .  .  6�   5R      m� 5S      -   5        $
 HelpMenu��5S   &   $��_Help��6   .@�5T      m� -   5        $
 	HelpAbout��5T   8   $��_About MSImage View��6   $# local:MainWindow.AboutCommand��6K   5U      5V      5W      `� 5Y   
   -    5         `�Name$ toolBarTray��5Y      $	��Top=�6+   $��1��6C   .�5[      c� -!   5         c�Name$ tbFile��5[      $��28��6&   .@�5\      �� -"   5         ��Name$ tbbOpen��5\       � ToolTip$ Open file...��6<   .��5]      �� ��6   ��  �� Path$ FileOpen256�5^      �� 5_      -#   5        $ 	tbbSaveAs��5_      $ Save current view as...��6@   .��5`      �� ��6   �� $ FileSaveAs256�5a      5b      5d        AvalonDock.DockingManager  5f   
   -$   5          Name$ dockingManager��5f      $��2��64   .��5g        AvalonDock.ResizingPanel  -%   5          Name$ resizingPane��5g         Orientation$ 
Horizontal=�65   .+�5h        AvalonDock.DocumentPane  -&   5          Name$ documentPane��5h   "   5i        AvalonDock.DockablePane  5j      -'   5          Name$ dockablePane��5j   "      ResizeWidth$	 262 �68   .@�5k      0   *Novartis.Msi.MSImageView.ImagePropsContent  -(   5          Name$ imagePropsContent��5k   .   /   )Novartis.Msi.MSImageView.RoiProjectWindow  5l      -)   5          Name$ roiProjectWindow��5l   -   *	   $Novartis.Msi.MSImageView.MetaContent	  5m      -*   5         	 Name$ metaContent��5m   (   5n      5o      5p      �� 5r   
   -+   5         ��Name$ 	statusBar��5r      $��3��6'   ��5s      �� 5t      .R�5u      � 	 5v      �� 5w      $��* �6,    � ColumnDefinitions 5y      �� 5z      $
��Auto �6/   �� 5{      $
��Auto �6/   �� 5|      $
��Auto �6/   �� 5}      $
��Auto �6/   �� 5~      $��10 �6/   �� 5      $��* �6/   �� 5�      $
��Auto �6/   �� 5�      $
��Auto �6/   �� 5�      $��15 �6/   5�      5�      .@�5�      �� $��0��6   .��5�      �� -,   5         ��Name$ coordinatesLabel��5�      $��0,0,2,0q�66   $y�Hidden=�6G   	Position:6f   5�      �� 5�      $��1��6   .��5�      �� --   5        $ coordinates��5�      $y�Hidden=�61   x = 123.4 mm, y = 456.7 mm6a   5�      �� 5�      $��2��6   .��5�      �� -.   5        $ intensityLabel��5�      $��0,0,2,0q�64   $y�Hidden=�6E   
Intensity:6e   5�      �� 5�      $��3��6   .��5�      �� -/   5        $ 	intensity��5�      $y�Hidden=�6/   10006I   5�      �� 5�      $��5��6   .��5�      �� -0   5        $ infoText��5�      60   5�      �� 5�      $��6��6   .��5�      �� -1   5        $ progressOperation��5�      $y�Hidden=�67   6M   5�      �� 5�      $��7��6   .��5�      � -2   5          �Name$  progressBar��5�      ! � Value$! 0Q�63   $��80��6=   $��15��6H   $y�Hidden=�6T   5�      5�      5�      5�      