   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   5    /TLCovidTest.Views.TestCovidCreateTestTermWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5      $��15(�6?      createTestTermWindowTitle � Title# �   5   	   $	��600��6I   $
��1000��6V   .��5	      � $��2q�6    � ColumnDefinitions 5
   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   ��   GroupBoxTemplate#���  6   ��5      �� 5        Background1#���  6     BorderStyle#���  6P   ._�5      �� -   5         ��Name$ txtGroupSummaryHeader��5         createTestTermGroupHeader#���  6?    �� Padding$ 3q�6~   5      .��5      �  5      �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'    5       �� 5!      $
��Auto �6$   �� 5"      $
��Auto �6$   �� 5#      .+�5%      ��  �� Orientation$ 
Horizontal=�6   .+�5&      ��   createTestLblTestRate#���  6    $��Center=�6[   �� 5'      -   5         ��Name$ txtRandomRate��5'   0   $��10,0,0,0q�6   $��20��6G   $��18(�6Q   $��3q�6_   $��Center=�6k   
���  ��$��80��6�   $��SemiBold"�6�   $��Center=�6�   �� 5(      $�� %��6    $��Center=�6*   $��SemiBold"�6E   5)      �� 5*      $��1��6   $��0,10,0,0q�6)     createTestTestDate#���  6;   (  "System.Windows.Controls.DatePicker  5+      -   5          Name$ 
dpTestDate��5+   *   $��2��6   $��Center=�6>   �� 5-      $��2��6   $��1��6)   $��	10,0,10,0q�69   $��Center=�6L   $��Bottom=�6i   �� 5.      -   5        	 ��Name$	 hlViewWorkerList��5.       .�6Y   .�5/      �� 
 � Cursor$

 Hand��6$     createTestViewWorkerList#���  62   50      51      �� 53      $��2��6   $��2��6)   $��	10,0,10,0q�69   $��Center=�6L   $��Bottom=�6i   �� 54      -   5        $	 hlTestPlanlist��54       .�6U   .�55      �� $

 Hand��6$     createTestViewTestPlanList#���  62   56      57      �� 59      -   5         ��Name$ 	btnCreate��59      $��3��6,   $��3��6<   $��5,0,0,0q�6M    	 ButtonTemplate#��� 	 6^   $

 Hand��6�   ��5:      � 5;       5<      �� 5=   "   $
��auto �60   �� 5>   "   $
��auto �60   .+�5@      �� $��2,0,0,0q�6(   $��1��69   $��Center=�6F    
 createTestBtnCreate#��� 
 6a   5A      5C      5D      5E      � 5G   
   $��1��6   $��0,0,0,6q�6    5H      �� 5I      �� 5J      $
��auto �6    �� 5K      $
��auto �6    .+�5M      �� -   5        $ btnImportExcel��5M   J   $��1��6   $	��200��6"   $��Right=�6.     menuCreateNewImportExcel#���  6b   $��	10,4,10,4q�6�   $��0,0,0,10q�6�   $

 Hand��6�   
���  ��$��SemiBold"�6�   6�   �� 5N      $��2��6   $ 
Horizontal=�6&   $��Right=�6?   .+�5O      ��   menuCreateNewInputWorkerId#���  6   $��Bottom=�6\   $��0,0,10,0q�6w   �� 5P      -	   5        $ txtWorkerId��5P      $��20(�6/   $	��200��6>   $��3q�6M   
����  �$��0,0,10,0q�6k   6}   �� 5Q      -
   5        $ btnAddWorkerId��5Q        menuCreateNewAdd#���  61   $��	10,4,10,4q�6j   $

 Hand��6~   
����  �$��SemiBold"�6�   6�   5R      5S      &   System.Windows.Controls.DataGrid  5U   
   -   5          Name$ dgRandomList��5U   E   $��1��6   $��2��6    $��0,2,0,0q�64      AutoGenerateColumns .   �� AlternationCount$ 2��6w      AlternatingRowBackground
 �����   CanUserAddRows .     SelectionUnit$ FullRow=�5V   .      HorizontalGridLinesBrush
 �����   VerticalGridLinesBrush
 �����   
