# infrastructure-microservices
Criação de uma aplicação com microsserviços

# markdown
https://dillinger.io/
https://stackedit.io/app#

# Criação do Kafka
https://www.baeldung.com/ops/kafka-new-topic-docker-compose
https://cursos.alura.com.br/forum/topico-sugestao-docker-compose-kafka-zookeeper-e-kakfa-ui-279740

# Criação do MongoDb
https://geshan.com.np/blog/2023/03/mongodb-docker-compose/

# Criação do PostgreSQL
https://commandprompt.com/education/how-to-install-postgresql-using-docker-compose/


## Git
```sh
git pull
git add .
git commit -m "mensagem-commit"
git push
```

## Criar pastas para organização
```sh
mkdir src
mkdir test
```

## .NetCore
```sh
dotnet new sln -n Credditus
dotnet new xunit -n Credditus.UnitTests
dotnet new webapi -n Credditus.Api -minimal
dotnet new gitignore
dotnet run --project src/Credditus.Api/Credditus.Api.csproj dev-certs https --trust
dotnet sln add ./ProjetoTeste.Tests/ProjetoTeste.Tests.csproj 
dotnet add ./Alura.Estacionamento.Tests/Alura.Estacionamento.Tests.csproj reference ./Alura.Estacionamento/Alura.Estacionamento.csproj 
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.10
```

## Docker
```sh
cd ~/
docker-compose up -d --build
docker compose down
```

## biography
https://renatogroffe.medium.com/net-core-3-1-apache-kafka-exemplos-utilizando-mensageria-21fad6e0aab