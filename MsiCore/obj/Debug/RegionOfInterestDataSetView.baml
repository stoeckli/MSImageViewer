   M S B A M L   `   `   `  ����  MsiCore3    -Novartis.Msi.Core.RegionOfInterestDataSetView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        $��ROIDataView��5   	     � Title$&   Region Of Interest Data Set View��5   	   $	��400��62   $	��550��6?    � DataContext 5   	   ��  �� RelativeSource �� *Self+.��5      � .+�5	   
   �� $ �� IsSynchronizedWithCurrentItem$
 TrueY�6Y   $��7,9,5,5q�6~   ��6   �� *ObservableRoiObjects+ �� ElementName$ ROIDataView����5
      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value$ Center=�6C   �� 5       � ��$ 12(�6=   �� 5       � ��
 �   �6?   5       �� View 5      �� 5      P�5      �� 5      $��Region of Interest��6)   	 �� Width$		 150��6E   
 �� DisplayMemberBinding
 6Q   �� *Title+�� 5      $
��Area��6)   $		 080��67   
 6C   �� *Area+ �� StringFormat$ 0.0000���� 5      $��Mean Intensity��6)   $		 130��6A   
 6M   �� *MeanIntensity+�� 5      $��Number of points in ROI��6)   $		 150��6J   
 6V   �� *NumberOfPointsInsideRoi+5      5      5      5      