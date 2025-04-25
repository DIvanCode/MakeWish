using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public class Friendship
{
    public required User FirstUser { get; set; }
    public required User SecondUser { get; set; }
    public required bool IsConfirmed { get; set; }
}