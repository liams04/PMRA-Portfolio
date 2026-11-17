// Purpose: Contract for querying Google Trends search interest and YoY metrics by keyword and location.

using System.Threading;
using System.Threading.Tasks;

namespace PMRA.Portfolio.Examples.GoogleTrends;

/// <summary>
/// Service contract for retrieving and calculating Google Trends consumer demand metrics.
/// </summary>
public interface IGoogleTrendsService
{
    /// <summary>
    /// Fetches Google Trends interest data for the given <paramref name="query"/> and
    /// <paramref name="geo"/>, returning a <see cref="GoogleTrendsResult"/> containing the
    /// current 1-year interest index, the year-over-year change percentage, and the worldwide
    /// interest index.
    /// </summary>
    /// <param name="query">Plain-language search term (e.g., "computer science degree").</param>
    /// <param name="geo">
    /// SerpApi <c>geo</c> code. Use an ISO 3166-1/3166-2 code such as "US-MN" for Minnesota
    /// or "US" for the United States. Pass an empty string for worldwide results.
    /// </param>
    /// <param name="ct">Cancellation token for aborting in-flight HTTP calls.</param>
    /// <returns>A calculated <see cref="GoogleTrendsResult"/> snapshot.</returns>
    Task<GoogleTrendsResult> QueryAsync(string query, string geo, CancellationToken ct = default);
}
