using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MetadataExtractor;
using Directory = System.IO.Directory;

namespace TavernAICardLib;

public class TavernAiCard
{
    public Image? Image;
    
    
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

    public TavernAiCard(Image? image)
    {
        this.Image = image;
    }

    public static string[] ImageFileTypes = {".png", ".webp", ".jpg", ".jpeg"};
    public static string[] JsonFileTypes = {".json"};
    
    private static Dictionary<string[], CardLoader> _cardLoaders = new()
    {
        {ImageFileTypes, new ImageCardLoader()},
        {JsonFileTypes, new JsonCardLoader()}
    };

    private static Dictionary<string[], CardSaver> _cardSavers = new()
    {
        {ImageFileTypes, new ImageCardSaver()},
        {JsonFileTypes, new JsonCardSaver()}
    };

    public static TavernAiCard? Load(string filePath)
    {
        foreach (var key in _cardLoaders.Keys)
            foreach (var end in key)
                if (filePath.ToLower().EndsWith(end))
                    return _cardLoaders[key].Load(filePath);

        return null;
    }

    public void Save(string filePath)
    {
        foreach (var key in _cardSavers.Keys)
            foreach (var end in key)
                if (filePath.ToLower().EndsWith(end))
                {
                    _cardSavers[key].Save(filePath, this);
                    return;
                }
    }
}

public class Extensions
{
    // Not implemented
}

public abstract class CardLoader
{
    public abstract TavernAiCard? Load(string filePath);
}

public abstract class CardSaver
{
    public abstract void Save(string filePath, TavernAiCard card);
}

public class ImageCardLoader : CardLoader
{
    public override TavernAiCard Load(string filePath)
    {
        Image? image = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            image = new Bitmap(filePath);
        }
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
                        card.Image = image;
                        return card;
                    }
                }
            }
        }

        throw new NoMetaDataException(filePath);
    }
}

public class JsonCardLoader : CardLoader
{
    public override TavernAiCard? Load(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        TavernAiCard? card = JsonSerializer.Deserialize<TavernAiCard>(jsonData, new JsonSerializerOptions()
        {
            IncludeFields = true
        });
        if (card != null)
        {
            return card;
        }

        throw new NoMetaDataException(filePath);
    }
}

public class JsonCardSaver : CardSaver
{
    public override void Save(string filePath, TavernAiCard card)
    {
        var dirName = Path.GetDirectoryName(filePath);
        if (dirName == null) return;
        if (!string.IsNullOrWhiteSpace(dirName)) Directory.CreateDirectory(dirName);
        File.WriteAllText(filePath, JsonSerializer.Serialize(card, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }
}

public class ImageCardSaver : CardSaver
{
    public override void Save(string filePath, TavernAiCard card)
    {
        if (card.Image == null) throw new NoImageException(filePath);
        throw new NotImplementedException();
    }
}

public class NoImageException : Exception
{
    public NoImageException(string fileName)
    {
        Message = $"Could not save {fileName}, Image is missing.";
    }

    public override string Message { get; }
}

public class NoMetaDataException : Exception
{
    public NoMetaDataException(string fileName)
    {
        Message = $"Failed to load {fileName}, No metadata.";
    }

    public override string Message { get; }
}