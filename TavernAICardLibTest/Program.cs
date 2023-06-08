using TavernAICardLib;

TavernAiCard? card = TavernAiCard.Load("main_Walter White_tavern.png");

if (card != null)
{
    Console.WriteLine(card.name);
}
