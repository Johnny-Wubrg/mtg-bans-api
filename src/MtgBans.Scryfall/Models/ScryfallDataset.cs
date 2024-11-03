namespace MtgBans.Scryfall.Models;

public class ScryfallDataset<T>
{
  public bool HasMore { get; set; }
  public T[] Data {get; set;}
}