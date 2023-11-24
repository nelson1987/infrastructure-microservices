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

git init
git add .
git commit -m "mensagemCommit"

git -config --global user.email "your@example.com"
git config --global user.name "Your Name"

git branch -M main
git remote add origin git@github.com:rodrigoalura87/numero-secreto.git
git push -u origin main

ssh-keygen -t ed25519 -C "rodrigo.alura87@gmail.com"

sincronizar
git push -u origin main -- Subir código
git pull origin main -- Baixar código
adicionar repositorio remoto
git remote add apelido url.git

git clone https://github.com/rodrigoalura87/numero-secreto.git

quais commits foram realizados no projeto e quem fez
git log

desfazer a mensagem do commit ANTERIOR(Só funciona com o anterior)
git commit --amend -m "Trocando botao para adivinhar"

REMOVER um commit
git log
git reset --hard <id_do_commit_anterior>

README-MARKDOWN
https://github.com/alura-cursos/3386-git-github/blob/main/README.md?plain=1
https://www.alura.com.br/artigos/escrever-bom-readme?_gl=1*18ny3le*_ga*NjYyMzY0NDQwLjE2OTUyMjk4MzQ.*_ga_1EPWSW3PCS*MTcwMDg2MzA0My4yOS4xLjE3MDA4NjUyMjYuMC4wLjA.*_fplc*cTdiZTBNbHFzeEtONWE4Q01Rb2VLOCUyQll0QXRBZkxXSkE5MFBocUN3cWhPSVFhQWdMdXc3NU9IVGJHYjlNZkFaclBTQktKTnZlQ0hBMFI0MUpmYktqa05TSGl3eUtWcWpPYTFBZ3JjUXpITWtLNTdFM2w2S1NaRFdxVndRVXclM0QlM0Q.
https://www.alura.com.br/artigos/como-criar-um-readme-para-seu-perfil-github?_gl=1*ol6kud*_ga*NjYyMzY0NDQwLjE2OTUyMjk4MzQ.*_ga_1EPWSW3PCS*MTcwMDg2MzA0My4yOS4xLjE3MDA4NjUyMzkuMC4wLjA.*_fplc*cTdiZTBNbHFzeEtONWE4Q01Rb2VLOCUyQll0QXRBZkxXSkE5MFBocUN3cWhPSVFhQWdMdXc3NU9IVGJHYjlNZkFaclBTQktKTnZlQ0hBMFI0MUpmYktqa05TSGl3eUtWcWpPYTFBZ3JjUXpITWtLNTdFM2w2S1NaRFdxVndRVXclM0QlM0Q.