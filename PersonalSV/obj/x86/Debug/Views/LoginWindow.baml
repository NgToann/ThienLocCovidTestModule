   M S B A M L   `   `   `  ����F  BTLCovidTest, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null5clr-namespace:TLCovidTest.ViewsTLCovidTest.Views  5   	   #    TLCovidTest.Views.LoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TLCovidTest.Views     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5         logInWindowTitle � Title# �   5   	    � SizeToContent$ WidthAndHeight=�64    � 
ResizeMode$ CanMinimize=�6S   .��5	      � .+�5
   
   �� $	��265��6   $	��297��6   ._�5      �� $��/Images/login-tl.png��6   5      �� 5   
   -   5         ��Name$ txtUserName��5      
������ $	��165��6A   
������ $��16(�6f   $��30,132,70,134q�6t   $��Center=�6�   $��4,0,0,0q�6�   .��6�   ��5      �� 5       �� 
TargetType" ��6    �� Triggers 5      P� 5       P� Property �@J�	IsFocused P� Value . . �5      �� 	 �� Property	 � ��
 �� Value

 ���� 6<   �� 5      	 � ��$
 0,0,0,0q�6@   5      P� 5       �@J�IsMouseOver . . �5      �� 	 � ��

 ���� 6<   �� 5      	 � ��$
 0,0,0,0q�6@   5      5      .��5      5      G� 5   
   -   5         G�Name$ txtPassword��5      
������ 
������ $	��165��6^   $��16(�6j   $��30,183,70,82q�6x   $��Center=�6�   $��4,0,0,0q�6�   ��5      �� 5      " G�6    5       P� 5!       �@J�	IsFocused . . �5"      �� 	 � ��

 ���� 6<   �� 5#      	 � ��$
 0,0,0,0q�6@   5$      P� 5%       �@J�IsMouseOver . . �5&      �� 	 � ��

 ���� 6<   �� 5'      	 � ��$
 0,0,0,0q�6@   5(      5*      5,      �� 5-   
   -   5         ��Name$ btnLogin��5-      $��29,234,29,29q�6#   
������ 
������ $��0,0,0,0q�6l    �� 	IsDefault . ��5.      �� 5/      $	��210��6   $��34��6$   $��/Images/btnlogin.png��60   $
��Fill=�6N   ��51      �� 52      " ��6   .�53      �� 	 � ��

 ���� 64   �� 54      	 � ��$
 0,0,0,0q�68   �� 55      	 � ��

 ���� 63    56      P� 57       �@J�IsMouseOver . . �58      �� 	 � ��

 ���� 6<   �� 59      	 � ��$
 0,0,0,0q�6@   �� 5:      	 � ��

 ���� 6;   5;      P� 5<       � �� . . �5=      �� 	 � ��

 ���� 6<   �� 5>      	 � ��$
 0,0,0,0q�6@   �� 5?      	 � ��

 ���� 6;   5@      5B      5D      5E      5F      