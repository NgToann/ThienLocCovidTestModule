   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   +    %TLCovidTest.Views.WorkerCheckInWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5        
�������$��14(�5   2     � WindowStartupLocation$  CenterScreen=�6@      workerCheckInWindowTitle � Title# �   5   	   $	��500��6H   $
��1200��6U   $	��800��6b    � WindowState$ 	Maximized=�6q   .��5	      � $��2q�6    � RowDefinitions 5
   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   �� $��2��6   $��0,0,0,15q�6&    �� CornerRadius$ 5��68   $��1,1,1,1q�6I   
�������._�5      �� -   5         ��Name$ tblTitle��5        scanPatientMainHeader#���  5      $��SemiBold"�5      $��35(�6*   $��Center=�68    �� TextAlignment$ Center=�5      5      � 5   
   $��1��6   $	��400��6   $	��400��6(   $
��Left=�67   $��Bottom=�6R   $��2,0,0,0q�6m    5      �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $��* �6    �� 5       $
��auto �6    .+�5"      ��   workerCheckInScanCardId#���  6   $ Center=�6U   $��20(�6l   �� 5#      -   5         ��Name$ 	txtCardId��5#   @   $��1��6   $��3q�6#   $��0,4,0,0q�6/   {�.  
���   �$��25(�5$   *   
����  �$��SemiBold"�6J   $��Center=�5%      	 �� TextAlignment$	 Center=�5&      6-   5'      �� 5(   
   $��1��6   $��1��6   $��0,0,2,0q�6.   
 �� Padding$
 	15,4,15,4q�6?     BorderStyle#���  6S   
�������$ 5��6�   $��1,1,1,1q�6�   ._�5)      �  5*      �� 5+      $
��auto �6$   �� 5,      .+�5.      �� 
�������$y�	Collapsed=�6,   ._�5/      �  50      �� 51      �� 52      .+�54      ��   workerCheckInStatistics#���  6$   
����  �$��SemiBold"�6s   $��18(�6�   �� 55      -   5        $ lblClock��55   P   $��1��6$   $��Right=�64   $��CLOCK��6b   $��18(�6o   
����  �$��SemiBold"�6�   56      57      � 58      -   5         �Name$ grStatistics��58      $��1��6-   $��0,5,0,0q�6:    59      �� 5:      $
��auto �6(   �� 5;      $
��auto �6(    5=      �� 5>      �� 5?      �� 5@      .+�5B      �� $��18(�6:   $��SemiBold"�6H   
����  ���6    �� *
ScanDate+6^   �� 5C      $��1��6    $��18(�6N   $��SemiBold"�6\   
����  ���60   �� *TotalPatient+6r   �� 5D      $��2��6    $��18(�6I   $��SemiBold"�6W   
��� � ���60   �� *	Scanned+6m   5E      5F      5G      �� 5H   
   -   5         ��Name$ 	brDisplay��5H      $��2��6$   $��2��61   $��2q�6E   
�������$��1,1,1,1q�6c   $ 5��6}   $
 4q�6�   ._�5I      (�  (� Stretch$ Uniform=�6   $��Center=�6(   $��Center=�6E   .��5J      � -   5        $ 	grDisplay��5J       5K      �� 5L      �� 5M      �� 5N      �� 5O      �� 5P      $
��auto �6(   �� 5Q      $
��auto �6(   .+�5S      �� $��Center=�6>   ��6    �� *EmployeeName+6>   �� 5T      $��1��6    $��Center=�6I   ��6-   �� *
EmployeeID+6I   �� 5U      $��2��6    $��Center=�6M   ��6-   �� *DepartmentName+6M   �� 5V      $��3��6    $��Center=�6I   ��6-   �� *
RecordTime+6I   �� 5W      $��4��6    $��Center=�6G   ��6-   �� *
Message1+6G   �� 5X      $��5��6    $��Center=�6G   ��6-   �� *
Message2+6G   5Y      5Z      5[      5\      5]      