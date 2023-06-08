using TavernAICardLib;

TavernAiCard? card = TavernAiCard.Load("main_Raiden Shogun and Ei_tavern.png");

if (card != null)
{
    Console.WriteLine(card.Name);
}
