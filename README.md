# Motoflow API

<p align="center">
  <img src="https://github.com/user-attachments/assets/e40d4759-e9d9-48dc-81c8-9f95ed8ab69c" alt="Motoflow Logo" width="128"/>
</p>

## 👥 Integrantes

- **Arthur Spedine**
- **Matheus Esteves**
- **Gabriel Falanga**

## 📋 Descrição do Projeto

O **Motoflow** é uma API RESTful desenvolvida em .NET 8 que implementa um sistema de gestão de motos em pátios. A API segue as melhores práticas REST e inclui recursos avançados como paginação, HATEOAS e documentação OpenAPI completa.

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

## 🚀 Recursos Implementados

### ✅ CRUD Completo
- **Pátios**: Create, Read, Update, Delete
- **Áreas**: Create, Read, Update, Delete  
- **Histórico de Motos**: Create, Read, Update, Delete

### ✅ Boas Práticas REST
- **Status Codes HTTP** apropriados (200, 201, 204, 400, 404, 500)
- **Verbos HTTP** semânticos (GET, POST, PUT, DELETE)
- **Estrutura de URLs** padronizada (`/api/resource/{id}`)

### ✅ Paginação
- Parâmetros `page` e `pageSize` em endpoints de listagem
- Resposta estruturada com metadados de paginação
- Links de navegação (first, last, prev, next)

### ✅ HATEOAS (Hypermedia as the Engine of Application State)
- Links relacionados em cada recurso
- Navegação entre recursos relacionados
- Links de ações disponíveis (self, edit, delete, collection)

### ✅ Documentação OpenAPI/Swagger
- Descrição detalhada de endpoints
- Exemplos de payloads de request/response
- Modelos de dados documentados
- Códigos de resposta explicados

## 🛠️ Tecnologias Utilizadas

- **.NET 8**: Framework web moderno
- **ASP.NET Core Web API**: API RESTful
- **Entity Framework Core**: ORM para persistência
- **Oracle Database**: Banco de dados relacional
- **Swagger/OpenAPI**: Documentação da API

## 📊 Estrutura de Endpoints

### Pátios
```
GET    /api/Patio?page=1&pageSize=10    # Listar pátios (paginado)
GET    /api/Patio/{id}                  # Obter pátio específico
POST   /api/Patio                       # Criar pátio
PUT    /api/Patio/{id}                  # Atualizar pátio
DELETE /api/Patio/{id}                  # Remover pátio
```

### Áreas
```
GET    /api/Area?page=1&pageSize=10     # Listar áreas (paginado)
GET    /api/Area/{id}                   # Obter área específica
POST   /api/Area                        # Criar área
PUT    /api/Area/{id}                   # Atualizar área
DELETE /api/Area/{id}                   # Remover área
```

### Histórico de Motos
```
GET    /api/HistoricoMoto?page=1&pageSize=10        # Listar históricos (paginado)
GET    /api/HistoricoMoto/{id}                      # Obter histórico específico
GET    /api/HistoricoMoto/moto/{motoId}             # Históricos por moto
GET    /api/HistoricoMoto/area/{areaId}             # Históricos por área
POST   /api/HistoricoMoto                           # Registrar entrada
PUT    /api/HistoricoMoto/{id}                      # Registrar saída
DELETE /api/HistoricoMoto/{id}                      # Remover histórico
```

## 📝 Exemplos de Uso

### Criar um Pátio
```json
POST /api/Patio
{
  "nome": "Pátio Central",
  "localizacao": "Centro da cidade"
}
```

### Criar uma Área
```json
POST /api/Area
{
  "identificador": "A1",
  "patioId": 1,
  "capacidadeMaxima": 50
}
```

### Registrar Entrada de Moto
```json
POST /api/HistoricoMoto
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
    "self": "https://api.motoflow.com/api/Patio/1",
    "edit": "https://api.motoflow.com/api/Patio/1",
    "delete": "https://api.motoflow.com/api/Patio/1",
    "collection": "https://api.motoflow.com/api/Patio",
    "areas": "https://api.motoflow.com/api/Area?patioId=1"
  }
}
```

## 🔧 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- Banco de dados Oracle
- Git (opcional)


1. **Clone o repositório**
```bash
git clone https://github.com/arthurspedine/dotnet-challenge-2025.git
cd dotnet-challenge-2025
```

2. **Configure o banco de dados**
```bash
dotnet ef database update
```

3. **Execute a aplicação**
```bash
dotnet run
```

4. **Acesse a documentação**
- OpenAPI JSON: `http://localhost:5186/swagger/index.html`