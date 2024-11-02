namespace MtgBans.Exceptions;

public class InvalidEntryOperation(string field, string value) : Exception
{
  public override string Message => $"\"{value}\" is not a valid entry for field \"{field}\"";
}