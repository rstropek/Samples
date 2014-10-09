REM For details see http://blogs.msdn.com/b/winsdk/archive/2009/11/13/steps-to-sign-a-file-using-signtool-exe.aspx
makecert.exe -r -pe -ss MY -sky exchange -n CN="MSI Training" CodeSign.cer
echo Please import CodeSign.cer into Trusted Root Certs of local computer
pause
Signtool sign /v /s MY /n "MSI Training" /t http://timestamp.verisign.com/scripts/timstamp.dll bin\Debug\AwesomeSoftware.msi
