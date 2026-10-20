# Francisca Cucina — Sistema de Login

Projeto desenvolvido para praticar autenticação e controle de acesso utilizando React e C# com ASP.NET Core.

## Funcionalidades

- Login com e-mail e senha;
- Cadastro de usuários;
- Autenticação com token JWT;
- Dashboard com informações do usuário;
- Controle de acesso conforme o perfil;
- Páginas protegidas para usuários autenticados;
- Acesso à área administrativa restrito ao administrador.

## Tecnologias utilizadas

**Frontend**
- React
- Vite
- JavaScript
- Axios
- React Router

**Backend**
- C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT
- xUnit para testes automatizados

## Como executar o projeto

### Backend

1. Abra a pasta do backend no terminal.
2. Execute:

```bash
dotnet run
```

### Frontend

1. Abra a pasta do frontend em outro terminal.
2. Instale as dependências, se necessário:

```bash
npm install
```

3. Inicie o projeto:

```bash
npm run dev
```

O Vite exibirá o endereço local para acessar a aplicação.

## Testes

O backend possui testes automatizados para verificar funcionalidades de cadastro, login, autenticação e autorização.

## Objetivo

Projeto de portfólio desenvolvido para praticar integração entre frontend e backend, autenticação, autorização e testes automatizados.