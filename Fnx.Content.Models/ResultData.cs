using System.Text.Json.Serialization;

namespace Fnx.Content.Models
{
    public class ResultData<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("incomplete_results")]
        public bool IncompleteResult {get; set; }

    }
}