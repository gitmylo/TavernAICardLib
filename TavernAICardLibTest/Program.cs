using System.Drawing;
using TavernAICardLib;

// Load the character, this can be .json, .png, .webp, .jpeg or .jpg. If the file format is not supported an exception will be thrown.
TavernAiCard card = TavernAiCard.Load("character.json");

// Example: Load the character from a png card.
// TavernAiCard? card = TavernAiCard.Load("character.png");

// Example: Print the name of the loaded character.
Console.WriteLine(card.Name);

// Example: Set a new name
card.Name = "New name";

// Example: Saving, you can save in .json, .png, .webp, .jpeg or .jpg. The saving format used depends on the file extension.
card.Save("character_saved.json");

// Example: Creating a new image for the character, supported on Windows only.
// Check if full image support is available to create a custom bitmap (only on Windows, on other OS's only image paths are supported.)
if (TavernAiCard.ImageFullySupported())
{
    // Create a transparent bitmap
    card.Image = new Bitmap(400, 600);
    // Save the card as an character card
    card.Save("character_saved.png");
}
