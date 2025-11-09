# Motoflow API

<p align="center">
  <img src="https://github.com/user-attachments/assets/e40d4759-e9d9-48dc-81c8-9f95ed8ab69c" alt="Motoflow Logo" width="128"/>
</p>

## 👥 Integrantes

- **Arthur Spedine**
- **Matheus Esteves**
- **Gabriel Falanga**

## 📋 Descrição do Projeto

O **Motoflow** é uma API RESTful desenvolvida em .NET 8 que implementa um sistema de gestão de motos em pátios. A API segue as melhores práticas REST e inclui recursos avançados como **versionamento de API**, **paginação**, **HATEOAS** e **documentação OpenAPI completa**.

## ✨ Recursos Principais

### ✅ Versionamento de API
- **Versão 1.0**: Endpoints básicos com funcionalidades essenciais
- **Versão 2.0**: Busca flexível de motos (ID, Placa, Chassi, QR Code)
- **Retrocompatibilidade**: Versões anteriores continuam funcionais
- **Documentação Separada**: Cada versão tem sua própria documentação Swagger

### ✅ CRUD Completo
- **Pátios**: Create, Read, Update, Delete
- **Áreas**: Create, Read, Update, Delete  
- **Histórico de Motos**: Create, Read, Update, Delete

### ✅ Boas Práticas REST
- **Status Codes HTTP** apropriados (200, 201, 204, 400, 404, 500)
- **Verbos HTTP** semânticos (GET, POST, PUT, DELETE)
- **Estrutura de URLs** padronizada com versionamento

### ✅ Paginação
- Parâmetros `page` e `pageSize` em endpoints de listagem
- Resposta estruturada com metadados de paginação
- Links de navegação (first, last, prev, next)

### ✅ HATEOAS (Hypermedia as the Engine of Application State)
- Links relacionados em cada recurso
- Navegação entre recursos relacionados
- Links de ações disponíveis (self, edit, delete, collection)

### ✅ Documentação OpenAPI/Swagger
- Documentação separada por versão
- Descrição detalhada de endpoints
- Exemplos de payloads de request/response
- Modelos de dados documentados
- Códigos de resposta explicados

### ✅ Machine Learning
- Previsão de ocupação de áreas usando ML.NET
- Modelo treinado em projeto separado
- Endpoint dedicado para predições

## 🏗️ Arquitetura do Domínio

### Entidades Principais

1. **Pátio** - Locais físicos onde as motos são armazenadas
   - Representa estabelecimentos como "Pátio Central", "Pátio Norte"
   - Contém múltiplas áreas organizacionais

2. **Área** - Subdivisões dentro dos pátios com capacidade limitada
   - Organiza o espaço em seções como "Área A1", "Área B2"
   - Controla ocupação e disponibilidade de vagas

3. **HistoricoMoto** - Registro de movimentação das motos
   - Documenta entrada e saída de veículos nas áreas
   - Mantém histórico completo de permanência

### Justificativa das Entidades

- **Escalabilidade**: Permite expansão para múltiplos pátios
- **Organização**: Facilita localização e gestão de veículos
- **Auditoria**: Mantém histórico completo de movimentações
- **Capacidade**: Controla ocupação e disponibilidade em tempo real

## 🛠️ Tecnologias Utilizadas

- **.NET 8**: Framework web moderno
- **ASP.NET Core Web API**: API RESTful
- **Entity Framework Core**: ORM para persistência
- **Oracle Database**: Banco de dados relacional
- **Swagger/OpenAPI**: Documentação da API
- **ML.NET**: Machine Learning para previsão de ocupação
- **API Versioning**: Versionamento de endpoints

## 🔄 Versionamento da API

A API utiliza **versionamento por URL** para garantir retrocompatibilidade e evolução controlada:

### 📌 Versões Disponíveis

#### **Versão 1.0** (Atual)
Versão inicial com funcionalidades básicas.

**Endpoints:**
- `GET /api/v1/HistoricoMoto/moto/{motoId}` - Busca históricos **apenas por ID numérico**

#### **Versão 2.0**
Versão aprimorada com busca flexível de motos.

**Novidades:**
- `GET /api/v2/HistoricoMoto/moto/{moto}` - Busca históricos por:
  - **ID numérico** (ex: `123`)
  - **Placa** (ex: `ABC1234`)
  - **Chassi** (ex: `9BWZZZ377VT004251`)
  - **QR Code** (ex: `QR123456789`)

