namespace MtgBans.Services.Extensions;

public static class DateTimeExtensions
{
  public static DateOnly GetValueOrNow(this DateOnly? date) => date ?? DateOnly.FromDateTime(DateTime.Now);
}