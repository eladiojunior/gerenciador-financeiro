# Projeto de Web API do Gerenciador Financeiro

Projeto em .NET Core 3.0 com a implementação da camada de backend do Gerenciador Financeiro.

# Executando a WebAPI em Docker

### Criando imagem
```
docker build -t gfin-webapi-image -f gfin.webapi/Dockerfile .
```

### Executando em container 
```
docker run -d --name gfin-webapi -p 8080:80 gfin-webapi-image
```

### Chamar URL
```
http://{servidor-docker}:8080/
```