using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using Directory = System.IO.Directory;

namespace TavernAICardLib;

public class TavernAiCard
{
    public Image? Image;
    public string? ImagePath;

    public static bool ImageFullySupported()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
    
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

    public TavernAiCard(string? imagePath, Image? image)
    {
        this.ImagePath = imagePath;
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

    public static TavernAiCard Load(string filePath)
    {
        foreach (var key in _cardLoaders.Keys) foreach (var end in key) if (filePath.ToLower().EndsWith(end)) return _cardLoaders[key].Load(filePath);

        throw new UnsupportedFileFormatException(filePath);
    }

    public void Save(string filePath)
    {
        foreach (var key in _cardSavers.Keys) foreach (var end in key) if (filePath.ToLower().EndsWith(end))
        {
            _cardSavers[key].Save(filePath, this);
            return;
        }

        throw new UnsupportedFileFormatException(filePath);
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

public abstract class CardSaver
{
    public abstract void Save(string filePath, TavernAiCard card);
}

// Loaders

public class ImageCardLoader : CardLoader
{
    public override TavernAiCard Load(string filePath)
    {
        PngReader reader = FileHelper.CreatePngReader(filePath);
        string data = reader.GetMetadata().GetTxtForKey("chara");
        reader.End();
        
        Image? image = null;
        string? imagePath = null;
        if (TavernAiCard.ImageFullySupported())
            image = new Bitmap(filePath);
        else
            imagePath = filePath;

        string jsonData = Encoding.ASCII.GetString(Convert.FromBase64String(data));
        TavernAiCard? card = JsonSerializer.Deserialize<TavernAiCard>(jsonData, new JsonSerializerOptions()
        {
            IncludeFields = true
        });
        if (card != null)
        {
            card.Image = image;
            card.ImagePath = imagePath;
            return card;
        }

        throw new NoMetaDataException(filePath);
    }
}

public class JsonCardLoader : CardLoader
{
    public override TavernAiCard Load(string filePath)
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

// Savers

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
        if (card.Image != null)
            card.Image.Save(filePath);
        else if (card.ImagePath == null)
            throw new NoImageSaveException(filePath);
        else if (card.ImagePath != filePath)
            File.Copy(card.ImagePath, filePath);

        // https://stackoverflow.com/a/32175522/
        String tmpFile = "tmp.png";
        PngReader reader = FileHelper.CreatePngReader(filePath);
        PngWriter writer = FileHelper.CreatePngWriter(tmpFile, reader.ImgInfo, true);
        int chunkBehav = ChunkCopyBehaviour.COPY_ALL_SAFE;
        writer.CopyChunksFirst(reader, chunkBehav);
        PngChunk chara = writer.GetMetadata().SetText("chara", 
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                        JsonSerializer.Serialize(card)
                    )
                )
            );
        chara.Priority = true;

        int channels = reader.ImgInfo.Channels;
        if (channels < 3)
            throw new Exception("Image saving only works with RGB/RGBA images");
        for (int row = 0; row < reader.ImgInfo.Rows; row++)
        {
            ImageLine l1 = reader.ReadRowInt(row); // format: RGBRGB... or RGBARGBA...
            writer.WriteRow(l1, row);
        }
        writer.CopyChunksLast(reader, chunkBehav);
        writer.End();
        reader.End();
        File.Delete(filePath);
        File.Move(tmpFile, filePath);
    }
}

// Exceptions

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

public class NoImageSaveException : Exception
{
    public NoImageSaveException(string fileName)
    {
        Message = $"Failed to save {fileName}, No image or imagepath.";
    }

    public override string Message { get; }
}

public class UnsupportedFileFormatException : Exception
{
    public UnsupportedFileFormatException(string fileName)
    {
        Message = $"Unsupported file format used for {fileName}. Not a supported image or JSON format.";
    }

    public override string Message { get; }
}