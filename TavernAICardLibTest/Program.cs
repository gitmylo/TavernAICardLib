using TavernAICardLib;

// Load the character, this can be .json, .png, .webp, .jpeg or .jpg. If the file format is not supported an exception will be thrown.
TavernAiCard card = new TavernAiCard("gaming_cat.jpg", true);

card.Name = "gaming cat";

card.Save("gaming cat character card.png");