### 🎯 Exemplos de Uso

**Versão 1.0 - Apenas ID:**
```bash
GET /api/v1/HistoricoMoto/moto/123
```

**Versão 2.0 - Busca Flexível:**
```bash
# Por ID
GET /api/v2/HistoricoMoto/moto/123

# Por Placa
GET /api/v2/HistoricoMoto/moto/ABC1234

# Por Chassi
GET /api/v2/HistoricoMoto/moto/9BWZZZ377VT004251

# Por QR Code
GET /api/v2/HistoricoMoto/moto/QR123456789
```

### 📚 Documentação Swagger

Cada versão possui sua própria documentação no Swagger:
- **v1.0**: http://localhost:5186/swagger/index.html (selecione "Motoflow API - v1")
- **v2.0**: http://localhost:5186/swagger/index.html (selecione "Motoflow API - v2")

### 🔧 Implementação Técnica

- **Padrão**: URL Segment Versioning (`/api/v{version}/...`)
- **Biblioteca**: `Microsoft.AspNetCore.Mvc.Versioning`
- **Estratégia**: Versionamento explícito na rota
- **Compatibilidade**: Versões anteriores permanecem funcionais

## 📁 Estrutura do Projeto

```
dotnet-challenge-2025/
├── Motoflow.Web/              # API Web principal
│   ├── Controllers/           # Controladores da API
│   ├── Services/              # Lógica de negócio
│   ├── Repositories/          # Acesso a dados
│   ├── Models/                # Entidades e DTOs
│   ├── Data/                  # Contexto do banco de dados
│   ├── Migrations/            # Migrações do EF Core
│   ├── Properties/            # Configurações de ambiente
│   ├── Program.cs             # Ponto de entrada da aplicação
│   ├── appsettings.json       # Configurações
│   └── motoflow-ml-model.zip  # Modelo ML treinado
│
├── Motoflow.Trainer/          # Projeto de treinamento ML
│   ├── Program.cs             # Treina o modelo
│   ├── AreaOccupancyData.cs   # Classes de dados para ML
│   └── README.md              # Documentação do Trainer
│
├── Motoflow.sln               # Solution principal
└── README.md                  # Este arquivo
```

## 📊 Estrutura de Endpoints

> **Nota**: Os endpoints abaixo usam o formato sem versionamento para brevidade. Para usar versões específicas, adicione `/v1` ou `/v2` após `/api` (ex: `/api/v1/Patio`).

### Pátios
```
GET    /api/v1/Patio?page=1&pageSize=10    # Listar pátios (paginado)
GET    /api/v1/Patio/{id}                  # Obter pátio específico
POST   /api/v1/Patio                       # Criar pátio
PUT    /api/v1/Patio/{id}                  # Atualizar pátio
DELETE /api/v1/Patio/{id}                  # Remover pátio
```

### Áreas
```
GET    /api/v1/Area?page=1&pageSize=10     # Listar áreas (paginado)
GET    /api/v1/Area/{id}                   # Obter área específica
POST   /api/v1/Area                        # Criar área
PUT    /api/v1/Area/{id}                   # Atualizar área
DELETE /api/v1/Area/{id}                   # Remover área
```

### Histórico de Motos

#### Versão 1.0
```
GET    /api/v1/HistoricoMoto?page=1&pageSize=10    # Listar históricos (paginado)
GET    /api/v1/HistoricoMoto/{id}                  # Obter histórico específico
GET    /api/v1/HistoricoMoto/moto/{motoId}         # Históricos por moto (apenas ID)
GET    /api/v1/HistoricoMoto/area/{areaId}         # Históricos por área
POST   /api/v1/HistoricoMoto                       # Registrar entrada
PUT    /api/v1/HistoricoMoto/{id}                  # Registrar saída
DELETE /api/v1/HistoricoMoto/{id}                  # Remover histórico
```

#### Versão 2.0
```
GET    /api/v2/HistoricoMoto?page=1&pageSize=10    # Listar históricos (paginado)
GET    /api/v2/HistoricoMoto/{id}                  # Obter histórico específico
GET    /api/v2/HistoricoMoto/moto/{moto}           # Históricos por ID/Placa/Chassi/QR
GET    /api/v2/HistoricoMoto/area/{areaId}         # Históricos por área
POST   /api/v2/HistoricoMoto                       # Registrar entrada
PUT    /api/v2/HistoricoMoto/{id}                  # Registrar saída
DELETE /api/v2/HistoricoMoto/{id}                  # Remover histórico
```

