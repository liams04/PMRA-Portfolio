# Application Architecture

## System Overview

The **Program Market Research Application (PMRA)** is engineered as a decoupled, multi-tier system. The architecture separates presentation, API orchestration, analytical calculation, data persistence, and external data ingestion to maximize modularity and maintainability.

```mermaid
flowchart TD
    subgraph Client["Presentation Layer (PMRA.Web)"]
        UI["Blazor Interactive UI<br/>(Pages & Components)"]
        State["Client State & ViewModels"]
        HttpClients["Typed API Clients<br/>(ReportsApiClient, EmploymentApiClient)"]
        UI --> State
        State --> HttpClients
    end

    subgraph Backend["API & Application Layer (PMRA.Api)"]
        API["REST Controllers<br/>(/api/reports, /api/employment)"]
        Validation["Input & CIP Validation"]
        RepGen["Report Generation Coordinator<br/>(ReportGenerationService)"]
        
        subgraph CalculationEngines["Analytical Engines"]
            DemandEngine["Student Demand Calculator"]
            CompEngine["Competitive Intensity Calculator"]
            EmpEngine["Employment & Wage Calculator"]
            ScoringEngine["Percentile Scoring & Normalization Engine"]
        end

        API --> Validation
        Validation --> RepGen
        RepGen --> DemandEngine
        RepGen --> CompEngine
        RepGen --> EmpEngine
        RepGen --> ScoringEngine
    end

    subgraph External["External Integrations"]
        SerpApi["SerpAPI<br/>(Google Trends Engine)"]
        NCHS["NCHS Public Health API<br/>(Health Data Services)"]
    end

    subgraph Database["Persistence Layer (PostgreSQL)"]
        subgraph Schemas["Three-Schema Isolation"]
            RawSchema[("raw<br/>(Unmodified Source Data)")]
            CleanSchema[("clean<br/>(Standardized, CIP/SOC Mapped)")]
            ReportSchema[("reporting<br/>(Materialized Reports & Scores)")]
        end
    end

    subgraph Pipeline["Data Pipeline (Python ETL)"]
        Extract["Source Discoverers & Downloaders"]
        Transform["Standardizer & CIP-SOC Crosswalk"]
        Validate["Pipeline Validator & Run Logger"]
        Extract --> Transform --> Validate
    end

    %% Communications
    HttpClients -->|HTTPS / JSON| API
    DemandEngine -->|HTTP Query| SerpApi
    DemandEngine -->|HTTP Query| NCHS
    
    RepGen -->|EF Core Queries| CleanSchema
    RepGen -->|Cache Lookup / Save| ReportSchema
    
    Validate -->|Bulk Load| RawSchema
    Transform -->|Standardize| CleanSchema
```

---

## Architectural Principles & Constraints

### 1. Hard Separation of Concerns
* **No Direct DB Access from UI**: The Blazor frontend operates strictly over HTTP calling ASP.NET Core endpoints. It has zero knowledge of database connections or underlying database schemas.
* **Service Independence**: Calculation services (e.g., `EmploymentCalculator`, `ReportScoringCalculator`) operate against standardized DTOs and interfaces, allowing scoring logic to be unit tested without live database or network dependencies.

### 2. Multi-Schema Database Isolation
The database is deliberately structured into three distinct schemas:
1. `raw`: Direct mirror of external downloads (IPEDS completions, BLS wage tables). Preserves original source integrity.
2. `clean`: Normalized and standardized data. Maps heterogeneous source IDs to unified CIP (Classification of Instructional Programs) and SOC (Standard Occupational Classification) codes.
3. `reporting`: Materialized reports and calculated percentile scores. Supports instant cache retrieval and historical report versioning.

### 3. Report Caching & Staleness Invalidation
To balance responsive performance with fresh data:
* When a report is requested for a specific `(CIP Code, Market, Award Level, Rubric)` tuple, the API first checks `reporting.Reports`.
* If a stored report exists and was generated after the latest data refresh (tracked by the pipeline run log), the cached report is returned immediately.
* If the underlying dataset has been refreshed or no report exists, the calculation engines execute, persist the result, and return the newly materialized report.

### 4. Resilient External Service Design
* Calls to external services like SerpAPI (Google Trends) are isolated behind strict interface contracts (`IGoogleTrendsService`).
* Configurable timeouts, error handling, and parameter validation prevent external third-party outages from crashing core report generation.

---

## Request & Report Generation Lifecycle

```mermaid
sequenceDiagram
    autonumber
    actor User as Academic User
    participant Blazor as Blazor Frontend
    participant Modal as Google Trends Modal
    participant API as ASP.NET Core API
    participant Cache as Report Cache / DB
    participant Serp as SerpAPI (Google Trends)
    participant Calc as Scoring Engine

    User->>Blazor: Selects CIP, Market, Award Level
    User->>Blazor: Clicks "Generate Report"
    Blazor->>Modal: Opens Search Term Dialog
    User->>Modal: Enters / Confirms search query
    Modal-->>Blazor: Confirmed Query & Geo Scope
    
    Blazor->>API: POST /api/reports/generate (Request DTO)
    API->>Cache: Check for existing unexpired report
    alt Valid Cache Found
        Cache-->>API: Return materialized report
    else Cache Miss or Data Updated
        API->>Serp: Query 2-yr regional + 1-yr worldwide timeline
        Serp-->>API: Timeline index values
        API->>Calc: Compute raw metrics & percentile distributions
        Calc-->>API: Calculated sections & transparency metadata
        API->>Cache: Persist new report to reporting schema
    end
    API-->>Blazor: Return Complete Report DTO
    Blazor->>User: Render interactive report panels & transparency drawer
```
