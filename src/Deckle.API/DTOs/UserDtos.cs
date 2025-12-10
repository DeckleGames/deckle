namespace Deckle.API.DTOs;

public record GoogleUserInfo(
    string GoogleId,
    string Email,
    string? Name,
    string? GivenName,
    string? FamilyName,
    string? Picture,
    string? Locale
);

public record CurrentUserDto
{
    public string? Id { get; init; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? Picture { get; init; }
}
