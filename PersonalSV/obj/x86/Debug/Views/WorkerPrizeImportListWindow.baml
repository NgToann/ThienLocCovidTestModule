   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   3    -TLCovidTest.Views.WorkerPrizeImportListWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   $��15(�67     � WindowState$  	Maximized=�5       � WindowStartupLocation$ CenterScreen=�62    $   workerPrizeImportListWindowTitle � Title# �   5   	   $	��450��6P   $	��800��6]   .��5	      � $��2q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ btnImportExcel��5   8   $	��200��6   $
��Left=�6     menuCreateNewImportExcel#���  6P   $��	10,4,10,4q�6�   $��0,0,0,10q�6�    � Cursor$
 Hand��6�   
���  ��$��SemiBold"�6�   6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dgWorkerPrize��5   4   $��2��6   $��2��6       AutoGenerateColumns .   �� AlternationCount$ 2��6g   	   AlternatingRowBackground
	 �����
   CanUserAddRows
 .     SelectionUnit$ FullRow=�5   .      HorizontalGridLinesBrush
 �����   VerticalGridLinesBrush
 �����   
IsReadOnly . $��Arial)�5   )   $��15(�6<      FrozenColumnCount$ 3��6J      RowHeaderWidth$	 NaNQ�6`    � 	Resources 5      *  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%�   '        �� 5      "  6    5      P� 5       �@ 
IsSelected . . �5      ��  � ��
 ����6;   �� 5       � ��
 �����6;   5      5      5         ColumnHeaderStyle 5      �� 5       =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5!      ��  � ��$ Center=�6C   �� 5"       � ��$ 16(�61   �� 5#       � ��  DataGridRowHeaderBG# �  63   �� 5$       � ��
 �����64   �� 5%       � ��$ 0.5,0.5,0.5,0q�68   �� 5&       � ��$ 5q�60   5'         Columns 5)      4  .System.Windows.Controls.DataGridTemplateColumn  5*        commonEmployeeID,  &System.Windows.Controls.DataGridColumn   Header# �  6)      SortMemberPath$ WorkerId��6a      ClipboardContentBinding 6{   �� *
WorkerId+   CellTemplate 5+      �� 5,      .R�5-      �� $��3q�6B   $��Center=�6M   $��Center=�6h   ��6(   �� *
WorkerId+6h   5.      50        51        commonEmployeeCode# �  6)   $ CardId��6c    6{   �� *CardId+ 52      �� 53      .R�54      �� $��3q�6@   $��Center=�6K   $��Center=�6f   ��6(   �� *CardId+6f   55      57        58        commonEmployeeName# �  6)   $ FullName��6c    59      �� 5:      .R�5;      �� $��3q�6B   $��Center=�6M   ��6(   �� *
FullName+6M   5<      5>        5?        commonEmployeeDepartment# �  6)   $ DepartmentName��6i    6�   �� *DepartmentName+ 5@      �� 5A      .R�5B      �� $��3q�6H   $��Center=�6S   ��6(   �� *DepartmentName+6S   5C      5E      5G      �� 5H   
   -   5        $ btnSave��5H      $��3��6     ButtonTemplate#���  6/   $
 Hand��6}   $	��200��6�   $��Right=�6�   $��0,5,0,0q�6�   ��5I      � 5J       5K      �� 5L      $
��auto �6(   �� 5M      $
��auto �6(   .+�5O      ��  �� Padding$ 	25,4,25,4q�6    $��1��64   $��Center=�6A     commonButtonSave#���  6\   5P      5R      5S      5T      