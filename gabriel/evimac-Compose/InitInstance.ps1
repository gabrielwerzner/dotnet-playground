echo "Building Keycloak Image"
mvn -f ../Keycloak/Authenticator/MaintenanceAuthenticator/pom.xml clean package
mvn -f ../Keycloak/Themes/pom.xml clean package
docker build -t evidanza-keycloak -f ../DockerfileKeycloak ../
echo "Building eviMaC Image"
dotnet publish -c Debug ../ServiceKestrel/ServiceKestrel.csproj
docker build -t evimac ../
echo "Starting Compose"
docker compose up -d