# Gerenciador Financeiro (Backend)
Projeto em Web API .NET Core 3.0 com a implementação da camada de backend do Gerenciador Financeiro.

## Autenticação e Autorização
Nesta solução iremos utilizar um processo de registro de usuário para utilização na aplicação e utilização para consumo das demais API's, após autenticação que irá retornar um TokenJWT para utilização dentro do backend.

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