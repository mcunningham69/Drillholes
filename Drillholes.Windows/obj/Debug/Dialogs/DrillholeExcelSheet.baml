   M S B A M L   `   `   `  ����M  IDrillholes.Windows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Drillholes.Windows.DialogsDrillholes.Windows.Dialogs  5   	   4    .Drillholes.Windows.Dialogs.DrillholeExcelSheet   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:Drillholes.Windows.Dialogs     5   	     � Title$  Import Excel Tables��5   	   $	��400��6%   $	��350��65   $	��300��6B    � 
ResizeMode$ NoResize=�6N   
����İ� � WindowStyle$ SingleBorderWindow=�6�    � 	Resources 5
       �� 
TargetType �� Property �� Value �� Triggers P� Property	 P� Value   
GreenBrush
 �� 
StartPoint �� EndPoint � Color � Offset  	BlueBrush  	SizeStyle �� BasedOn��%F  '��       &	      &	 �    &	 u    '�� P    7  7  '�� �    �� 5   
   " ��6   .�5      ��  � ��$
 Left=�64   �� 5       � ��$ Center=�62   �� 5       � ��$	 500��6&   �� 5       � ��$ 18(�6)    5      P� 5       � ��$	  ��6*   . �5      ��  � ��
 �����63   5      5      �� 5   
   $	
 0,0&�61   $	 1,0&�6B   .��5      � $ Green��6   $ 0Q�6)   � 5      $
 Lime��6   $	 0.3Q�6(   � 5      $ Transparent��6   $	 0.8Q�6/   5      �� 5   
   $	
 0,0&�60   $	 1,0&�6A   .��5      � $ 	LightBlue��6   $ 0Q�6-   � 5      $ White��6   $ 1Q�6)   5       �� 5!   
   " �6"   .�5"      ��  � ��$	 250��6&   �� 5#       � ��$ 50��6'   �� 5$       � ��$
 Left=�64   �� 5%       � ��$	 Top=�62   �� 5&       � ��$ 6q�6'   5'      �� 5(   
   " ��6   8   .�5)      ��  � ��8  6+   �� 5*       � ��$
 Bold"�6+   �� 5+       � ��$	 120��6&    5,      P� 5-       �@J�IsMouseOver	 . . �5.      ��  � ��$	 105��6.   �� 5/       � ��$	 200��6/   �� 50       � ��$ 18(�61   �� 51       � ��$
 Bold"�63   52      54      �� 55   
   " ��6   .�56      ��  � ��$ 18(�6)   57      58      .��5:      �  � ColumnDefinitions 5;   
   �� 5<      $��10 �6   �� 5=      $��* �6   �� 5>      $��10 �6    � RowDefinitions 5@   
   �� 5A      $��20 �6   �� 5B      $��30 �6   �� 5C      $	��180 �6   �� 5D      $��50 �6   �� 5E      $��10 �6   .+�5H   
   �� $!��Available Excel Worksheets:��6   $
��Bold"�66   $��1��6H   $��1��6X   $��Bottom=�6e   �� 5I   
   -   5         ��Name$ 	btnCancel��5I      $��Cancel��6$   $��1��65   $��3��6E   $
��Auto��6R   $��25��6_   $
��Left=�6k   6�   �� 5J   
   -   5        $ 	btnSelect��5J      $��Select Excel Worksheet��6$   $��1��6E   $��3��6U   $
��Auto��6b   $��25��6o   $��Right=�6{   6�   �� 5K   
   -   5        $��	lstSheets��5K      
�������$��1��6:   $��2��6J   $
��Auto��6W   $
��Auto��6d   $��Stretch=�6r   $��Stretch=�6�   5M      5N      5O      