## 📝 Exemplos de Uso

### Criar um Pátio
```json
POST /api/v1/Patio
{
  "nome": "Pátio Central",
  "localizacao": "Centro da cidade"
}
```

### Criar uma Área
```json
POST /api/v1/Area
{
  "identificador": "A1",
  "patioId": 1,
  "capacidadeMaxima": 50
}
```

### Registrar Entrada de Moto
```json
POST /api/v2/HistoricoMoto
{
  "moto": {
    "type": "Scooter",
    "placa": "ABC1234"
  },
  "areaId": 1,
  "observacaoEntrada": "Moto em bom estado"
}
```

### Resposta com HATEOAS
```json
{
  "id": 1,
  "nome": "Pátio Central",
  "localizacao": "Centro da cidade",
  "links": {
    "self": "https://api.motoflow.com/api/v1/Patio/1",
    "edit": "https://api.motoflow.com/api/v1/Patio/1",
    "delete": "https://api.motoflow.com/api/v1/Patio/1",
    "collection": "https://api.motoflow.com/api/v1/Patio",
    "areas": "https://api.motoflow.com/api/v1/Area?patioId=1"
  }
}
```

## 🔧 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- Banco de dados Oracle
- Git (opcional)

### Passos

1. **Clone o repositório**
```bash
git clone https://github.com/arthurspedine/dotnet-challenge-2025.git
cd dotnet-challenge-2025
```

2. **Configure o banco de dados**

Atualize a connection string no arquivo `Motoflow.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=seu_usuario;Password=sua_senha;Data Source=seu_servidor"
  }
}
```

3. **Execute as migrações**
```bash
cd Motoflow.Web
dotnet ef database update
```

4. **(Opcional) Retreine o modelo ML**
```bash
cd ../Motoflow.Trainer
dotnet run
cd ../Motoflow.Web
```

5. **Execute a aplicação**
```bash
dotnet run --project Motoflow.Web
```

6. **Acesse a documentação**
- Swagger UI: `http://localhost:5186/swagger/index.html`
  - Selecione **"Motoflow API - v1"** ou **"Motoflow API - v2"** no dropdown do Swagger
- API Base v1.0: `http://localhost:5186/api/v1`
- API Base v2.0: `http://localhost:5186/api/v2` (recomendada)

## 🤖 Machine Learning

O projeto inclui um sistema de previsão de ocupação de áreas usando ML.NET:

- **Modelo**: FastTree Regression
- **Features**: Capacidade, motos atuais, média de entradas/saídas, dia da semana
- **Predição**: Taxa de ocupação esperada (0-100%)

### Treinamento do Modelo

O treinamento do modelo ML foi separado em um projeto dedicado (`Motoflow.Trainer`), seguindo o princípio de responsabilidade única:

```bash
cd Motoflow.Trainer
dotnet run
```

Isso gera o arquivo `motoflow-ml-model.zip` que é consumido pela API principal. O modelo já está pré-treinado e incluído no projeto Web.

### Endpoint de Predição

```
POST /api/v1/MLPrediction
{
  "capacidadeMaxima": 50,
  "motosAtuais": 30,
  "mediaEntradasDiarias": 15.5,
  "mediaSaidasDiarias": 12.3,
  "diaDaSemana": 2
}
```

## 🧪 Testes

O projeto implementa testes automatizados em múltiplas camadas usando xUnit, Moq e WebApplicationFactory.

### Como Executar os Testes

```bash
# Executar todos os testes
dotnet test

# Executar com verbosidade detalhada
dotnet test --verbosity detailed

# Executar apenas testes unitários
dotnet test --filter "FullyQualifiedName~Unit"

# Executar apenas testes de integração
dotnet test --filter "FullyQualifiedName~Integration"
```

### Estrutura de Testes

```
Motoflow.Tests/
├── Unit/                      # Testes unitários (mocking)
│   └── Services/
│       ├── AreaServiceTests.cs
│       └── HistoricoMotoServiceTests.cs
└── Integration/               # Testes de integração (E2E)
    └── HistoricoMotoControllerIntegrationTests.cs
```

### 🎯 Banco de Dados em Memória vs Oracle

O projeto está configurado para usar **diferentes bancos de dados** dependendo do ambiente:

#### Desenvolvimento/Produção: Oracle
```bash
# Executa com Oracle Database
dotnet run --project Motoflow.Web
```

