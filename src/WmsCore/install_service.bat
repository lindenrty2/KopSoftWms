sc delete WMSService
sc Create WMSService binpath= "dotnet {WMS所在绝对路径}/WMSCore.dll" displayname= "WMS服务"