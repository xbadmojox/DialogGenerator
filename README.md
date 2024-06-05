Система генерации игровых диалогов
Описание проекта

Этот проект предназначен для разработки системы генерации игровых диалогов в зависимости от структуры квеста и окружающего мира. Система использует нейронные сети и базу данных SQLite для формирования реалистичных диалогов между NPC и игроком.
Структура проекта

    DialogueGenerator: Класс для генерации диалогов на основе входных данных.
    DataLoader: Класс для загрузки и подготовки данных для обучения модели.
    AppDbContext: Класс контекста базы данных для работы с SQLite.
    Models: Папка с моделями данных, включая NPC, Quest, Item и другие.
    TensorFlow: Используется для обучения и работы с нейронной сетью.
    API: Веб-сервер для взаимодействия с системой через API.

Установка и запуск
Предварительные требования

    .NET 6.0 SDK
    SQLite
    ML.NET
    TensorFlow.NET

Установка зависимостей

    Установите .NET 6.0 SDK, следуя инструкциям на официальном сайте .NET.
    Установите SQLite, следуя инструкциям на официальном сайте SQLite.

Клонирование репозитория
```bash
git clone https://github.com/username/repository.git
cd repository
```
Настройка базы данных

Создайте файл базы данных SQLite и примените миграции:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
Запуск веб-сервера

Перейдите в директорию проекта и запустите сервер:
```bash
dotnet run
```
Сервер будет запущен по умолчанию на http://localhost:5000.
API
Получение диалога

Запрос:
POST /api/dialogue
```json
{
  "query": "Где именно находиться дракон?",
  "context": {
    "participants": [
      {
        "id": 12345,
        "name": "Воин",
        "is_responding": false
      },
      {
        "id": 67890,
        "name": "Король",
        "is_responding": true
      }
    ],
    "world_knowledge": {
      "weather": "Ночь",
      "temperature": 0,
      "quest_id": 1001,
      "quest_name": "Сражение с драконом"
    }
  }
}
```
Ответ:
```json
{
  "response": "Дракон находится в горах на северо-востоке. Путь будет опасен ночью, будьте осторожны."
}
```
Заключение

Этот проект демонстрирует, как можно использовать нейронные сети и базы данных для генерации реалистичных игровых диалогов, адаптированных к контексту квестов и окружающего мира.
