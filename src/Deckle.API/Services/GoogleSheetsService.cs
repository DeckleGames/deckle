using Deckle.API.DTOs;
using Deckle.Domain.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Deckle.API.Services;

public class GoogleSheetsService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public GoogleSheetsService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<SheetsService> GetSheetsServiceForUserAsync(Guid userId)
    {
        var credential = await _dbContext.GoogleCredentials
            .FirstOrDefaultAsync(gc => gc.UserId == userId);

        if (credential == null)
        {
            throw new InvalidOperationException("User has not authorized Google Sheets access");
        }

        // Check if token is expired
        if (credential.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
        {
            await RefreshTokenAsync(credential);
        }

        var googleCredential = GoogleCredential.FromAccessToken(credential.AccessToken);

        var sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = googleCredential,
            ApplicationName = "Deckle"
        });

        return sheetsService;
    }

    private async Task RefreshTokenAsync(Domain.Entities.GoogleCredential credential)
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var clientSecret = _configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Google OAuth credentials not configured");
        }

        var tokenResponse = await new HttpClient().PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["refresh_token"] = credential.RefreshToken,
                ["grant_type"] = "refresh_token"
            })
        );

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to refresh Google access token");
        }

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (tokenData == null)
        {
            throw new InvalidOperationException("Invalid token response");
        }

        credential.AccessToken = tokenData["access_token"].ToString() ?? throw new InvalidOperationException("No access token in response");
        credential.ExpiresAt = DateTime.UtcNow.AddSeconds(int.Parse(tokenData["expires_in"].ToString() ?? "3600"));
        credential.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public static string? ExtractSpreadsheetIdFromUrl(string url)
    {
        // Extract spreadsheet ID from various Google Sheets URL formats
        var patterns = new[]
        {
            @"docs\.google\.com/spreadsheets/d/([a-zA-Z0-9-_]+)",
            @"spreadsheets/d/([a-zA-Z0-9-_]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(url, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        return null;
    }

    public async Task<SpreadsheetMetadata> GetSpreadsheetMetadataAsync(Guid userId, string spreadsheetId)
    {
        var service = await GetSheetsServiceForUserAsync(userId);

        var request = service.Spreadsheets.Get(spreadsheetId);
        request.Fields = "spreadsheetId,properties(title),sheets(properties(sheetId,title,gridProperties))";

        var spreadsheet = await request.ExecuteAsync();

        return new SpreadsheetMetadata
        {
            SpreadsheetId = spreadsheet.SpreadsheetId,
            Title = spreadsheet.Properties.Title,
            Sheets = spreadsheet.Sheets?.Select(s => new SheetMetadata
            {
                SheetId = s.Properties.SheetId ?? 0,
                Title = s.Properties.Title,
                RowCount = s.Properties.GridProperties?.RowCount ?? 0,
                ColumnCount = s.Properties.GridProperties?.ColumnCount ?? 0
            }).ToList() ?? []
        };
    }

    public async Task<List<IList<object>>> GetSheetDataAsync(Guid userId, string spreadsheetId, string range)
    {
        var service = await GetSheetsServiceForUserAsync(userId);

        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();

        return response.Values?.ToList() ?? [];
    }
}
