%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe D:\Source\repo2017\apiStartSvc\apiStartSvc\bin\Debug\apiStartSvc.exe
Net Start apiStartSvc.exe
sc config apiStartSvc.exe start= auto