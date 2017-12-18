using AlexaBotframework.BotFrameworkBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Helpers
{
    public static class EntityTypeExtension
    {
        public static string Parse(this EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Age:
                    return Constants.Entities.Builtin_Age;
                case EntityType.Location:
                    return Constants.Entities.Location;
                case EntityType.Opinion:
                    return Constants.Entities.Opinion;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}