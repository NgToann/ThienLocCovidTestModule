   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   -    'TLCovidTest.Views.WorkerPrizeScanWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5          � WindowState$  	Maximized=�5   7    � WindowStartupLocation$ CenterScreen=�5      
������� � Title$ ThienLoc Scan Gift��5   	   $	��600��6$   $
��1200��61   .��5	      � $��2q�6    � ColumnDefinitions 5
   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   �� $��0,0,0,10q�6   $��2��6#    �� CornerRadius$ 5��67   $��1,1,1,1q�6H   
�������
����·�._�5      �� -   5         ��Name$ 	lblHeader��5      
��� � �$��NHẬN QUÀ NGÀY��5      $��SemiBold"�5      $��35(�6*   $��Center=�68    �� TextAlignment$ Center=�5      5      � 5   
   $��1��6   $	��400��6   $
��Left=�6(   $��Bottom=�6C   $��2,0,0,0q�6^    5      �� 5      �� 5      .+�5       ��    workerCheckInScanCardId#���   6   $��20(�6U   $��Center=�6c   �� 5!      -   5         ��Name$ 	txtCardId��5!   /   $��1��6   $��3q�6#   {�.  
���   �$��25(�5"   *   
����  �$��SemiBold"�6J   $��Center=�5#      	 �� TextAlignment$	 Center=�5$      6-   5%      �� 5&   
   -   5        
 ��Name$
 	brDisplay��5&      $��2��6$   $��2��61   $��2q�6E   
�������$��1,1,1,1q�6c   $ 5��6}    �� Padding$ 4q�6�   ._�5'      �  5(      �� 5)      �� 5*      $
��auto �6$   .+�5,      (�  (� Stretch$ Uniform=�6   $��Center=�6,   $��Center=�6I   .��5-      � -   5         �Name$ 	grDisplay��5-       5.      �� 5/      $
��auto �6,   �� 50      $
��auto �6,   �� 51      $
��auto �6,   �� 52      $
��auto �6,   .+�54      �� $��Center=�6>   ��6$   �� *
FullName+6>   �� 55      $��1��6$   $��Center=�6K   ��61   �� *
WorkerId+6K   �� 56      $��2��6$   $��Center=�6Q   ��61   �� *DepartmentName+6Q   �� 57      $��3��6$   $��Center=�6J   ��61   �� *	Message+6J   58      59      5:      5;      5<      5=      