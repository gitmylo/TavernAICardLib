using TavernAICardLib;

TavernAiCard? card = TavernAiCard.Load("main_Walter White_tavern.png");

if (card != null)
{
    Console.WriteLine(card.Name);
    card.Name = "Waltuh White";
    card.Save("Waltuh White.json");
}
