using DialogGenerator.Data;
using DialogGenerator.Models;

namespace DialogGenerator.Dialog;

    public class NPCService
    {
        private readonly AppDbContext _dbContext;

        public NPCService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public NPC GetNPC(int id)
        {
            return _dbContext.NPCs.FirstOrDefault(n => n.Id == id);
        }

        public bool IsWeatherPreferredByNPC(NPC npc, string currentWeather)
        {
            if (npc == null) throw new ArgumentNullException(nameof(npc));
            if (string.IsNullOrEmpty(currentWeather)) throw new ArgumentNullException(nameof(currentWeather));

            return npc.PreferredWeather == currentWeather;
        }

        public bool IsWeatherProhibitedForNPC(NPC npc, int currentWeatherId)
        {
            if (npc == null) throw new ArgumentNullException(nameof(npc));

            return npc.ProhibitedWeatherId == currentWeatherId;
        }

        public bool IsTemperatureSuitableForNPC(NPC npc, int currentTemperature)
        {
            if (npc == null) throw new ArgumentNullException(nameof(npc));

            return currentTemperature >= npc.MinTemperature && currentTemperature <= npc.MaxTemperature;
        }

        public string GetUnsuitableWeatherResponse(string playerName, string npcName, string query)
        {
            return $"{playerName}, {npcName} не может говорить в такую погоду.";
        }

        public string GetUnsuitableTemperatureResponse(string playerName, string npcName, string query, int currentTemperature, bool isTooHigh)
        {
            var temperatureType = isTooHigh ? "слишком высокая" : "слишком низкая";
            return $"{playerName}, {npcName} не может говорить при такой температуре. Температура {temperatureType}.";
        }

        public string GetUnknownNPCResponse(string playerName, string query)
        {
            return $"{playerName}, кажется, что этот NPC неизвестен или не хочет отвечать на вопрос '{query}'.";
        }
    }