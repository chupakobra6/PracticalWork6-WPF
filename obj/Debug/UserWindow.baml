   M S B A M L   `   `   `  ����I  EPracticalWork6, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:PracticalWork6PracticalWork6  5   	       PracticalWork6.UserWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:PracticalWork6     5   	     � WindowStartupLocation$  CenterOwner=�5   	    � Title$ 
UserWindow��5	   	   $	��450��6   $	��800��6)   .��5
      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��5      $��0��5      .+�5      �� $��Users in Chat��5      $
��Bold"�5      $��5q�5      �� 5      -   5         ��Name$ UserList��5       $��5q�5!      5#      � 5%   
   $��1��5&      $��1��5'       5(      �� 5)      $��* �6    �� 5*      $
��Auto �6    .+�5-      �� $��Chat Window��5.      $
��Bold"�5/      $��5q�50      �� 52      -   5         ��Name$ ChatLog��53      $��0��54      $��0��55      $��5q�56      57      �� 59      $��1��5:       �� Orientation$ 
Horizontal=�5;      $��5q�5<      .+�5>      �� -   5         ��Name$ MessageInput��5?       �� TextWrapping$
 Wrap=�5@      	 �� AcceptsReturn	 . $��0,0,10,0q�5B      $	��500��5C      $	��100��5D      "
 �� VerticalScrollBarVisibility$

 Auto=�5E      �� 5G      -   5         ��Name$ 
SendButton��5H      $
��Send��5J      $��5q�5K      $
��10,5q�5L      $	��100��5M      $
��Bold"�5N      5P      5Q      �� 5S   
   -   5        $ DiconnectButton��5T      $��2��5V      $��1��5W      $��
Disconnect��5X      $��5q�5Y      $��Right=�5Z      $	��100��5[      $
��Bold"�5\      5]      5^      