   M S B A M L   `   `   `  ����T  PAvalonDock, Version=1.3.3571.0, Culture=neutral, PublicKeyToken=85a1e0ada7ec13e4;,clr-namespace:AvalonDock;assembly=AvalonDock
AvalonDock  5       MSImageView0   *Novartis.Msi.MSImageView.ImagePropsContent   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;ad,clr-namespace:AvalonDock;assembly=AvalonDock     5      -   5            AvalonDock.DockableContent   Name$  
imageProps��5         AvalonDock.ManagedContent   Title$ Image Properties��63   
�������   DockableStyle$ DockableToBorders=�6_      IsCloseable .  .��5      � -   5         �Name$ 
LayoutRoot��5      .+�5   
   �� -   5         ��Name$ scrollViewer��5      $
��Auto=�6-   $
��Auto=�6R   .��5      �� -   5         ��Name$ 
stackPanel��5      .+�5      �� $��	color map��6   $��18��6-   $��4,4,4,0q�69   $��Center=�6J   $
��Left=�6e   �� 5      -   5         ��Name$ cbImageRepresentation��5   '   $��23��6   $��4,4,4,4q�6F   $	��240��6�   $
��Left=�6�   $	��Top=�6�   �� 5      $	��240��6   $��4,4,4,4q�6(   $
��Left=�69   �� 5       �� Orientation$ 
Horizontal=�6   $��Stretch=�66   .+�5      �� $��Current Max. Intensity��6    $��23��6>   $��4,4,4,4q�6J   $��Center=�6[   $
��Left=�6v   �� 5      -   5        $��maxIntensity��5   *   $��23��6   $��8,4,4,4q�6>   $��Center=�6O   $��Right=�6j   $	��100��6�   6�   5      �� 5      -   5        	 ��Name$	 slMaxIntensity��5   %   $��23��6   $��4,4,4,4q�6=   $	��240��6x   $
��Left=�6�   $	��Top=�6�   �� 5      $	��240��6   $��4,4,4,4q�6(   $
��Left=�69   �� 5       $ 
Horizontal=�6   $��Stretch=�66   .+�5!      �� $��Current Min. Intensity��6    $��23��6>   $��4,4,4,4q�6J   $��Center=�6[   $
��Left=�6v   �� 5"      -   5        $��minIntensity��5"   *   $��23��6   $��8,4,4,4q�6>   $��Center=�6O   $��Right=�6j   $	��100��6�   6�   5#      �� 5$      -	   5        $	 slMinIntensity��5$   %   $��23��6   $��4,4,4,4q�6=   $	��240��6x   $
��Left=�6�   $	��Top=�6�   �� 5%      $	��240��6   $��4,4,4,4q�6(   $
��Left=�69   �� 5&      $ 
Horizontal=�6   $��Stretch=�66   $	��186��6T   .+�5'      �� -
   5        $��BrowseMassTB��5'       $��Browse Mass Range��64   $��23��6M   $��
4,-140,4,4q�6Y   $
��Bold"�6m   �� 5(      -   5        $��currMassRangeTB��5(       $��Current Mass [m/z]��67   $��23��6Q   $��-115,-90,0,0q�6]   �� 5)      -   5        $��currMass��5)      $��23��6.   $��
10,-90,2,4q�6:   $	��100��6N   6�   �� 5*      -   5        $	 
slCurrMass��5*   )   $��23��6   $��-230,-30,4,4q�6=   $	��240��6S   6_   �� 5+      -   5        $��MinMassLabel��5+      $��-245,85,185,75q�60   $��	Min [m/z]��6H   �� 5,      -   5        $��MaxMassLabel��5,      $��-70,85,5,75q�60   $��	Max [m/z]��6E   �� 5-      -   5        $��
ShowTICBtn��5-   B   $��Show Tic Image��6   $��23��66   $��95��6T   $��-385,80,5,25q�6_   6u   5.      5/      50      51      52      