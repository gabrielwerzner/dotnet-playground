rm ..\ServiceKestrel\bin\Debug -r -fo
dotnet publish -c Debug ../ServiceKestrel/ServiceKestrel.csproj
docker build -t evimac ../
docker compose up -d --force-recreate evimac


docker cp ./dist/. eviMaC:/app/JavascriptClient/ng_web_components/dist
#docker cp ../Frontend/Thin/Client_TS/InfoBackstage/InfoCreator.js eviMaC:app/JavascriptClient/Frontend/Thin/Client_TS/InfoBackstage/InfoCreator.js
docker restart eviMaC
