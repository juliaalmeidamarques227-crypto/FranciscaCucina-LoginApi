# Francisca Cucina — Login API

API REST desenvolvida em C# e ASP.NET Core para gerenciamento de usuários, autenticação e autorização.

O projeto foi criado para um sistema interno da Francisca Cucina, com foco em uma estrutura simples, organizada e fácil de manter.

## Funcionalidades

- Cadastro de usuários com validação dos dados;
- verificação de e-mail duplicado;
- armazenamento de senhas com hash;
- login com e-mail e senha;
- geração de token JWT;
- proteção de rotas;
- controle de acesso por perfil;
- documentação da API com Swagger/OpenAPI;
- testes automatizados de integração.

## Tecnologias utilizadas

- C#
- ASP.NET Core 9
- Entity Framework Core 9
- SQL Server
- SQLite em memória para os testes
- JWT Bearer Authentication
- Swagger / OpenAPI
- xUnit
- Git e GitHub

## Estrutura do projeto

```text
LoginApi/
├── Controllers/
│   └── UsuariosController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── CadastroUsuarioDto.cs
│   └── LoginUsuarioDto.cs
├── LoginApi.Tests/
│   ├── CustomWebApplicationFactory.cs
│   ├── LoginApi.Tests.csproj
│   └── UsuariosControllerTests.cs
├── Migrations/
├── Models/
│   └── Usuario.cs
├── Services/
│   └── UsuarioService.cs
├── Properties/
├── appsettings.json
├── LoginApi.csproj
├── LoginApi.sln
├── Program.cs
└── README.md
```

## Principais endpoints

### Cadastro

`POST /api/Usuarios`

Cria um novo usuário.

### Login

`POST /api/Usuarios/login`

Valida as credenciais e retorna um token JWT.

### Rota protegida

`GET /api/Usuarios/protegido`

Exige autenticação com um token válido.

### Área administrativa

`GET /api/Usuarios/admin`

Exige autenticação e perfil `Admin`.

## Autenticação e autorização

O fluxo de autenticação funciona da seguinte forma:

1. O usuário realiza o cadastro.
2. A senha é transformada em hash.
3. Os dados são armazenados no banco de dados.
4. O usuário realiza o login.
5. A API valida as credenciais.
6. Um token JWT é gerado.
7. O cliente envia o token nas próximas requisições.
8. A API valida o token e as permissões antes de liberar o acesso.

Os perfis utilizados atualmente são:

- `Admin`
- `Usuario`

Um usuário comum pode se autenticar, mas não tem acesso às rotas que exigem o perfil de administrador.

## Tratamento de erros

A API utiliza respostas HTTP de acordo com cada situação, incluindo:

- `200` — requisição realizada com sucesso;
- `400` — dados inválidos;
- `401` — usuário não autenticado ou credenciais inválidas;
- `403` — usuário sem permissão;
- `404` — recurso não encontrado;
- `409` — e-mail já cadastrado.

## Banco de dados

O banco principal da aplicação é o SQL Server, acessado por meio do Entity Framework Core.

As alterações na estrutura do banco são controladas por migrations.

Nos testes de integração, é utilizado SQLite em memória para manter os testes isolados do banco de dados principal.

## Segurança

As senhas não são armazenadas diretamente no banco. A API utiliza `PasswordHasher` para gerar e verificar o hash das senhas.

A chave usada para assinar os tokens JWT deve ser mantida fora do código-fonte. Durante o desenvolvimento, o ASP.NET Core User Secrets pode ser utilizado para armazená-la.

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

O projeto possui testes de integração com xUnit para verificar os principais fluxos da API, incluindo:

- cadastro de usuário;
- cadastro com e-mail duplicado;
- login com credenciais válidas;
- login com senha incorreta;
- acesso a rota protegida sem autenticação;
- tentativa de acesso de um usuário comum a uma rota administrativa.

Os testes utilizam uma versão isolada da aplicação e um banco SQLite em memória.

Para executar os testes, utilize:

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

### 3. Configurar o banco de dados

Configure uma instância do SQL Server e ajuste a string de conexão de acordo com o ambiente local.

### 4. Configurar a chave JWT

Inicialize o User Secrets, caso ainda não esteja configurado:

```bash
dotnet user-secrets init
```

Configure uma chave secreta própria para o ambiente local:

```bash
dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_SECRETA"
```

Substitua `SUA_CHAVE_SECRETA` por uma chave apropriada para desenvolvimento. Não publique uma chave real no repositório.

### 5. Aplicar as migrations

Com a conexão com o banco configurada, execute:

```bash
dotnet ef database update
```

Esse comando requer que a ferramenta `dotnet-ef` esteja instalada em uma versão compatível com o Entity Framework Core utilizado pelo projeto.

### 6. Executar a API

```bash
dotnet run
```

A API pode ser explorada pelo Swagger/OpenAPI, conforme a configuração da aplicação.

## Objetivo do projeto

Este projeto faz parte do meu portfólio de desenvolvimento backend e foi criado para praticar conceitos de APIs REST, banco de dados, autenticação, autorização, JWT, segurança de senhas, organização de código e testes automatizados.

## Próximos passos

- Melhorar a padronização das respostas e mensagens de erro;
- aprimorar a validação dos dados recebidos pela API;
- continuar evoluindo a documentação do projeto;
- preparar a aplicação para deploy.