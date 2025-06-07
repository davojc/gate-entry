rmdir /S /Q publish
dotnet publish -c Release -r win-x64 --self-contained true --output ./publish