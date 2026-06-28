cd ..
dotnet publish -c Release -r win-x64 --self-contained true VpMobilPlusPlus.Desktop
dotnet publish -c Release -r linux-x64 --self-contained true VpMobilPlusPlus.Desktop
dotnet publish -c Release VpMobilPlusPlus.Android