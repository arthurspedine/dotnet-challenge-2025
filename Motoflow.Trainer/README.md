# Motoflow.Trainer

Projeto de treinamento do modelo de Machine Learning para previsão de ocupação de áreas no sistema Motoflow.

## Descrição

Este projeto é responsável por treinar o modelo de ML usando ML.NET com o algoritmo FastTree Regression. O modelo prevê a taxa de ocupação de áreas de estacionamento com base em:

- Capacidade total da área
- Número atual de motos
- Média de entradas por dia
- Média de saídas por dia
- Dia da semana (0-6, onde 0 = Domingo)

## Como usar

### 1. Treinar o modelo

Execute o projeto para treinar e gerar o arquivo do modelo:

```bash
dotnet run --project Motoflow.Trainer
```

Isso irá:
- Treinar o modelo com dados sintéticos
- Salvar o modelo em `motoflow-ml-model.zip`
- Executar uma previsão de teste

### 2. Copiar o modelo para a API

O modelo gerado precisa estar na raiz do projeto principal Motoflow:

```bash
cp Motoflow.Trainer/motoflow-ml-model.zip ./
```

### 3. Executar a API

A API Motoflow irá carregar automaticamente o modelo ao iniciar:

```bash
dotnet run
```

## Estrutura do Projeto

```
Motoflow.Trainer/
├── Program.cs              # Código de treinamento do modelo
├── AreaOccupancyData.cs   # Classes de dados para ML
├── Motoflow.Trainer.csproj
└── README.md
```

## Dados de Treinamento

O modelo é treinado com 28 amostras sintéticas que simulam padrões de ocupação ao longo da semana:

- **Segunda-feira**: Alta ocupação (80-90%)
- **Terça-feira**: Alta ocupação (77-86%)
- **Quarta-feira**: Média ocupação (64-70%)
- **Quinta-feira**: Média ocupação (60-66%)
- **Sexta-feira**: Baixa ocupação (40-45%)
- **Sábado**: Muito baixa ocupação (20-25%)
- **Domingo**: Muito baixa ocupação (16-20%)

## Tecnologias

- .NET 8.0
- ML.NET 3.0.1
- ML.NET FastTree 3.0.1

## Output

O modelo treinado é salvo como `motoflow-ml-model.zip` e deve ter aproximadamente 20-50 KB.
