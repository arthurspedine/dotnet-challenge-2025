using Microsoft.ML;
using Motoflow.Trainer;

Console.WriteLine("=== Motoflow ML Model Trainer ===");
Console.WriteLine();

// 1. Inicializar o Contexto do ML.NET
var mlContext = new MLContext(seed: 0);

// 2. Preparar dados de treinamento sintéticos
Console.WriteLine("Preparando dados de treinamento...");
var trainingData = new List<AreaOccupancyData>
{
    // Segunda-feira - alta ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 45, MediaEntradasDia = 20, MediaSaidasDia = 18, DiaSemana = 1, TaxaOcupacao = 90 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 85, MediaEntradasDia = 35, MediaSaidasDia = 30, DiaSemana = 1, TaxaOcupacao = 85 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 60, MediaEntradasDia = 25, MediaSaidasDia = 22, DiaSemana = 1, TaxaOcupacao = 80 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 170, MediaEntradasDia = 50, MediaSaidasDia = 45, DiaSemana = 1, TaxaOcupacao = 85 },
    
    // Terça-feira - alta ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 43, MediaEntradasDia = 19, MediaSaidasDia = 17, DiaSemana = 2, TaxaOcupacao = 86 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 82, MediaEntradasDia = 33, MediaSaidasDia = 28, DiaSemana = 2, TaxaOcupacao = 82 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 58, MediaEntradasDia = 24, MediaSaidasDia = 21, DiaSemana = 2, TaxaOcupacao = 77 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 165, MediaEntradasDia = 48, MediaSaidasDia = 43, DiaSemana = 2, TaxaOcupacao = 82 },
    
    // Quarta-feira - média ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 35, MediaEntradasDia = 15, MediaSaidasDia = 14, DiaSemana = 3, TaxaOcupacao = 70 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 65, MediaEntradasDia = 28, MediaSaidasDia = 25, DiaSemana = 3, TaxaOcupacao = 65 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 48, MediaEntradasDia = 20, MediaSaidasDia = 18, DiaSemana = 3, TaxaOcupacao = 64 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 130, MediaEntradasDia = 40, MediaSaidasDia = 35, DiaSemana = 3, TaxaOcupacao = 65 },
    
    // Quinta-feira - média ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 33, MediaEntradasDia = 14, MediaSaidasDia = 13, DiaSemana = 4, TaxaOcupacao = 66 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 62, MediaEntradasDia = 26, MediaSaidasDia = 24, DiaSemana = 4, TaxaOcupacao = 62 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 45, MediaEntradasDia = 18, MediaSaidasDia = 17, DiaSemana = 4, TaxaOcupacao = 60 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 125, MediaEntradasDia = 38, MediaSaidasDia = 34, DiaSemana = 4, TaxaOcupacao = 62 },
    
    // Sexta-feira - baixa ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 20, MediaEntradasDia = 10, MediaSaidasDia = 12, DiaSemana = 5, TaxaOcupacao = 40 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 45, MediaEntradasDia = 18, MediaSaidasDia = 22, DiaSemana = 5, TaxaOcupacao = 45 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 30, MediaEntradasDia = 12, MediaSaidasDia = 15, DiaSemana = 5, TaxaOcupacao = 40 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 85, MediaEntradasDia = 25, MediaSaidasDia = 30, DiaSemana = 5, TaxaOcupacao = 42 },
    
    // Sábado - muito baixa ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 10, MediaEntradasDia = 5, MediaSaidasDia = 8, DiaSemana = 6, TaxaOcupacao = 20 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 25, MediaEntradasDia = 10, MediaSaidasDia = 15, DiaSemana = 6, TaxaOcupacao = 25 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 18, MediaEntradasDia = 7, MediaSaidasDia = 10, DiaSemana = 6, TaxaOcupacao = 24 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 50, MediaEntradasDia = 15, MediaSaidasDia = 20, DiaSemana = 6, TaxaOcupacao = 25 },
    
    // Domingo - muito baixa ocupação
    new AreaOccupancyData { Capacidade = 50, MotosAtuais = 8, MediaEntradasDia = 4, MediaSaidasDia = 6, DiaSemana = 0, TaxaOcupacao = 16 },
    new AreaOccupancyData { Capacidade = 100, MotosAtuais = 20, MediaEntradasDia = 8, MediaSaidasDia = 12, DiaSemana = 0, TaxaOcupacao = 20 },
    new AreaOccupancyData { Capacidade = 75, MotosAtuais = 15, MediaEntradasDia = 6, MediaSaidasDia = 9, DiaSemana = 0, TaxaOcupacao = 20 },
    new AreaOccupancyData { Capacidade = 200, MotosAtuais = 40, MediaEntradasDia = 12, MediaSaidasDia = 18, DiaSemana = 0, TaxaOcupacao = 20 },
};

