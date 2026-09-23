# LoginApi

API REST desenvolvida em ASP.NET Core para gerenciamento de usuários, autenticação e autorização.

O projeto foi desenvolvido com foco em boas práticas de backend, incluindo persistência de dados, hash de senhas, autenticação com JWT e controle de acesso por perfil.

## Funcionalidades

* Cadastro de usuários;
* Validação de dados;
* Verificação de e-mail duplicado;
* Hash seguro de senhas;
* Login com e-mail e senha;
* Geração de JWT;
* Rotas protegidas por autenticação;
* Perfis de acesso (`Admin` e `Usuario`);
* Autorização de rotas por perfil;
* Documentação e testes pelo Swagger/OpenAPI.

## Tecnologias utilizadas

* C#
* ASP.NET Core 9
* Entity Framework Core 9
* SQL Server
* JWT Bearer Authentication
* Swagger / OpenAPI
* Git e GitHub

## Estrutura do projeto

```text
LoginApi/
├── Controllers/
│   └── UsuariosController.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── Models/
│   ├── CadastroUsuario.cs
│   ├── LoginUsuario.cs
│   └── Usuario.cs
├── Properties/
├── appsettings.json
├── Program.cs
├── LoginApi.csproj
└── README.md
```

## Principais endpoints

### Cadastro

`POST /api/Usuarios`

Cria um novo usuário.

### Login

`POST /api/Usuarios/login`

Valida as credenciais e retorna um JWT.

### Rota protegida

`GET /api/Usuarios/protegido`

Exige um token JWT válido.

### Rota Admin

`GET /api/Usuarios/admin`

Exige um JWT válido e o perfil `Admin`.

## Autenticação e autorização

O fluxo de autenticação funciona da seguinte forma:

```text
Cadastro
   ↓
Senha transformada em hash
   ↓
Usuário salvo no SQL Server
   ↓
Login
   ↓
Validação da senha
   ↓
JWT gerado
   ↓
Token enviado nas próximas requisições
   ↓
API valida o token
   ↓
Permissão liberada ou negada
```

Os perfis utilizados no projeto são:

* `Admin`
* `Usuario`

Exemplo:

```text
Admin → acesso à rota /admin
Usuario → acesso negado à rota /admin
```

## Respostas de autenticação

O projeto trata diferentes situações de erro:

| Situação               | HTTP |
| ---------------------- | ---: |
| Requisição válida      |  200 |
| Dados inválidos        |  400 |
| Usuário não encontrado |  404 |
| Senha incorreta        |  401 |
| Token inválido         |  401 |
| Usuário sem permissão  |  403 |
| E-mail duplicado       |  409 |

## Banco de dados

O projeto utiliza **SQL Server** com **Entity Framework Core**.

As alterações da estrutura do banco são controladas por migrations.

## Configuração

A chave utilizada pelo JWT não fica armazenada diretamente no código-fonte.

Durante o desenvolvimento, o projeto utiliza **ASP.NET Core User Secrets** para armazenar a chave privada do JWT.

O `appsettings.json` contém apenas as configurações não sensíveis, como:

```json
{
  "Jwt": {
    "Issuer": "LoginApi",
    "Audience": "LoginApi"
  }
}
```

## Como executar

### 1. Clonar o projeto

```bash
git clone https://github.com/juliaalmeidamarques227-crypto/FranciscaCucina-LoginApi.git
cd LoginApi
```

### 2. Restaurar as dependências

```bash
dotnet restore
```

### 3. Configurar o banco

Configure uma instância do SQL Server e ajuste a `ConnectionStrings` do projeto conforme seu ambiente.

### 4. Configurar a chave JWT

Crie a chave usando User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_SECRETA"
```

### 5. Aplicar as migrations

```bash
dotnet ef database update
```

### 6. Executar a API

```bash
dotnet run
```

Depois, acesse o Swagger disponibilizado pela aplicação.

## Objetivo do projeto

Este projeto foi desenvolvido como parte do meu portfólio de backend para praticar conceitos importantes de desenvolvimento de APIs, banco de dados, autenticação, autorização e segurança.

## Próximos passos

* Melhorar a padronização das respostas da API;
* adicionar testes automatizados;
* separar responsabilidades em serviços;
* implementar frontend para consumo da API;
* evoluir a documentação;
* preparar o projeto para deploy.


