   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5   9   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType �� Property   GradientAnimated �� Value  ProgressBarBackground  ProgressBarBorder �� 
TargetType �Name J� SnapsToDevicePixels �� RadiusX	 �� RadiusY
 ��Name {�Name � ColumnDefinitions � RowDefinitions ��Name  ProgressBarIndicatorBrush J� OpacityMask `� 	Converter<    6Microsoft.Windows.Themes.ProgressBarHighlightConverter $  ProgressBarIndicatorAnimatedFill �� Source �� Path �� ElementName ��Name  ProgressBarValueForeground � Value �� Triggers P� Property P� Value �� 
TargetName �� Angle�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      gMicrosoft_Windows_ThemesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  69     5      �� 5   
   $ Common.xaml?�6   %r	  '�       7   7  7  7  7   7  7  7  7   �� 5	      " �6   .�5
   
   ��  � ��8   6'   �� 5   
    � ��8  6'   �� 5   
    � ��8  6(   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��$ 18��6&   �� 5   
    � ��$ 5q�6#   �� 5   
    � �� 5      �� 5      " �6"   .R�5      � $ 
Background��66    . .+�5      �� #��z��6$    . $ 3��6c   $	 3��6o   �� 5      $
 
PART_Track��6/   $��1q�6$    . $ 3��6^   $	 3��6j   {� 5      $ PART_Indicator��6J   $
��Left=�6$   $��1q�6?   ._�5      � $ 
Foreground��6#    5   "   �� 5   &   $��15��67   �� 5   &   $
��0.1* �67   �� 5   &   $��15��67    5   "   �� 5   &   �� 5   &   .+�5!   "   �� $ 	Indicator��6)    . $��3��6W   $��2��6k   8�� $��0q�6�   �� 5"   "   $
 	Animation��6,    . $��3��6[   $��2��6o   8�� $ 2��6�   $	 2��6�    5#   &   `� 5$   *    5%   .      5&   2   .1�5(   .   �� 8  66   �� 5)   .   $ ActualWidth�66   $ 
Background��6I   �� 5*   .   $ ActualHeight�66   $ 
Background��6J   5+   +   5-   #   5.      5/      �� 50       . #��z��6<   #��z��6h   � 51      $ 	GridValue��6   $��Center=�62   $��Center=�6O    52      �� 53   "   $
��Auto �63   �� 54   "   $
��Auto �63   .+�56      �� $ 
LabelValue��6$   $��0��68   $��0q�6H   8�� #��z 6�   �� 57      $ LabelPercent��6$   $��1��6:   $��0q�6J   8�� $��%��6�   58      59       5:      P� 5;       �@�Orientation$ Vertical=�69   . �5<      �� $ 
Background��6@    �@�LayoutTransform 5=   "   �� 5>   &   $	 -90Q�66   5@      �� 5A      $ 
PART_Track��6@    �@�LayoutTransform 5B   "   �� 5C   &   $ 90Q�66   5E      �� 5F      $ PART_Indicator��6@    �@�LayoutTransform 5G   "   �� 5H   &   $ 90Q�66   5J      �� 5K      $ 
Foreground��6@    �@�LayoutTransform 5L   "   �� 5M   &   $	 -90Q�66   5O      5P      P� 5Q       �@�IsIndeterminate . . �5R      �� $ 
LabelValue��6;    � y�$ 	Collapsed=�6S   �� 5S      $ LabelPercent��6;    � y�$ 	Collapsed=�6U   5T      P� 5U       �@�IsIndeterminate .  . �5V      �� $ 	Animation��65   8   � ��6L   5W      5Y      5[      5\      5]      