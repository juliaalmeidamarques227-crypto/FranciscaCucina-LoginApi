# Francisca Cucina — Login API

API REST desenvolvida em C# e ASP.NET Core para gerenciamento de usuários, autenticação e autorização.

O projeto foi criado para um sistema interno da Francisca Cucina, com foco em uma estrutura simples, organizada e fácil de manter.

## O que a API faz

* Cadastro de usuários;
* Validação dos dados de cadastro;
* Verificação de e-mail duplicado;
* Hash das senhas;
* Login com e-mail e senha;
* Geração de token JWT;
* Rotas protegidas;
* Controle de acesso por perfil;
* Documentação da API com Swagger/OpenAPI;
* Testes automatizados de integração.

## Tecnologias

* C#
* ASP.NET Core 9
* Entity Framework Core 9
* SQL Server
* SQLite
* JWT Bearer Authentication
* Swagger / OpenAPI
* xUnit
* Git e GitHub

## Estrutura do projeto

```text
LoginApi/
├── Controllers/
│   └── UsuariosController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── CadastroUsuarioDto.cs
│   └── LoginUsuarioDto.cs
│
├── LoginApi.Tests/
│   ├── CustomWebApplicationFactory.cs
│   ├── LoginApi.Tests.csproj
│   └── UsuariosControllerTests.cs
│
├── Migrations/
│
├── Models/
│   └── Usuario.cs
│
├── Services/
│   └── UsuarioService.cs
│
├── Properties/
├── appsettings.json
├── LoginApi.csproj
├── LoginApi.sln
├── Program.cs
└── README.md
```

## Principais endpoints

### Cadastro

```http
POST /api/Usuarios
```

Cria um novo usuário.

### Login

```http
POST /api/Usuarios/login
```

Valida as credenciais e retorna um token JWT.

### Rota protegida

```http
GET /api/Usuarios/protegido
```

Só pode ser acessada com um token válido.

### Área Admin

```http
GET /api/Usuarios/admin
```

Exige autenticação e perfil `Admin`.

## Autenticação

O fluxo de autenticação funciona assim:

```text
Cadastro
   ↓
Senha transformada em hash
   ↓
Usuário salvo no banco
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
Acesso liberado ou negado
```

Os perfis utilizados atualmente são:

```text
Admin
Usuario
```

Um usuário comum consegue se autenticar, mas não tem acesso às rotas que exigem o perfil `Admin`.

## Tratamento de erros

A API trabalha com diferentes respostas HTTP de acordo com cada situação:

* `200` — requisição realizada com sucesso;
* `400` — dados enviados de forma inválida;
* `401` — usuário não autenticado ou senha incorreta;
* `403` — usuário autenticado, mas sem permissão;
* `404` — usuário não encontrado;
* `409` — e-mail já cadastrado.

## Banco de dados

O banco principal da aplicação é o SQL Server, utilizando Entity Framework Core.

As alterações da estrutura do banco são controladas por migrations.

Nos testes de integração, é utilizado SQLite em memória para manter os testes isolados do banco real.

## Segurança

As senhas não são armazenadas diretamente no banco. A API utiliza `PasswordHasher` para gerar o hash da senha.

A chave utilizada para assinar os tokens JWT não fica salva diretamente no código-fonte. Durante o desenvolvimento, a aplicação utiliza ASP.NET Core User Secrets para armazenar a chave.

Exemplo de configuração não sensível:

```json
{
  "Jwt": {
    "Issuer": "LoginApi",
    "Audience": "LoginApi"
  }
}
```

## Testes automatizados

O projeto possui testes de integração com xUnit.

Atualmente são testados os principais fluxos da API:

* cadastro de usuário;
* cadastro com e-mail duplicado;
* login com credenciais válidas;
* login com senha incorreta;
* acesso a rota protegida sem token;
* acesso de usuário comum a uma rota de Admin.

Os testes utilizam uma versão isolada da aplicação e um banco SQLite em memória.

Para executar:

```bash
dotnet test LoginApi.Tests/LoginApi.Tests.csproj
```

## Como executar o projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/juliaalmeidamarques227-crypto/FranciscaCucina-LoginApi.git
cd FranciscaCucina-LoginApi
```

### 2. Restaurar as dependências

```bash
dotnet restore
```

### 3. Configurar o banco

Configure uma instância do SQL Server e informe a sua `ConnectionStrings` de acordo com o ambiente local.

### 4. Configurar a chave JWT

Inicialize o User Secrets:

```bash
dotnet user-secrets init
```

Depois configure a chave:

```bash
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

Depois, a API pode ser explorada pelo Swagger/OpenAPI disponibilizado pela aplicação.

## Objetivo do projeto

Este projeto faz parte do meu portfólio de desenvolvimento backend e foi criado para praticar, em uma aplicação realista, conceitos de:

* desenvolvimento de APIs REST;
* banco de dados;
* Entity Framework Core;
* autenticação;
* autorização;
* JWT;
* segurança de senhas;
* organização de código;
* testes automatizados.

## Próximos passos

* Padronizar melhor as respostas e mensagens de erro;
* continuar evoluindo a estrutura da API;
* criar o frontend em React;
* integrar frontend e backend;
* preparar o projeto para deploy.
