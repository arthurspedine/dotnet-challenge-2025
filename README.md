# Motoflow API

<p align="center">
  <img src="https://github.com/user-attachments/assets/e40d4759-e9d9-48dc-81c8-9f95ed8ab69c" alt="Motoflow Logo" width="128"/>
</p>

## 📋 Índice
- [Visão Geral](#-visão-geral)
- [Endpoints](#-endpoints)
- [Instalação](#-instalação)
- [Variáveis de Ambiente](#-variáveis-de-ambiente)
- [Exemplos de Uso](#-exemplos-de-uso)
- [Contribuição](#-contribuição)
- [Licença](#-licença)

## 🌟 Visão Geral

O Motoflow é uma API RESTful para gestão de patios da Mottu, oferecendo:

- Controle de pátios e áreas de estacionamento
- Registro de entrada/saída de motos
- Monitoramento de vagas em tempo real
- Histórico completo de movimentações
- Classificação por tipos de motos (Pop, Sport, Elétrica)

**Tecnologias principais**:
- ASP.NET Core 6.6.2
- Entity Framework Core
- Oracle SQL
- Swagger/OpenAPI

## 🔌 Endpoints

### Áreas `/api/Area`

| Método | Rota                | Descrição                          |
|--------|---------------------|-----------------------------------|
| GET    | `/api/Area`         | Lista todas as áreas              |
| GET    | `/api/Area/{id}`    | Obtém uma área específica         |
| POST   | `/api/Area`         | Cria uma nova área                |
| PUT    | `/api/Area/{id}`    | Atualiza uma área existente       |
| DELETE | `/api/Area/{id}`    | Remove uma área                   |

### Histórico `/api/HistoricoMoto`

| Método | Rota                                | Descrição                                  |
|--------|-------------------------------------|-------------------------------------------|
| GET    | `/api/HistoricoMoto`               | Lista todo o histórico                    |
| GET    | `/api/HistoricoMoto/{id}`          | Obtém um registro específico              |
| GET    | `/api/HistoricoMoto/moto/{motoId}` | Histórico por moto                        |
| GET    | `/api/HistoricoMoto/area/{areaId}` | Histórico por área                        |
| POST   | `/api/HistoricoMoto`               | Registra entrada de uma moto              |
| PUT    | `/api/HistoricoMoto/{id}`          | Registra saída de uma moto                |

### Pátios `/api/Patio`

| Método | Rota              | Descrição                      |
|--------|-------------------|-------------------------------|
| GET    | `/api/Patio`      | Lista todos os pátios          |
| GET    | `/api/Patio/{id}` | Obtém um pátio específico      |
| POST   | `/api/Patio`      | Cria um novo pátio             |
| PUT    | `/api/Patio/{id}` | Atualiza um pátio existente    |
| DELETE | `/api/Patio/{id}` | Remove um pátio                |

## 🛠 Instalação

### Pré-requisitos

- .NET 6.0 SDK
- Banco de dados Oracle
- Git (opcional)

### Passo a passo

1. Clone o repositório:
   ```bash
   git clone https://github.com/arthurspedine/dotnet-challenge-2025.git
   cd motoflow
   ```
2. Configure o banco de dados:
   ```bash
   dotnet ef database update
   ```
3. Execute a aplicação::
   ```bash
   dotnet run
   ```
4. Acesse a documentação:
   ```bash
   http://localhost:5186/swagger/index.html
   ```

## 💡 Exemplos de Uso

### Registrar entrada de moto
```http
POST /api/HistoricoMoto
Content-Type: application/json

{
  "moto": {
    "type": "Sport",
    "placa": "ABC1D23"
  },
  "areaId": 1,
  "observacaoEntrada": "Moto com problema no motor"
}
```

### Consultar Area
```http
GET /api/Area/1
```

### Registrar saída de moto
```http
PUT /api/HistoricoMoto/5
Content-Type: application/json

{
  "observacaoSaida": "Retirada pelo dono"
}
```