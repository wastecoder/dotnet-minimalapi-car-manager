# 🚗 Car Manager – Minimal API em .NET

Este projeto foi desenvolvido para **praticar o uso de Minimal APIs no .NET** com foco em **boas práticas de arquitetura, autenticação JWT e testes automatizados**.

A aplicação realiza o **gerenciamento de veículos e administradores**, utilizando o **Entity Framework Core** para persistência e **MySQL** via Docker.  
Também inclui **testes de unidade e integração**, garantindo a qualidade e confiabilidade das operações do sistema.


---


## 📂 Estrutura do Projeto

### API
```
dotnet-minimalapi-car-manager/CarManager/
├── Domain/
│ ├── Entities/
│ ├── Interfaces/
│ └── Services/
├── Infrastructure/
│ └── Database/
├── Migrations/
├── Program.cs
├── Startup.cs
└── appsettings.json
```

### Testes
```
dotnet-minimalapi-car-manager/CarManager.Tests/
├── Domain/
│ ├── Entities/
│ └── Services/
└── Requests/
```

> O ponto de entrada da aplicação é o **Program.cs**, que define as rotas, middlewares e autenticação JWT.  
> Os testes podem ser executados separadamente no projeto **CarManager.Tests**.


---


## 🛠️ Funcionalidades

- [x] **Autenticação JWT**
  - [x] Geração e validação de tokens
  - [x] Controle de acesso por perfil (Adm / Editor)

- [x] **Gerenciamento de Administradores**
  - [x] Cadastro e login
  - [x] Diferenciação de permissões por função
  - [x] Listagem paginada e busca

- [x] **Gerenciamento de Veículos**
  - [x] CRUD completo
  - [x] Validações de negócio
  - [x] Integração com Entity Framework Core

- [x] **Camadas bem definidas**
  - [x] Domain → Entidades e enums
  - [x] Infrastructure → Banco de dados e seed
  - [x] Services → Regras de negócio e validações
  - [x] API → Endpoints com Minimal API

- [x] **Testes Automatizados**
  - [x] Testes de entidades e serviços
  - [x] Banco de teste isolado (`CarManagerDbTest`)
  - [x] Uso de `EnsureCreated()` para criação automática do schema


---


## ⚙️ Tecnologias Utilizadas

- **.NET SDK 9.0** → plataforma principal do projeto
- **C# 12** → linguagem de desenvolvimento
- **Entity Framework Core** → ORM para persistência
- **Docker** → containerização da aplicação (API + MySQL)
- **MySQL** → banco de dados relacional
- **JWT (Json Web Token)** → autenticação e autorização
- **Rider** → IDE utilizada no desenvolvimento
- **MSTest** → testes automatizados


---


## 🧪 Como Executar o Projeto

1. Clone o repositório:
```bash
git clone https://github.com/wastecoder/dotnet-minimalapi-car-manager.git
cd dotnet-minimalapi-car-manager
```

2. Configure o banco de dados com Entity Framework:

Antes de iniciar a aplicação, aplique as migrations para criar o banco de dados:
```bash
cd CarManager
dotnet ef database update
```

3. Suba toda a aplicação (API + MySQL) com Docker:

Na raiz do projeto, execute o comando abaixo para construir e iniciar os containers:
```bash
docker compose up -d --build
```
A aplicação e o banco de dados serão inicializados automaticamente.  
A API estará disponível na porta [8080](http://localhost:8080/swagger).

4. Faça login como administrador para obter o token JWT:

No Swagger, acesse o endpoint `POST /login` e use as credenciais padrão abaixo para autenticação.
```json
{
  "email": "administrador@teste.com",
  "password": "123456"
}
```

O endpoint retornará um **token JWT**.  
Copie-o, clique em **"Authorize"** no topo do Swagger, cole o token (sem aspas e sem "Bearer ") e confirme.  
Assim, você poderá testar todos os endpoints protegidos como administrador.

5. Caso queira parar os containers:
```bash
docker compose down
```

6. **(Opcional)** Executar os testes:
```bash
cd ../CarManager.Tests
dotnet test
```


---


## 🧰 Endpoints Principais

| Método | Rota | Descrição |
|:--|:--|:--|
| **POST** | `/login` | Autenticação e geração de token JWT |
| **GET** | `/administrators` | Lista administradores com paginação |
| **POST** | `/administrators` | Cria novo administrador |
| **GET** | `/vehicles` | Lista todos os veículos |
| **POST** | `/vehicles` | Cadastra novo veículo |
| **PUT** | `/vehicles/{id}` | Atualiza um veículo |
| **DELETE** | `/vehicles/{id}` | Remove um veículo |


---


## 📈 Próximos Passos

- **🧪 Ampliar os testes de integração**
  - Criar testes para os **endpoints de veículos**, semelhantes aos realizados para administradores.  
  - Avaliar a migração do banco de testes:
    - Manter **InMemory** no curto prazo (pela simplicidade e rapidez).  
    - Considerar futuramente o uso de **SQLite InMemory** ou **Testcontainers** (para simular melhor o MySQL real em ambiente de CI/CD).

- **🌱 Automatizar e expandir o seed de dados**
  - Garantir que a criação do banco e a seed rodem automaticamente na inicialização do ambiente de **desenvolvimento** (ex.: via `EnsureCreated()` ou `Migrate()` no `Program.cs`).  
  - Manter o seed atual do administrador padrão.  
  - Adicionar seeds opcionais para **veículos** e outros dados de teste, facilitando a validação e demonstrações da aplicação.
