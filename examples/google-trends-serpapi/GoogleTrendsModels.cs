// Purpose: Data transfer models for Google Trends consumer search metrics and request parameters.

namespace PMRA.Portfolio.Examples.GoogleTrends;

/// <summary>
/// Encapsulates search interest index metrics and YoY growth figures derived from Google Trends.
/// </summary>
public class GoogleTrendsResult
{
    /// <summary>
    /// Average relative interest index (0–100) over the most recent 52 weeks in the requested geo.
    /// Displayed as "Google Search Interest" on the report.
    /// </summary>
    public decimal CurrentIndex { get; set; }

    /// <summary>
    /// Average relative interest index (0–100) over weeks 53–104 of the 2-year pull (prior year).
    /// Used to compute <see cref="YoYChangePct"/>.
    /// </summary>
    public decimal PriorYearIndex { get; set; }

    /// <summary>
    /// Year-over-year percentage change in relative interest:
    /// ((CurrentIndex - PriorYearIndex) / PriorYearIndex) * 100.
    /// Null if prior year index is zero (prevents divide-by-zero).
    /// </summary>
    public decimal? YoYChangePct { get; set; }

    /// <summary>
    /// Average relative interest index (0–100) worldwide over the past 12 months.
    /// Displayed as "International Interest" on the report.
    /// </summary>
    public decimal WorldwideIndex { get; set; }

    /// <summary>
    /// Geographic ISO sub-region code (e.g., "US-MN" or "US").
    /// </summary>
    public string Geo { get; set; } = string.Empty;

    /// <summary>
    /// Search term submitted to Google Trends.
    /// </summary>
    public string Query { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for querying Google Trends search volume.
/// </summary>
public class TrendsQueryRequest
{
    /// <summary>
    /// Query keyword string.
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Geographic ISO code (defaults to "US").
    /// </summary>
    public string Geo { get; set; } = "US";
}
