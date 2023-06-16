# TavernAI card lib .net

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/gitmylo/TavernAICardLib/dotnet-desktop.yml?style=for-the-badge)

<!-- TOC -->
* [TavernAI card lib .net](#tavernai-card-lib-net)
* [Description](#description)
* [Examples](#examples)
  * [Importing](#importing)
  * [Loading](#loading)
  * [Saving](#saving)
  * [Data reading and writing](#data-reading-and-writing)
  * [Image modification](#image-modification)
  * [Card creation](#card-creation)
  * [Card format conversion](#card-format-conversion)
<!-- TOC -->

# Description
TavernAI card lib is a library for loading, modifying and saving TavernAI cards within a dotnet application.

It supports `.json`, `.png`, `.webp`, `.jpg` and `.jpeg` files. Although some of these formats have been untested so far.

# Examples

## Importing
```csharp
using TavernAICardLib;
```

## Loading
* Loading a file from a `.json`
```csharp
TavernAiCard card = TavernAiCard.Load("character.json");
```
* Loading a file from an `image`
```csharp
TavernAiCard card = TavernAiCard.Load("character.png");
```

## Saving
* Saving a `.json`
```csharp
card.Save("character_saved.json");
```

* Saving a `.png`
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

## Card creation
* Create simple card
```csharp
// Instantiating, then setting properties
TavernAiCard card = new TavernAiCard();
// Giving a name
card.Name = "Name";

// Instantiating and declaring properties
TavernAiCard card = new TavernAiCard(){
    Name = "Name"
};
```

* Create card with image from path
```csharp
// Instantiating, then setting properties
TavernAiCard card = new TavernAiCard("path/to/the/image.png");
// Giving a name
card.Name = "Name";

// Instantiating and declaring properties
TavernAiCard card = new TavernAiCard("path/to/the/image.png"){
    Name = "Name"
};
```

* Create card with image bitmap (**Windows only**)
```csharp
// Instantiating, then setting properties
TavernAiCard card = new TavernAiCard(new Bitmap(400, 600));
// Giving a name
card.Name = "Name";

// Instantiating and declaring properties
TavernAiCard card = new TavernAiCard(new Bitmap(400, 600)){
    Name = "Name"
};
```

## Card format conversion
* Converting from `.json` to `.png` with an image from an existing file
```csharp
TavernAiCard card = TavernAiCard.Load("character.json");
card.ImagePath = "path/to/the/image.png";
card.Save("character_saved.png");
```

* Converting from `.json` to `.png` with a bitmap image (**Windows only**)
```csharp
TavernAiCard card = TavernAiCard.Load("character.json");
card.Image = new Bitmap(400, 400);
card.Save("character_saved.png");
```

* Converting from `.png` to `.json`
```csharp
TavernAiCard card = TavernAiCard.Load("character.png");
card.Save("character_saved.json");
```
