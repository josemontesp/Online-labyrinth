   M S B A M L   `   `   `  ����  Chat2 - Client    Chat2___Client.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Chat2 - Cliente��5   	   $	��110��6!   $��155.667��6.   $	��110��6>   $	��275��6N   $	��110��6]    � WindowStartupLocation$ Manual=�5   	    � Left$	 240��5   	   
 � Top$ 0��6    � 	Resources 5         placeHolder �� 
TargetType �� BasedOn �� Property �� Value	 �� 
TargetType
 ��Name K� ZIndex �� Path �� RelativeSource �� Mode �� UpdateSourceTrigger
 � Tag �� Triggers �� Binding �� Source%  System.Windows.Markup.Reference �� Value��%z  &	        7���� 5	   
   " ��6$   8   .�5
      ��  � �� 5      �� 5      "	 ��6&   .R�5      � .+�5      �� $
 
textSource��5   "   
������ $ 2��5   "   ��5   &   �� $
 Text� �� *TemplatedParent+$ TwoWay=�$ PropertyChanged=�5   "   �� 5      #��z 6&   #��z��6C   $ 1��6m   .��6~   ��5   "   �� 5   &   " ��6,   .�5   *   ��  � ��
 ���� 6G    5   *   �� 5   .    6:   �� $
 Text�   *
textSource+  6y   .a�5   2   ��  � ��
 �����6O   5   /   5   '   .��5   1   5       5!      5"      5$      5%      5'      .��5)      � .+�5*   
   �� -   5        $��TextBoxUserName��5*   O   $ Nombre de usuario��6   #��[  6*   $��23��6f   $��
10,10,99,0q�6r    �� TextWrapping$
 Wrap=�6�   $�� ��6�   $	��Top=�6�   �� 5+   
   -   5        $��	TextBoxIP��5+   X   $  Ingrese la IP del servidor��6   #��[  63   $��23��6i   $��
10,41,99,0q�6u   $
 Wrap=�6�   $�� ��6�   $	��Top=�6�   �� 5,   
   -   5        $��ButtonConnect��5,      $��Conectar��6&   $��	0,41,10,0q�69   $	��Top=�6L   $��Right=�6d   $��75��6�   �� 5-   
   -   5        $
 puerto��5-   P   $
��8000��6   $ Puerto��6   #��[  6+   $��23��6`   $��182,10,12,0q�6l   $
 Wrap=�6�   $	��Top=�6�   5/      50      