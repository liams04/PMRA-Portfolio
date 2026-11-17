# Application Architecture

## System Diagram

The application is structured into four main layers: the Blazor frontend, the ASP.NET Core Web API, a multi-schema PostgreSQL database, and a background Python data pipeline.

```mermaid
flowchart TD
    subgraph Frontend["Blazor Web App (PMRA.Web)"]
        UI["Blazor Interactive UI<br/>(Components & Pages)"]
        State["View State & Filter Selection"]
        HttpClients["Typed HTTP Clients<br/>(ReportsApiClient)"]
        UI --> State --> HttpClients
    end

    subgraph Backend["Backend API (PMRA.Api)"]
        API["API Controllers<br/>(/api/reports)"]
        RepGen["Report Generation Coordinator"]
        
        subgraph Services["Calculation Services"]
            Demand["Student Demand Service"]
            Comp["Competition Service"]
            Emp["Employment & Wage Service"]
            Scoring["Percentile Scoring Service"]
        end

        API --> RepGen
        RepGen --> Demand & Comp & Emp & Scoring
    end

    subgraph External["External APIs"]
        SerpApi["SerpAPI<br/>(Google Trends Data)"]
        NCHS["CDC / NCHS API<br/>(Health Data)"]
    end

    subgraph Database["PostgreSQL Database"]
        RawSchema[("raw<br/>Original government downloads")]
        CleanSchema[("clean<br/>Cleaned tables & CIP-SOC mappings")]
        ReportSchema[("reporting<br/>Cached reports & scores")]
    end

    subgraph Pipeline["Data Pipeline (Python)"]
        ETL["Ingest & Cleaning Scripts"]
    end

    %% Connections
    HttpClients -->|HTTPS JSON| API
    Demand -->|HTTP| SerpApi
    Demand -->|HTTP| NCHS
    
    RepGen -->|EF Core Queries| CleanSchema
    RepGen -->|Read / Write Cache| ReportSchema
    
    ETL -->|Bulk Insert| RawSchema
    ETL -->|Clean & Map| CleanSchema
```

---

## How the Pieces Fit Together

### 1. Blazor Frontend (`PMRA.Web`)
* The frontend is built with Blazor and talks to the backend strictly over HTTP via typed API clients.
* We kept all database logic and heavy math out of the client. The frontend's job is purely to collect user filter selections (program, market, award level), show progress spinners while calculations run, render the report sections, and display provenance metadata when a user clicks on a number.

### 2. ASP.NET Core Web API (`PMRA.Api`)
* When a user requests a report, the `ReportGenerationService` acts as a traffic cop.
* It first checks the `reporting` database schema to see if a report with those exact parameters already exists.
* If a cached report is found and the underlying data hasn't been updated since it was built, it returns the report immediately.
* If it's a new query or the dataset was recently updated, the coordinator calls the individual calculation services (`StudentDemand`, `Competition`, `Employment`), queries SerpAPI for live search trends, computes percentile scores, saves the result to the cache, and sends it back.

### 3. Three-Schema Database Design
Rather than dumping everything into one flat schema, we split PostgreSQL into three isolated areas:
1. `raw`: Contains unedited raw data directly as downloaded from federal files (IPEDS, BLS). This makes it easy to audit source data without re-downloading files.
2. `clean`: Tables after our Python pipeline standardizes data types, normalizes institution names, and runs the CIP-to-SOC career crosswalks.
3. `reporting`: Materialized reports and calculated metric scores. This allows instant loading for frequently requested programs.

---

## Report Generation Flow

```mermaid
sequenceDiagram
    autonumber
    actor User as User
    participant UI as Blazor Frontend
    participant Modal as Trends Modal
    participant API as ASP.NET Core API
    participant DB as Database (reporting schema)
    participant Serp as SerpAPI (Google Trends)

    User->>UI: Selects Program, Market, Award Level
    User->>UI: Clicks "Generate Report"
    UI->>Modal: Prompts for search term (defaults to cleaned title)
    User->>Modal: Confirms search query
    Modal-->>UI: Passes query back to page
    
    UI->>API: POST /api/reports/generate
    API->>DB: Check for existing unexpired report
    
    alt Cached Report Found
        DB-->>API: Returns cached report
    else Not Cached / Data Updated
        API->>Serp: Queries 2-year regional + 1-year global search index
        Serp-->>API: Returns weekly timeline points
        API->>DB: Queries clean employment & completion stats
        DB-->>API: Returns cleaned metrics
        API->>API: Calculates percentiles against peer programs
        API->>DB: Saves generated report to reporting schema
    end
    
    API-->>UI: Returns report payload
    UI-->>User: Renders report and updates Data Transparency drawer
```
