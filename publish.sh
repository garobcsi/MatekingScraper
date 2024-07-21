dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false

cp ./src/.env.example ./src/bin/Release/net8.0/win-x64/publish/.env
wget https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip -O ffmpeg.zip
unzip -p ffmpeg.zip ffmpeg-master-latest-win64-gpl/bin/ffmpeg.exe > ./src/bin/Release/net8.0/win-x64/publish/ffmpeg.exe

cp ./src/.env.example ./src/bin/Release/net8.0/linux-x64/publish/.env

cd ./src/bin/Release/net8.0/win-x64/publish
zip -r ../../../../../../MathScraper-windows.zip ./* .env

cd ../../../../../..

cd ./src/bin/Release/net8.0/linux-x64/publish
tar -zcf ../../../../../../MathScraper-linux.tar.gz * .env