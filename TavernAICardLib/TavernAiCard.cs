using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MetadataExtractor;

namespace TavernAICardLib;

public class TavernAiCard
{
    public Image image;
    
    
    // Card Fields:
    [JsonPropertyName("alternate_greetings")] public string[] alternateGreetings { get; set; }
    [JsonPropertyName("avatar")] public string avatar { get; set; }
    [JsonPropertyName("character_version")] public string characterVersion { get; set; }
    [JsonPropertyName("creator")] public string creator { get; set; }
    [JsonPropertyName("creator_notes")] public string creatorNotes { get; set; }
    [JsonPropertyName("description")] public string description { get; set; }
    [JsonPropertyName("mes_example")] public string messageExample { get; set; }
    [JsonPropertyName("name")] public string name { get; set; }
    [JsonPropertyName("personality")] public string personality { get; set; }
    [JsonPropertyName("post_history_instructions")] public string postHistoryInstructions { get; set; }
    [JsonPropertyName("scenario")] public string scenario { get; set; }
    [JsonPropertyName("system_prompt")] public string systemPrompt { get; set; }
    [JsonPropertyName("tags")] public string[] tags { get; set; }

    public TavernAiCard(Image image)
    {
        this.image = image;
    }
    
    private static Dictionary<string[], CardLoader> FilePathEndsWith = new Dictionary<string[], CardLoader>()
    {
        {new []{".png", ".webp", ".jpg", ".jpeg"}, new ImageCardLoader()},
    };

    public static TavernAiCard? Load(string filePath)
    {
        foreach (var key in FilePathEndsWith.Keys)
            foreach (var end in key)
                if (filePath.ToLower().EndsWith(end))
                    return FilePathEndsWith[key].Load(filePath);

        return null;
    }
}

public abstract class CardLoader
{
    public abstract TavernAiCard Load(string filePath);
}

public class ImageCardLoader : CardLoader
{
    public override TavernAiCard Load(string filePath)
    {
        Image image = new Bitmap(filePath);
        var directories = ImageMetadataReader.ReadMetadata(filePath);
        foreach (var dir in directories)
        {
            foreach (var tag in dir.Tags)
            {
                string[]? splitDesc = tag.Description?.Split(": ");
                if (splitDesc != null && splitDesc[0] == "chara")
                {
                    string jsonData = Encoding.ASCII.GetString(Convert.FromBase64String(splitDesc[1]));
                    TavernAiCard? card = JsonSerializer.Deserialize<TavernAiCard>(jsonData);
                    if (card != null)
                    {
                        card.image = image;
                        return card;
                    }
                }
            }
        }
        return new TavernAiCard(image);
    }
}