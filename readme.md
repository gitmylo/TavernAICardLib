# TavernAI card lib .net

<!-- TOC -->
* [TavernAI card lib .net](#tavernai-card-lib-net)
* [Description](#description)
* [Examples](#examples)
  * [Loading](#loading)
  * [Saving](#saving)
  * [Data reading and writing](#data-reading-and-writing)
  * [Image modification](#image-modification)
<!-- TOC -->

# Description
TavernAI card lib is a library for loading, modifying and saving TavernAI cards within a dotnet application.

It supports .json, .png, .webp, .jpg and .jpeg files. Although some of these formats have been untested so far.

# Examples

## Loading
* Loading a file from a JSON
```csharp
TavernAiCard card = TavernAiCard.Load("character.json");
```
* Loading a file from an image
```csharp
TavernAiCard card = TavernAiCard.Load("character.png");
```

## Saving
* Saving a .json
```csharp
card.Save("character_saved.json");
```

* Saving a .png
```csharp
card.Save("character_saved.png");
```

## Data reading and writing
* Reading the name
```csharp
string name = card.Name;
```

* Writing the name
```csharp
card.Name = "New name";
```

## Image modification
* Create an empty image for the card (**Windows only**).
```csharp
// Check if bitmap image editing is supported, Windows only.
if (TavernAiCard.ImageFullySupported())
{
    // Create a transparent bitmap
    card.Image = new Bitmap(400, 600);
}
```

* Setting the image to another image file
```csharp
// Set Image to null in case it was already set from loading from an image file on Windows.
card.Image = null;
card.ImagePath = "path/to/the/image.png";
```
