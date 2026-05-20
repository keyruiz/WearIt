Get-Process -Name "dotnet", "MvcWearIt" -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet build