#### Testes: InMemory Database
```bash
# Testes usam automaticamente InMemory
dotnet test
```

**Como funciona?**

1. **`Program.cs`** detecta a configuração `UseInMemoryDatabase`:
   ```csharp
   // Se true: usa InMemory
   // Se false: usa Oracle
   var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase", false);
   ```

2. **`appsettings.Test.json`** define `UseInMemoryDatabase = true`
3. **WebApplicationFactory** configura o ambiente como "Test"
4. **Resultado**: Testes rodam em memória, sem necessidade de Oracle! ✅

### Cobertura de Testes

#### ✅ Testes Unitários 

**AreaServiceTests** - 5 testes
- ✅ GetPagedAreasAsync_ShouldReturnPagedResult
- ✅ CreateAreaAsync_WithValidData_ShouldCreateArea
- ✅ UpdateAreaAsync_WithValidData_ShouldUpdateArea
- ✅ DeleteAreaAsync_WithExistingArea_ShouldReturnTrue
- ✅ DeleteAreaAsync_WithNonExistentArea_ShouldReturnFalse

**HistoricoMotoServiceTests** - 5 testes
- ✅ GetHistoricoByIdAsync_WithExistingId_ShouldReturnHistorico
- ✅ GetHistoricoByIdAsync_WithNonExistentId_ShouldReturnNull
- ✅ DeleteHistoricoAsync_WithExistingHistorico_ShouldReturnTrue
- ✅ DeleteHistoricoAsync_WithNonExistentHistorico_ShouldReturnFalse
- ✅ GetAllHistoricosAsync_ShouldReturnPagedResult

#### ✅ Testes de Integração

**HistoricoMotoControllerIntegrationTests**
- ✅ GetHistoricos_ShouldReturnPagedResult
- ✅ PostHistorico_WithNonExistentArea_ShouldReturnNotFound
- ✅ PostHistorico_WithValidData_ShouldReturnCreated
- ✅ PutHistorico_WithValidSaida_ShouldReturnOk
- ✅ DeleteHistorico_WithExistingId_ShouldReturnNoContent

### Resultados dos Testes

```bash
$ dotnet test

Test summary: total: 15, failed: 0, succeeded: 15, skipped: 0
✅ 100% de cobertura dos casos de teste implementados
⚡ Tempo de execução: ~4 segundos
```

## 🏛️ Arquitetura e Padrões

### Princípios SOLID Aplicados

#### Single Responsibility Principle (SRP)
- **Controllers**: Apenas responsáveis por receber requisições HTTP e retornar respostas
- **Services**: Contêm toda a lógica de negócio (paginação, DTOs, HATEOAS)
- **Repositories**: Exclusivamente para acesso a dados
- **Separação ML**: Treinamento do modelo em projeto separado

#### Dependency Inversion Principle (DIP)
- Services dependem de interfaces (`IAreaRepository`, `IHistoricoMotoRepository`, `IPatioRepository`)
- Facilita testes unitários através de mocking
- Baixo acoplamento entre camadas

### Padrões de Projeto

- **Repository Pattern**: Abstração da camada de dados
- **Service Layer**: Centralização da lógica de negócio
- **DTO Pattern**: Separação entre entidades de domínio e modelos de API
- **Factory Pattern**: `WebApplicationFactory` para testes de integração

### Organização em Camadas

```
Presentation Layer (Controllers)
        ↓
Business Logic Layer (Services)
        ↓
Data Access Layer (Repositories)
        ↓
Database (Oracle)
```

## 🔐 Autenticação e Autorização

O projeto implementa autenticação JWT (JSON Web Tokens):

### Registro de Usuário
```json
POST /api/v1/Auth/register
{
  "username": "usuario",
  "password": "senha123",
  "email": "usuario@email.com"
}
```

### Login
```json
POST /api/v1/Auth/login
{
  "username": "usuario",
  "password": "senha123"
}
```

### Resposta com Token
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "usuario",
  "expiresAt": "2025-11-07T15:30:00Z"
}
```

### Usando o Token

Adicione o token no header de requisições protegidas:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📊 Banco de Dados

### Migrations

O projeto usa Entity Framework Core Migrations para versionamento do banco:

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project Motoflow.Web

# Aplicar migrations
dotnet ef database update --project Motoflow.Web

# Reverter última migration
dotnet ef migrations remove --project Motoflow.Web

# Ver histórico de migrations
dotnet ef migrations list --project Motoflow.Web
```