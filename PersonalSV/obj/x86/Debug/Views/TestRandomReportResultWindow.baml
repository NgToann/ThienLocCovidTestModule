   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   4    .TLCovidTest.Views.TestRandomReportResultWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5        $��14(�5        � WindowStartupLocation$  CenterScreen=�6(    � WindowState$ 	Maximized=�6M    $   testCovidReportResultWindowTitle � Title# �   5   	   $	��600��6P   $
��1000��6]   .��5	      � $��2q�6    � RowDefinitions 5
   
   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   ��   GroupBoxTemplate#���  6   ��5      �� 5        Background1#���  6     BorderStyle#���  6P   ._�5      �� -   5         ��Name$ txtGroupSummaryHeader��5         reportCheckInOutOption#���  6?   5      .��5      �  5      �� 5      �� 5      �� 5       5      �� 5       �� 5!      .+�5#      �� -   5         ��Name$ txtFindWhat��5#      $��20(�6/   $��3q�6=   $��Center=�6I   $	��240��6�   �� 5$      $��1��6    �� Orientation$ 
Horizontal=�6*   .+�5%      ��   commonDatePickerDate#���  6    $��Bottom=�6Z   (  "System.Windows.Controls.DatePicker  5&      -   5          Name$ dpFilterDate��5&   !   $��5,5,0,0q�67   $��Center=�6H   $	��150��6j   5'      �� 5(      -   5        	 ��Name$	 
btnPreview��5(      $��5,0,0,0q�6-     ButtonTemplate#���  6>   $��3��6v   $��5��6�   {�.  ��5)      � 5*       5+      �� 5,   "   $
��auto �60   �� 5-   "   $
��auto �60   .+�5/      �� $��16��6$   $��16��6/   $��/Images/update.png��6;   �� 50      $��2,0,0,0q�6(   $��1��69   $��Center=�6F     commonButtonPreview#���  6a   51      53      54      55      �� 56   
   $��1��6   $��2��6   $��0,5,0,0q�62   ._�57      &   System.Windows.Controls.DataGrid  -   5        
  Name$
 dgReport��57         AutoGenerateColumns .   �� AlternationCount$ 2��6E      AlternatingRowBackground
 �����   ClipboardCopyMode$ IncludeHeader=�6      CanUserAddRows .     SelectionUnit$ FullRow=�58   .      HorizontalGridLinesBrush
 �����   VerticalGridLinesBrush
 �����$��15(�6�      
IsReadOnly .    FrozenColumnCount$ 3��59   )   $��Cabri)�6?      RowHeaderWidth$	 NaNQ�6R    � 	Resources 5:      *  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%�   '        �� 5;      "  6    5<      P� 5=       �@ 
IsSelected . . �5>   "   ��  � ��
 ����6?   �� 5?   "    � ��
 �����6?   5@      5B      5C         ColumnHeaderStyle 5D      �� 5E      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5F      ��  � ��$ Center=�6G   �� 5G       � ��$ 15(�65   �� 5H       � ��  DataGridRowHeaderBG# �  67   �� 5I       � ��
 �����68   �� 5J       � ��$ 0.5,0.5,0.5,0q�6<   �� 5K       � ��$ 5q�64   5L         Columns 5N      4  .System.Windows.Controls.DataGridTemplateColumn  5O       	 commonEmployeeCode,  &System.Windows.Controls.DataGridColumn   Header# � 	 6-       SortMemberPath$  EmployeeCode��6g   !   ClipboardContentBinding! 6�   �� *EmployeeCode+"   CellTemplate" 5P      �� 5Q      .R�5R   "   �� $��3q�6J   $��Center=�6U   $��Center=�6p   ��6,   �� *EmployeeCode+6p   5S      5U        5V       
 commonEmployeeID# � 
 6-   $  
EmployeeID��6e   ! 6�   �� *
EmployeeID+" 5W      �� 5X      .R�5Y   "   �� $��3q�6H   $��Center=�6S   $��Center=�6n   ��6,   �� *
EmployeeID+6n   5Z      5\        5]        commonEmployeeName# �  6-   $  EmployeeName��6g   ! 6�   �� *EmployeeName+" 5^      �� 5_      .R�5`   "   �� $��3q�6J   $��Center=�6U   ��6,   �� *EmployeeName+6U   5a      5c        5d        commonEmployeeDepartment# �  6-   $  DepartmentName��6m   ! 6�   �� *DepartmentName+" 5e      �� 5f      .R�5g   "   �� $��3q�6L   $��Center=�6W   ��6,   �� *DepartmentName+6W   5h      5j        5k        commonEmployeeDate# �  6-   $  TestDate��6g   ! 6�   �� *
TestDate+" 5l      �� 5m      .R�5n   "   �� $��3q�6_   $��Center=�6j   ��6,   �� *
TestDate+# �� StringFormat$# 
dd/MM/yyyy��6j   5o      5q        5r        reportGridTimeIn# �  6-   $  TimeIn��6e   ! 6}   �� *TimeIn+" 5s      �� 5t      .R�5u   "   �� $��3q�6D   $��Center=�6O   $��Center=�6l   ��6,   �� *TimeIn+6l   5v      5x        5y        confirmTestResultTestResult# �  6-   $  Result��6p   ! 6�   �� *Result+" 5z      �� 5{      .R�5|   "   �� $��3q�6D   $��Center=�6O   $��Center=�6l   ��6,   �� *Result+6l   5}      5        5�       " confirmTestResultConfirmedTime# �  6-   $  ConfirmTime��6s   ! 6�   �� *ConfirmTime+" 5�      �� 5�      .R�5�   "   �� $��3q�6I   $��Center=�6T   $��Center=�6q   ��6,   �� *ConfirmTime+6q   5�      5�        5�         confirmTestResultConfirmedBy# �  6-   $  ConfirmedBy��6q   ! 6�   �� *ConfirmedBy+" 5�      �� 5�      .R�5�   "   �� $��3q�6I   $��Center=�6T   $��Center=�6q   ��6,   �� *ConfirmedBy+6q   5�      5�        5�        confirmTestResultRemark# �  6-   $  Remark��6l   ! 6�   �� *Remark+" 5�      �� 5�      .R�5�   "   �� $��3q�6D   $��Center=�6O   $��Center=�6l   ��6,   �� *Remark+6l   5�      5�      5�      5�      �� 5�   
   -   5        $ lblTotalWorker��5�   !   $��2��6   
����  �$��SemiBold"�6K   $y�	Collapsed=�6a   �� 5�   
   -   5        $	 btnExportExcel��5�      $	��150��6)   $��0,2,0,0q�65   $��Right=�6F   $��2��6b   $��2��6o   #���  6   $ � Cursor$
$ Hand��6�   ��5�      � 5�       5�      �� 5�      $
��auto �6(   �� 5�      $
��auto �6(   .+�5�      �� $��2,0,0,0q�6    $��1��61   $��Center=�6>     reportExportExcel#���  6Y   5�      5�      5�      5�      