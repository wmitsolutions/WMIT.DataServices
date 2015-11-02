mkdir output
nuget pack "..\WMIT.DataServices\WMIT.DataServices.csproj" -OutputDirectory ".\output"
nuget pack "..\WMIT.DataServices.Core\WMIT.DataServices.Core.csproj" -OutputDirectory ".\output"