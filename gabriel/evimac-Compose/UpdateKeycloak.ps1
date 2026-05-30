echo "Building Keycloak Image"
mvn -f ../Keycloak/Authenticator/MaintenanceAuthenticator/pom.xml clean package
mvn -f ../Keycloak/Themes/pom.xml clean package
docker build -t evidanza-keycloak -f ../DockerfileKeycloak ../
echo "Restarting Keycloak Container"
docker compose up -d --force-recreate keycloak