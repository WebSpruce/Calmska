using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Calmska.Models.Models
{
    public class Settings
    {
        [Key]
        [BsonElement("_id")]
        public Guid SettingsId { get; set; }
        public required string SettingsJson { get; set; } = string.Empty;
        public required Guid UserId { get; set; }
        public Settings()
        {
            SettingsId = Guid.NewGuid();
        }
    }
}
