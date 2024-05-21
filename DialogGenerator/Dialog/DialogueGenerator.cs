using DialogGenerator.Data;
using DialogGenerator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;

namespace DialogGenerator.Dialog;

public class DialogueGenerator
    {
        private readonly PredictionEngine<Dialogue, DialoguePrediction> _predictionEngine;
        private readonly AppDbContext _dbContext;
        private readonly NPCService _npcService;

        public DialogueGenerator(ITransformer trainedModel, MLContext mlContext, AppDbContext dbContext, NPCService npcService)
        {
            if (trainedModel == null) throw new ArgumentNullException(nameof(trainedModel));
            if (mlContext == null) throw new ArgumentNullException(nameof(mlContext));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _npcService = npcService ?? throw new ArgumentNullException(nameof(npcService));

            _predictionEngine = mlContext.Model.CreatePredictionEngine<Dialogue, DialoguePrediction>(trainedModel);
        }

        public string GenerateResponse(DialogueRequest dialogueRequest)
        {
            if (dialogueRequest == null || dialogueRequest.Context == null)
                throw new ArgumentException("Invalid DialogueRequest format.");

            var respondingParticipant = dialogueRequest.Context.Participants.FirstOrDefault(p => p.IsResponding);
            var nonRespondingParticipant = dialogueRequest.Context.Participants.FirstOrDefault(p => !p.IsResponding);

            if (respondingParticipant == null || nonRespondingParticipant == null)
                throw new ArgumentException("Participants data is invalid.");

            var quest = _dbContext.Quests
                .Include(q => q.QuestType)
                .Include(q => q.RequiredItems)
                .FirstOrDefault(q => q.Id == dialogueRequest.Context.WorldKnowledge.QuestId);
            var npc = _npcService.GetNPC(nonRespondingParticipant.Id);
            var weather = _dbContext.Weathers.FirstOrDefault(w => w.Id == dialogueRequest.Context.WorldKnowledge.IdWeather);

            if (quest == null || weather == null)
                throw new ArgumentException("Database data is invalid.");

            if (npc == null)
            {
                return _npcService.GetUnknownNPCResponse(respondingParticipant.Name, dialogueRequest.Query);
            }

            if (_npcService.IsWeatherProhibitedForNPC(npc, weather.Id))
            {
                return _npcService.GetUnsuitableWeatherResponse(respondingParticipant.Name, npc.Name, dialogueRequest.Query);
            }

            if (!_npcService.IsTemperatureSuitableForNPC(npc, dialogueRequest.Context.WorldKnowledge.Temperature))
            {
                var isTooHigh = dialogueRequest.Context.WorldKnowledge.Temperature > npc.MaxTemperature;
                return _npcService.GetUnsuitableTemperatureResponse(respondingParticipant.Name, npc.Name, dialogueRequest.Query, dialogueRequest.Context.WorldKnowledge.Temperature, isTooHigh);
            }

            var preferredWeather = _npcService.IsWeatherPreferredByNPC(npc, dialogueRequest.Context.WorldKnowledge.Weather);
            var input = $"{dialogueRequest.Query} ({nonRespondingParticipant.Name} спрашивает у {respondingParticipant.Name}) в квесте '{quest.Name}' (Тип: {quest.QuestType.Name}) при погоде '{dialogueRequest.Context.WorldKnowledge.Weather}'";

            if (!preferredWeather)
            {
                input += $", но {npc.Name} предпочитает погоду '{npc.PreferredWeather}'";
            }

            var dialogue = new Dialogue
            {
                Input = input,
                Quest = quest.Name,
                Environment = $"{dialogueRequest.Context.WorldKnowledge.Weather}, {dialogueRequest.Context.WorldKnowledge.Temperature}°C"
            };

            var prediction = _predictionEngine.Predict(dialogue);
            return prediction.Response;
        }
    }

    public class DialoguePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Response { get; set; }
    }