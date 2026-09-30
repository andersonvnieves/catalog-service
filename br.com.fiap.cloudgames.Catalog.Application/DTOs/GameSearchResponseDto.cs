namespace br.com.fiap.cloudgames.Catalog.Application.DTOs;

public class GameSearchResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string Franchise { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public int AgeRating { get; set; }
    public List<string> GameModes { get; set; } = new();
    public string? PublisherName { get; set; }
    public List<string> DeveloperNames { get; set; } = new();
    public decimal PriceAmount { get; set; }
}