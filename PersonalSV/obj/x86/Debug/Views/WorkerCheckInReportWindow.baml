   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   1    +TLCovidTest.Views.WorkerCheckInReportWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5        $��14(�5        � WindowStartupLocation$  CenterScreen=�6(    � WindowState$ 	Maximized=�6M      clinicReportWindowTitle � Title# �   5   	   $	��450��6G   $	��800��6T   .��5	      � $��2q�6    � RowDefinitions 5
   
   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   ��   GroupBoxTemplate#���  6   ��5      �� 5        Background1#���  6     BorderStyle#���  6P   ._�5      �� -   5         ��Name$ txtGroupSummaryHeader��5   4    �� Padding$ 	10,0,10,0q�6      reportCheckInOutOption#���  6S   5      .��5      �  5      �� 5      �� 5      �� 5       5      �� 5       �� 5!      .+�5#      �� -   5         ��Name$ txtFindWhat��5#      $��20(�6/   $��3q�6=   $��Center=�6I   $	��240��6�   � 5$      $��1��6    5%      �� 5&      $
��auto �6(   �� 5'      $
��auto �6(   �� 5(      $
��auto �6(   �� 5)      $
��auto �6(    5+      �� 5,      $
��auto �6+   �� 5-      $��10 �6+   �� 5.      $
��auto �6+   �� 5/      $��10 �6+   �� 50      $
��auto �6+   �� 51      $��10 �6+   �� 52      $
��auto �6+   .+�54      ��   commonDatePickerFrom#���  6    �� 55        commonDatePickerTo#���  6    $��2��6X   (  "System.Windows.Controls.DatePicker  56      -   5          Name$ dpFrom��56   !   $��1��61   $��0,2,0,0q�6>   $��Center=�6O     57      -   5        $
 dpTo��57   !   $��1��6/   $��2��6<   $��0,2,0,0q�6L   $��Center=�6]   58      �� 59      -   5        	 ��Name$	 
btnPreview��59      $��5,0,0,0q�61     ButtonTemplate#���  6B   $��3��6z   $��5��6�   {�.  ��5:      � 5;       5<      �� 5=   "   $
��auto �60   �� 5>   "   $
��auto �60   .+�5@      �� $��16��6$   $��16��6/   $��/Images/update.png��6;   �� 5A      $��2,0,0,0q�6(   $��1��69   $��Center=�6F     commonButtonPreview#���  6a   5B      5D      5E      5F      �� 5G   
   $��1��6   $��2��6   $��0,5,0,0q�62   ._�5H      &   System.Windows.Controls.DataGrid  -   5        
  Name$
 dgReport��5H         AutoGenerateColumns .   �� AlternationCount$ 2��6E      AlternatingRowBackground
 �����   ClipboardCopyMode$ IncludeHeader=�6      CanUserAddRows .     SelectionUnit$ FullRow=�5I   .      HorizontalGridLinesBrush
 �����   VerticalGridLinesBrush
 �����$��15(�6�      
IsReadOnly .    FrozenColumnCount$ 3��5J   )   $��Cabri)�6?      RowHeaderWidth$	 NaNQ�6R    � 	Resources 5K      *  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%�   '        �� 5L      "  6    5M      P� 5N       �@ 
IsSelected . . �5O   "   ��  � ��
 ����6?   �� 5P   "    � ��
 �����6?   5Q      5S      5T         ColumnHeaderStyle 5U      �� 5V      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5W      ��  � ��$ Center=�6G   �� 5X       � ��$ 15(�65   �� 5Y       � �� 	 DataGridRowHeaderBG# � 	 67   �� 5Z       � ��
 �����68   �� 5[       � ��$ 0.5,0.5,0.5,0q�6<   �� 5\       � ��$ 5q�64   5]         Columns 5_      4  .System.Windows.Controls.DataGridTemplateColumn  5`       
 commonEmployeeCode,  &System.Windows.Controls.DataGridColumn   Header# � 
 6-       SortMemberPath$  EmployeeCode��6g   !   ClipboardContentBinding! 6�   �� *EmployeeCode+"   CellTemplate" 5a      �� 5b      .R�5c   "   �� $��3q�6J   $��Center=�6U   $��Center=�6p   ��6,   �� *EmployeeCode+6p   5d      5f        5g        commonEmployeeID# �  6-   $  
EmployeeID��6e   ! 6�   �� *
EmployeeID+" 5h      �� 5i      .R�5j   "   �� $��3q�6H   $��Center=�6S   $��Center=�6n   ��6,   �� *
EmployeeID+6n   5k      5m        5n        commonEmployeeName# �  6-   $  EmployeeName��6g   ! 6�   �� *EmployeeName+" 5o      �� 5p      .R�5q   "   �� $��3q�6J   $��Center=�6U   ��6,   �� *EmployeeName+6U   5r      5t        5u        commonEmployeeDepartment# �  6-   $  DepartmentName��6m   ! 6�   �� *DepartmentName+" 5v      �� 5w      .R�5x   "   �� $��3q�6L   $��Center=�6W   ��6,   �� *DepartmentName+6W   5y      5{        5|        commonEmployeeDate# �  6-   $  TestDate��6g   ! 6�   �� *
TestDate+" 5}      �� 5~      .R�5   "   �� $��3q�6b   $��Center=�6m   ��6,   �� *ConfirmDate+# �� StringFormat$# 
dd/MM/yyyy��6m   5�      5�        5�        clinicReportTimeInScan# �  6-   $  
TimeInScan��6k   ! 6�   �� *
TimeInScan+" 5�      �� 5�      .R�5�   "   �� $��3q�6H   $��Center=�6S   $��Center=�6p   ��6,   �� *
TimeInScan+6p   5�      5�        5�        clinicReportTimeInOrigin# �  6-   $  TimeInOrigin��6m   ! 6�   �� *TimeInOrigin+" 5�      �� 5�      .R�5�   "   �� $��3q�6J   $��Center=�6U   $��Center=�6r   ��6,   �� *TimeInOrigin+6r   5�      5�        5�        clinicReportState# �  6-   $  StateIndexDisplay��6f   ! 6�   �� *StateIndexDisplay+" 5�      �� 5�      .R�5�   "   �� $��3q�6O   $��Center=�6Z   $��Center=�6w   ��6,   �� *StateIndexDisplay+6w   5�      5�        5�        clinicReportTimeInRemarks# �  6-   $  Remarks��6n   ! 6�   �� *	Remarks+" 5�      �� 5�      .R�5�   "   �� $��3q�6E   $��Center=�6P   $��Center=�6m   ��6,   �� *	Remarks+6m   5�      5�      5�      5�      �� 5�   
   -   5        $ lblTotalWorker��5�   !   $��2��6   
����  �$��SemiBold"�6K   $y�	Collapsed=�6a   �� 5�   
   -	   5        $	 btnExportExcel��5�      $	��150��6)   $��0,2,0,0q�65   $��Right=�6F   $��2��6b   $��2��6o   #���  6   $ � Cursor$
$ Hand��6�   ��5�      � 5�       5�      �� 5�      $
��auto �6(   �� 5�      $
��auto �6(   .+�5�      �� $��2,0,0,0q�6    $��1��61   $��Center=�6>     clinicReportExcel#���  6Y   5�      5�      5�      5�      