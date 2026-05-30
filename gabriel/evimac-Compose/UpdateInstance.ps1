rm ..\ServiceKestrel\bin\Debug -r -fo
dotnet publish -c Debug ../ServiceKestrel/ServiceKestrel.csproj
docker build -t evimac ../
docker compose up -d --force-recreate evimac