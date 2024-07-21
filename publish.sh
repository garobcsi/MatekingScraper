dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false

cp ./src/.env.example ./src/bin/Release/net8.0/win-x64/publish/.env.example
cp ./src/.env.example ./src/bin/Release/net8.0/linux-x64/publish/.env.example

cd ./src/bin/Release/net8.0/win-x64/publish
zip -r ../../../../../../MathScraper-windows.zip ./*

cd ./src/bin/Release/net8.0/linux-x64/publish
tar -zcf ../../../../../../MathScraper-linux.tar.gz *