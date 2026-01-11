namespace FritzPtr.Core.FritzBox;

public sealed class FritzBoxClientOptions
{
    public const string SECTION_NAME = "FritzBoxClient";

    public string Host { get; init; } = "fritz.box";
    public required string Username { get; set; } = "admin";
    public required string Password { get; set; }
}
