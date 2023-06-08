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
    [JsonInclude] [JsonPropertyName("alternate_greetings")] public List<string>? AlternateGreetings { get; set; }
    [JsonInclude] [JsonPropertyName("avatar")] public string? Avatar { get; set; }
    [JsonInclude] [JsonPropertyName("character_version")] public string? CharacterVersion { get; set; }
    [JsonInclude] [JsonPropertyName("creator")] public string? Creator { get; set; }
    [JsonInclude] [JsonPropertyName("creator_notes")] public string? CreatorNotes { get; set; }
    [JsonInclude] [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonInclude] [JsonPropertyName("extensions")] public Extensions? Extensions { get; set; }
    [JsonInclude] [JsonPropertyName("first_mes")] public string? FirstMessage { get; set; }
    [JsonInclude] [JsonPropertyName("mes_example")] public string? MessageExample { get; set; }
    [JsonInclude] [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonInclude] [JsonPropertyName("personality")] public string? Personality { get; set; }
    [JsonInclude] [JsonPropertyName("post_history_instructions")] public string? PostHistoryInstructions { get; set; }
    [JsonInclude] [JsonPropertyName("scenario")] public string? Scenario { get; set; }
    [JsonInclude] [JsonPropertyName("system_prompt")] public string? SystemPrompt { get; set; }
    [JsonInclude] [JsonPropertyName("tags")] public List<string>? Tags { get; set; }

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

public class Extensions
{
    // Not implemented
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
                    TavernAiCard? card = JsonSerializer.Deserialize<TavernAiCard>(jsonData, new JsonSerializerOptions()
                    {
                        IncludeFields = true
                    });
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