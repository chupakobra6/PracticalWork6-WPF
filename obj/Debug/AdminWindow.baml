   M S B A M L   `   `   `  ����I  EPracticalWork6, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:PracticalWork6PracticalWork6  5   	        PracticalWork6.AdminWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:PracticalWork6     5   	     � WindowStartupLocation$  CenterOwner=�5   	    � Title$ AdminWindow��5	   	   $	��450��6   $	��800��6*   .��5
      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��5      $��0��5      .+�5      �� $��Users in Chat��5      $
��Bold"�5      $��5q�5      �� 5      -   5         ��Name$ UserList��5       $��5q�5!      5#      � 5%   
   $��1��5&      $��1��5'       5(      �� 5)      $��* �6    �� 5*      $
��Auto �6    .+�5-      �� -   5         ��Name$ ChatLog��5.      $��0��5/      $��0��50      $��5q�51      52      �� 54      $��1��55       �� Orientation$ 
Horizontal=�56      $��5q�57      .+�59      �� -   5         ��Name$ MessageInput��5:       �� TextWrapping$
 Wrap=�5;      	 �� AcceptsReturn	 . $��0,0,10,0q�5=      $	��500��5>      $	��100��5?      "
 �� VerticalScrollBarVisibility$

 Auto=�5@      �� 5B      -   5         ��Name$ 
SendButton��5C      $
��Send��5E      $��5q�5F      $
��10,5q�5G      $	��100��5H      $
��Bold"�5I      5K      5L      �� 5N   
   -   5        $ DiconnectButton��5O      $��2��5Q      $��1��5R      $��
Disconnect��5S      $��5q�5T      $��Right=�5U      $	��100��5V      $
��Bold"�5W      �� 5Y   
   -   5        $ 
LogsButton��5Z      $��2��5\      $��1��5]      $
��Logs��5^      $��	5,5,111,5q�5_      $��Right=�5`      $	��100��5a      $
��Bold"�5b      5c      5d      