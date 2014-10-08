set wixDir="C:\Program Files (x86)\WiX Toolset v3.8\bin\"

%wixDir%candle Old\Product.wxs -out Old\
%wixDir%light Old\Product.wixobj -out Output\Product1_0_0_0.msi

%wixDir%candle New\Product.wxs -out New\
%wixDir%light New\Product.wixobj -out Output\Product1_0_1_0.msi

%wixDir%candle Patch\Patch.wxs -out Patch\
%wixDir%light Patch\Patch.wixobj -out Output\Patch.wixmsp

%wixDir%torch -p -xi Output\Product1_0_0_0.wixpdb Output\Product1_0_1_0.wixpdb -out Output\Diff.wixmst 
%wixDir%pyro Output\Patch.wixmsp -t RTM Output\Diff.wixmst -out Output\Patch.msp 

