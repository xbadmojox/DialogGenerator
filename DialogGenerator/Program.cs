using System;
using System.IO;
using DialogGenerator.Data;
using DialogGenerator.Dialog;
using DialogGenerator.Trainer;
using Microsoft.ML;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер зависимостей
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<NPCService>();

var mlContext = new MLContext();
var dataPath = Path.Combine(Environment.CurrentDirectory, "dialogues.csv");

// Загрузка и подготовка данных
var dataLoader = new DataLoader(dataPath, mlContext);
var dataView = dataLoader.LoadData();

// Обучение модели
var modelTrainer = new ModelTrainer(mlContext);
var trainedModel = modelTrainer.TrainModel(dataView);

// Сохранение модели
var modelPath = Path.Combine(Environment.CurrentDirectory, "model.zip");
modelTrainer.SaveModel(trainedModel, dataView.Schema, modelPath);
Console.WriteLine("Модель обучена и сохранена в " + modelPath);

// Добавление зависимостей для генератора диалогов
builder.Services.AddSingleton(trainedModel);
builder.Services.AddSingleton(mlContext);

builder.Services.AddScoped<DialogueGenerator>(serviceProvider =>
{
    var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
    var npcService = serviceProvider.GetRequiredService<NPCService>();
    return new DialogueGenerator(trainedModel, mlContext, dbContext, npcService);
});

builder.Services.AddControllers();

var app = builder.Build();

// Конфигурация конвейера HTTP-запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();