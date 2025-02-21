using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }



        [JsonIgnore]
        public bool Sunday { get; set; }
        [JsonIgnore]
        public bool Monday { get; set; }
        [JsonIgnore]
        public bool Tuesday { get; set; }
        [JsonIgnore]
        public bool Wednesday { get; set; }
        [JsonIgnore]
        public bool Thursday { get; set; }
        [JsonIgnore]
        public bool Friday { get; set; }
        [JsonIgnore]
        public bool Saturday { get; set; }
    }
}