IsReadOnly . $��Arial)�5W   )   $��15(�6<      FrozenColumnCount$ 3��6J      RowHeaderWidth$	 NaNQ�6`    � 	Resources 5X      *  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%�   '        �� 5Y      "  6    5Z      P� 5[       �@ 
IsSelected . . �5\      ��  � ��
 ����6;   �� 5]       � ��
 �����6;   5^      5`      5a         ColumnHeaderStyle 5c      �� 5d      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5e      ��  � ��$ Center=�6C   �� 5f       � ��$ 16(�61   �� 5g       � ��  DataGridRowHeaderBG# �  63   �� 5h       � ��
 �����64   �� 5i       � ��$ 0.5,0.5,0.5,0q�68   �� 5j       � ��$ 5q�60   5k         Columns 5m      4  .System.Windows.Controls.DataGridTemplateColumn  5n        commonEmployeeCode,  &System.Windows.Controls.DataGridColumn    Header#  �  6)   !   SortMemberPath$! EmployeeCode��6c   "   ClipboardContentBinding" 6�   �� *EmployeeCode+#   CellTemplate# 5o      �� 5p      .R�5q      �� $��3q�6F   $��Center=�6Q   $��Center=�6l   ��6(   �� *EmployeeCode+6l   5r      5t        5u        commonEmployeeID#  �  6)   $! 
EmployeeID��6a   " 6}   �� *
EmployeeID+# 5v      �� 5w      .R�5x      �� $��3q�6D   $��Center=�6O   $��Center=�6j   ��6(   �� *
EmployeeID+6j   5y      5{        5|        commonEmployeeName#  �  6)   $! EmployeeName��6c   # 5}      �� 5~      .R�5      �� $��3q�6F   $��Center=�6Q   ��6(   �� *EmployeeName+6Q   5�      5�        5�        commonEmployeeDepartment#  �  6)   $! DepartmentName��6i   " 6�   �� *DepartmentName+# 5�      �� 5�      .R�5�      �� $��3q�6H   $��Center=�6S   ��6(   �� *DepartmentName+6S   5�      5�        5�      #  �  6)   $! TestDate��6c   " 6}   �� *
TestDate+# 5�      �� 5�      .R�5�      �� $��3q�6[   $��Center=�6f   ��6(   �� *
TestDate+$ �� StringFormat$$ 
dd/MM/yyyy��6f   5�      5�        5�        createTestTerm#  �  6)   $
! Term��6_   %   
Visibility$% 	Collapsed=�6�   " 6u   �� *Term+# 5�      �� 5�      .R�5�      �� $��3q�6>   $��Center=�6I   ��6(   �� *Term+6I   5�      5�        5�        createTestRound#  �  6)   $! Round��6`   $% 	Collapsed=�6�   " 6w   �� *Round+# 5�      �� 5�      .R�5�      �� $��3q�6?   $��Center=�6J   ��6(   �� *Round+6J   5�      5�      5�      � 5�   
   $��2��6   $��2��6   $��0,2,0,0q�60    5�      �� 5�      $��* �6#   �� 5�      $
��auto �6#   �� 5�      $
��auto �6#   .+�5�      �� -   5        $ 	btnDelete��5�      $��1��6(   $��0,0,30,0q�68   #��� 	 6J   $

 Hand��6�   ��5�      � 5�       5�      �� 5�      $
��auto �6,   �� 5�      $
��auto �6,   .+�5�      �� $ 	25,4,25,4q�6$   $��1��68   $��Center=�6E     commonButtonDelete#���  6`   
���  ��$��SemiBold"�6�   5�      5�      �� 5�      -   5        $ btnSave��5�      $��2��6&   #��� 	 66   $

 Hand��6�   ��5�      � 5�       5�      �� 5�      $
��auto �6,   �� 5�      $
��auto �6,   .+�5�      �� $ 	25,4,25,4q�6$   $��1��68   $��Center=�6E     commonButtonSave#���  6`   5�      5�      5�      5�      5�      