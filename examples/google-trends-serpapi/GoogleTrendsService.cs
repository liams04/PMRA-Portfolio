// Purpose: Queries SerpApi's Google Trends endpoint to fetch live search volume indices and derive YoY search interest change.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PMRA.Portfolio.Examples.GoogleTrends;

/// <summary>
/// Configuration options for the SerpAPI Google Trends client.
/// </summary>
public class GoogleTrendsOptions
{
    /// <summary>
    /// API secret key for SerpAPI. Injected securely via User Secrets or environment variables.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL endpoint for SerpApi search requests.
    /// </summary>
    public string BaseUrl { get; set; } = "https://serpapi.com/search.json";
}

/// <summary>
/// Production implementation of <see cref="IGoogleTrendsService"/> backed by SerpApi Google Trends.
/// Executes two targeted HTTP requests per report:
/// <list type="number">
///   <item>A 2-year regional continuous window: splits at the midpoint to derive current 1-year volume and YoY momentum on an identical baseline.</item>
///   <item>A 1-year worldwide window: captures international demand index.</item>
/// </list>
/// </summary>
public class GoogleTrendsService : IGoogleTrendsService
{
    private readonly HttpClient _http;
    private readonly GoogleTrendsOptions _opts;
    private readonly ILogger<GoogleTrendsService> _log;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GoogleTrendsService(
        HttpClient http,
        IOptions<GoogleTrendsOptions> opts,
        ILogger<GoogleTrendsService> log)
    {
        _http = http;
        _opts = opts.Value;
        _log  = log;
    }

    /// <inheritdoc />
    public async Task<GoogleTrendsResult> QueryAsync(string query, string geo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_opts.ApiKey))
        {
            throw new InvalidOperationException(
                "SerpApi API key is not configured. Please set 'GoogleTrends:ApiKey' via dotnet user-secrets or environment variable.");
        }

        // --- Call 1: Regional 2-Year Continuous Window ---
        // Google Trends requires "yyyy-MM-dd yyyy-MM-dd" date strings for custom multi-year windows.
        var startDate = DateTime.UtcNow.AddYears(-2).ToString("yyyy-MM-dd");
        var endDate   = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var twoYearDate = $"{startDate} {endDate}";

        var regionalTimeline = await FetchTimelineAsync(query, geo, twoYearDate, ct);

        // --- Call 2: Worldwide 1-Year Baseline Window ---
        var worldwideTimeline = await FetchTimelineAsync(query, string.Empty, "today 12-m", ct);
        var worldwideAvg = worldwideTimeline.Count > 0
            ? Math.Round((decimal)worldwideTimeline.Average(), 1)
            : 0m;

        // --- Mathematical Derivation: Split the 2-Year Timeline at Midpoint ---
        // By fetching a single 2-year timeline, Google Trends normalizes all 104 data points 
        // to the same peak index (0–100), ensuring YoY calculations are mathematically sound.
        var (currentAvg, priorAvg) = SplitTimeline(regionalTimeline);

        decimal? yoy = (priorAvg > 0)
            ? Math.Round((currentAvg - priorAvg) / priorAvg * 100m, 1)
            : null;

        return new GoogleTrendsResult
        {
            CurrentIndex   = currentAvg,
            PriorYearIndex = priorAvg,
            YoYChangePct   = yoy,
            WorldwideIndex = worldwideAvg,
            Geo            = geo,
            Query          = query
        };
    }

    // -------------------------------------------------------------------------
    // Private Helpers & Transformation Logic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Fetches the raw weekly timeline index points for a given query, geo, and date window.
    /// </summary>
    private async Task<List<int>> FetchTimelineAsync(string query, string geo, string date, CancellationToken ct)
    {
        var url = BuildUrl(query, geo, date);
        _log.LogInformation("Querying SerpApi Trends for '{Query}' (Geo: '{Geo}', Window: '{Date}')", query, geo, date);

        using var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<SerpApiResponse>(body, _json)
                   ?? throw new InvalidOperationException("SerpApi returned a null or empty response body.");

        var points = root.InterestOverTime?.TimelineData ?? [];
        return points
            .Select(p => p.Values?.FirstOrDefault()?.ExtractedValue ?? 0)
            .ToList();
    }

    /// <summary>
    /// Splits a list of ~104 weekly index points at the midpoint.
    /// SerpApi returns timeline data in chronological order (oldest first).
    /// Points 0 to mid = Prior Year (52 weeks ago to 104 weeks ago).
    /// Points mid to end = Current Year (most recent 52 weeks).
    /// </summary>
    private static (decimal current, decimal prior) SplitTimeline(List<int> points)
    {
        if (points.Count == 0)
            return (0m, 0m);

        var mid         = points.Count / 2;
        var priorHalf   = points.Take(mid).ToList();
        var currentHalf = points.Skip(mid).ToList();

        var priorAvg   = priorHalf.Count   > 0 ? (decimal)priorHalf.Average()   : 0m;
        var currentAvg = currentHalf.Count > 0 ? (decimal)currentHalf.Average() : 0m;

        return (Math.Round(currentAvg, 1), Math.Round(priorAvg, 1));
    }

    /// <summary>
    /// Constructs the query URI with properly encoded query parameters.
    /// </summary>
    private string BuildUrl(string query, string geo, string date)
    {
        var builder = new UriBuilder(_opts.BaseUrl);
        var qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
        
        qs["engine"]  = "google_trends";
        qs["q"]       = query;
        if (!string.IsNullOrWhiteSpace(geo))
        {
            qs["geo"] = geo;
        }
        qs["date"]    = date;
        qs["api_key"] = _opts.ApiKey;

        builder.Query = qs.ToString();
        return builder.ToString();
    }

    // -------------------------------------------------------------------------
    // Internal SerpApi JSON Deserialization DTOs
    // -------------------------------------------------------------------------

    private sealed class SerpApiResponse
    {
        [JsonPropertyName("interest_over_time")]
        public SerpApiInterestOverTime? InterestOverTime { get; set; }
    }

    private sealed class SerpApiInterestOverTime
    {
        [JsonPropertyName("timeline_data")]
        public List<SerpApiTimelineEntry>? TimelineData { get; set; }

        [JsonPropertyName("averages")]
        public List<SerpApiAverage>? Averages { get; set; }
    }

    private sealed class SerpApiTimelineEntry
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("values")]
        public List<SerpApiValue>? Values { get; set; }
    }

    private sealed class SerpApiValue
    {
        [JsonPropertyName("query")]
        public string? Query { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("extracted_value")]
        public int ExtractedValue { get; set; }
    }

    private sealed class SerpApiAverage
    {
        [JsonPropertyName("query")]
        public string? Query { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}
