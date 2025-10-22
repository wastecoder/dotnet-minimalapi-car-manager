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
- **MySQL (via Docker)** → banco de dados relacional  
- **JWT (Json Web Token)** → autenticação e autorização  
- **Rider** → IDEs utilizadas no desenvolvimento  
- **MSTest** → testes automatizados  


---


## 🧪 Como Executar o Projeto

1. Clone o repositório:

```bash
git clone https://github.com/wastecoder/dotnet-minimalapi-car-manager.git
cd dotnet-minimalapi-car-manager
```

2. Suba o banco de dados com Docker
```
docker compose up -d
```

3. Execute a API
```
cd CarManager
dotnet run
```
A API estará disponível na porta [5054](http://localhost:5054).

4. Executar os testes
```
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

- **🐳 Containerizar totalmente a aplicação**
  - Criar um **Dockerfile** para a API ASP.NET Core.  
  - Configurar um **docker-compose.yml** para orquestrar API e banco MySQL.  
  - Permitir que o sistema completo suba com um único comando:
    ```
    docker compose up
    ```

- **🌱 Automatizar e expandir o seed de dados**
  - Garantir que a criação do banco e a seed rodem automaticamente na inicialização do ambiente de **desenvolvimento** (ex.: via `EnsureCreated()` ou `Migrate()` no `Program.cs`).  
  - Manter o seed atual do administrador padrão.  
  - Adicionar seeds opcionais para **veículos** e outros dados de teste, facilitando a validação e demonstrações da aplicação.
