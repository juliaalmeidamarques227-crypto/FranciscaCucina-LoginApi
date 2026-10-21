# Francisca Cucina — Login API

API REST desenvolvida com C# e ASP.NET Core para cadastro, login e controle de acesso de usuários. O projeto também conta com uma interface web feita em React.

## Funcionalidades

- Cadastro de usuários e verificação de e-mails duplicados.
- Login com e-mail e senha.
- Senhas armazenadas com hash.
- Autenticação por token JWT.
- Rotas protegidas por autenticação e perfil de usuário.
- Interface web para login, cadastro e acesso às áreas restritas.
- Testes automatizados de integração.

## Tecnologias

**Backend:** C#, ASP.NET Core 9, Entity Framework Core 9, SQL Server, JWT e Swagger/OpenAPI.

**Frontend:** React, JavaScript, Vite, Axios e React Router.

**Testes:** xUnit e SQLite em memória.

**Ferramentas:** Git e GitHub.

## Telas do sistema

### Login

![Tela de login](./docs/login.png)

### Dashboard

![Dashboard do sistema](./docs/dashboard.png)

### Área administrativa

![Área administrativa](./docs/admin.png)

## Estrutura do projeto

```text
LoginApi/
├── Controllers/
├── Data/
├── DTOs/
├── LoginApi.Tests/
├── Migrations/
├── Models/
├── Properties/
├── Services/
├── frontend/
├── .gitignore
├── LoginApi.csproj
├── LoginApi.sln
├── Program.cs
└── README.md
```

## Endpoints da API

| Método | Endpoint | Função |
|---|---|---|
| POST | `/api/Usuarios` | Cadastrar usuário |
| POST | `/api/Usuarios/login` | Fazer login e obter token |
| GET | `/api/Usuarios/protegido` | Acessar rota autenticada |
| GET | `/api/Usuarios/admin` | Acessar rota restrita a administradores |

A API utiliza os perfis `Admin` e `Usuario`. O acesso às rotas restritas depende da autenticação e das permissões do usuário.

## Testes automatizados

Os testes de integração verificam sete cenários:

- Cadastro de usuário válido.
- Cadastro com e-mail duplicado.
- Login com credenciais válidas.
- Login com senha incorreta.
- Acesso a rota protegida sem token.
- Tentativa de acesso à rota administrativa por usuário comum.
- Tentativa de cadastro por usuário comum.

Para executar os testes na pasta principal do projeto:

```bash
dotnet test LoginApi.Tests/LoginApi.Tests.csproj
```

## Como executar

### Backend

É necessário ter o SDK do .NET compatível com o projeto e uma instância do SQL Server configurada.

1. Clone o repositório:

```bash
git clone https://github.com/juliaalmeidamarques227-crypto/FranciscaCucina-LoginApi.git
cd FranciscaCucina-LoginApi
```

2. Restaure as dependências:

```bash
dotnet restore
```

3. Configure a conexão com o banco de dados e a chave JWT no ambiente local. Não publique credenciais no repositório.

4. Aplique as migrations:

```bash
dotnet ef database update
```

5. Inicie a API:

```bash
dotnet run
```

### Frontend

Com a API em execução, abra outro terminal e execute:

```bash
cd frontend
npm install
npm run dev
```

O Vite mostrará no terminal o endereço local para abrir a interface.

## Sobre o projeto

Desenvolvi este projeto para praticar o desenvolvimento de APIs com C#, integração com banco de dados, autenticação JWT, controle de permissões e testes automatizados. Também integrei o backend a uma interface em React para permitir a utilização dos principais recursos pelo navegador.
