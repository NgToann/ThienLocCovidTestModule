   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   4    .TLCovidTest.Views.TestCovidConfirmResultWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5          � WindowState$  	Maximized=�5   N   $��16(�5       � WindowStartupLocation$ CenterScreen=�6)       confirmTestResultWindowTitle � Title# �   5   	   $	��700��6L   $
��1000��6Y   .��5	      � $��2q�6    � ColumnDefinitions 5
   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   �� $��0,0,0,10q�6   $��2��6#    �� CornerRadius$ 5��67   $��1,1,1,1q�6H   
�������
�������._�5      �� -   5         ��Name$ 	lblHeader��5      
��� � �  confirmTestResultHeader#���  5      $��SemiBold"�5      $��35(�6*   $��Center=�68    �� TextAlignment$ Center=�5      5      � 5   
   $��1��6   $	��400��6   $
��Left=�6(   $��Bottom=�6C   $��2,0,0,0q�6^    5      �� 5      �� 5      .+�5      ��   workerCheckInScanCardId#���  6   $��20(�6U   $��Center=�6c   �� 5       -   5         ��Name$ 	txtCardId��5    /   $��1��6   $��3q�6#   {�.  
���   �$��25(�5!   *   
����  �$��SemiBold"�6J   $��Center=�5"      	 �� TextAlignment$	 Center=�5#      6-   5$      � 5&   
   $��1��6   $��2��6    5'      �� 5(      �� 5)      .+�5+      �� -   5        $ lblTotalCheckIn��5+      $��Center=�61   $��Center=�6N   $��20(�6i   
����  �6w   �� 5,      -   5        $ lblConfirmed��5,      $��1��6.   $��Center=�6>   $��Center=�6[   $��20(�6v   
��� � �6�   5-      �� 5/   
   -   5        
 ��Name$
 	brDisplay��5/      $��2��6$   $��2��61   $��2q�6E   
�������$��1,1,1,1q�6c   $ 5��6}    �� Padding$ 10q�6�   ._�50      �� $
��Auto=�6   .��51      �  52      �� 53      $
��auto �6$   �� 54      $
��auto �6$   �� 55      $
��auto �6$   �� 56      $
��auto �6$   �� 57      $
��auto �6$   �� 58      $
��auto �6$   �� 59      $
��auto �6$   �� 5:      $
��auto �6$   �� 5;      $
��auto �6$   �� 5<      $
��auto �6$   �� 5=      $
��auto �6$   �� 5>      $
��auto �6$   �� 5?       5A      �� 5B      �� 5C      $��4* �6'   .+�5E      �� $��Test Id��6    �� Padding$ 4q�6+   �� 5F      $��1��6     confirmTestResultFullName#���  6)   $ 4q�6h   �� 5G      $��2��6    ! confirmTestResultEmployeeCode#���  6)   $ 4q�6l   �� 5H      $��3��6     confirmTestResultEmployeeId#���  6)   $ 4q�6j   �� 5I      $��4��6    # confirmTestResultDepartmentName#���  6)   $ 4q�6n   �� 5J      $��5��6     confirmTestResultTestDate#���  6)   $ 4q�6h   �� 5K      $��6��6     confirmTestResultTimeIn#���  6)   $ 4q�6f   �� 5L      $��7��6    "	 confirmTestResultConfirmedTime#��� 	 6)   $ 4q�6m   �� 5N      $��8��6   $��0,10,0,0q�6)    
 confirmTestResultTestResult#��� 
 6;   
���  ��$��SemiBold"�6�   $ 4q�6�   �� 5O      $��9��6   $��Center=�6)      confirmTestResultConfirmedBy#���  6D   $ 4q�6�   �� 5P      $��10��6   $��Center=�6*     confirmTestResultRemark#���  6E   $ 4q�6�   � 5R      $��1��6    5S      �� 5T      �� 5U      $
��auto �6/   .+�5W      �� $ 4q�68   ��6$   �� *Id+68   �� 5X      $��1��6%    �� Orientation$ 
Horizontal=�65   $��Center=�6N   .+�5Y      �� -   5         ��Name$ cbChangeConfirmDate��5Y   '    � Cursor$
 Hand��6D   $��Change Confirm Date��6R   $��Center=�6p   
���  ��$��Center=�6�    d� 	IsChecked$ FalseY�6�   6�   (  "System.Windows.Controls.DatePicker  5Z      -   5          Name$ dpConfirmDate��5Z   )   $��Center=�6@   $��10,0,0,0q�6b   $y�	Collapsed=�6t   6�   5[      5\      �� 5]      $��2��6   $��1��6,   $ 4q�6W   ��69   �� *EmployeeName+6W   �� 5^      $��2��6   $��2��6,   $ 4q�6W   ��69   �� *EmployeeCode+6W   �� 5_      $��2��6   $��3��6,   $ 4q�6U   ��69   �� *
EmployeeID+6U   �� 5`      $��2��6   $��4��6,   $ 4q�6Y   ��69   �� *DepartmentName+6Y   �� 5a      $��2��6   $��5��6,   $ 4q�6S   ��69   �� *
TestDate+6S   �� 5b      $��2��6   $��6��6,   $ 4q�6Q   ��69   �� *TimeIn+6Q   �� 5c      $��2��6   $��7��6,   $ 4q�6[   ��69   �� *UpdateResultTime+6[   �� 5e      -	   5         ��Name$ 	cboResult��5e      $��2��6.   $��8��6>   $��Center=�6K   $��Center=�6m   $	��300��6�   $
��Left=�6�   �� 5g      -
   5        $ txtConfirmBy��5g   N   $��2��6   
����  �$��9��6A   $��0,10,0,0q�6d   $��80��6v   $��3q�6�    �� TextWrapping$
 Wrap=�6�   �� 5h      -   5        $ 	txtRemark��5h   S   $��2��6   
����  �$��10��6E   $��0,10,0,0q�6f   $��80��6x   $��3q�6�   $
 Wrap=�6�   5j      5k      5l      �� 5m   
   -   5         ��Name$ btnSave��5m      $��3��6"   $��1��6/     ButtonTemplate#���  6?   {�.  $	��200��6�   $��0,1,2,0q�6�   $��Right=�6�   $
 Hand��6�   ��5n      � 5o       5p      �� 5q      $
��auto �6(   �� 5r      $
��auto �6(   .+�5t      �� $ 	15,4,15,4q�6    $��1��64   $��Center=�6A     commonButtonSave#���  6\   5u      5w      5y      5z      