Console.WriteLine($"Total de amostras de treinamento: {trainingData.Count}");

// 3. Carregar os dados
var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

// 4. Definir o Pipeline de Treino
Console.WriteLine("Configurando pipeline de treinamento...");
var pipeline = mlContext.Transforms.Concatenate("Features",
        nameof(AreaOccupancyData.Capacidade),
        nameof(AreaOccupancyData.MotosAtuais),
        nameof(AreaOccupancyData.MediaEntradasDia),
        nameof(AreaOccupancyData.MediaSaidasDia),
        nameof(AreaOccupancyData.DiaSemana))
    .Append(mlContext.Regression.Trainers.FastTree(
        labelColumnName: nameof(AreaOccupancyData.TaxaOcupacao),
        numberOfLeaves: 20,
        numberOfTrees: 100));

// 5. Treinar o modelo
Console.WriteLine("Treinando o modelo...");
var model = pipeline.Fit(dataView);
Console.WriteLine("✓ Modelo treinado com sucesso!");

// 6. Salvar o modelo em um ficheiro .zip
var modelPath = "motoflow-ml-model.zip";
mlContext.Model.Save(model, dataView.Schema, modelPath);

Console.WriteLine();
Console.WriteLine($"✓ Modelo salvo como '{modelPath}'");
Console.WriteLine();
Console.WriteLine("PRÓXIMOS PASSOS:");
Console.WriteLine($"1. Copie o arquivo '{modelPath}' para a raiz do projeto Motoflow principal");
Console.WriteLine("2. O MLPredictionService irá carregar automaticamente este modelo");
Console.WriteLine();
Console.WriteLine("Para testar o modelo, você pode executar:");
Console.WriteLine("  dotnet run --project Motoflow.Trainer");
Console.WriteLine();

// 7. Fazer uma previsão de teste
Console.WriteLine("=== Teste de Previsão ===");
var predictionEngine = mlContext.Model.CreatePredictionEngine<AreaOccupancyData, AreaOccupancyPrediction>(model);

var testData = new AreaOccupancyData
{
    Capacidade = 100,
    MotosAtuais = 80,
    MediaEntradasDia = 30,
    MediaSaidasDia = 25,
    DiaSemana = 1 // Segunda-feira
};

var prediction = predictionEngine.Predict(testData);

Console.WriteLine($"Dados de teste:");
Console.WriteLine($"  - Capacidade: {testData.Capacidade}");
Console.WriteLine($"  - Motos Atuais: {testData.MotosAtuais}");
Console.WriteLine($"  - Média Entradas/Dia: {testData.MediaEntradasDia}");
Console.WriteLine($"  - Média Saídas/Dia: {testData.MediaSaidasDia}");
Console.WriteLine($"  - Dia da Semana: {testData.DiaSemana} (Segunda-feira)");
Console.WriteLine();
Console.WriteLine($"Taxa de Ocupação Prevista: {prediction.TaxaOcupacaoPrevista:F2}%");
Console.WriteLine();
