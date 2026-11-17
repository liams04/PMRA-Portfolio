# Program Market Research Application (PMRA) — Portfolio Showcase

[![Tech Stack](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20Layered-blue)](#application-architecture)
[![Frontend](https://img.shields.io/badge/Frontend-Blazor-512BD4?logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Backend](https://img.shields.io/badge/Backend-ASP.NET%20Core%20API-512BD4)](https://learn.microsoft.com/aspnet/core)
[![Database](https://img.shields.io/badge/Database-PostgreSQL%20%2F%20EF%20Core-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![External Integration](https://img.shields.io/badge/Integration-SerpAPI%20%2F%20Google%20Trends-4285F4?logo=google&logoColor=white)](https://serpapi.com/)

> **Note**: This repository is a sanitized, public portfolio showcase of the **Program Market Research Application (PMRA)** developed for **Minnesota State University, Mankato**. Proprietary business rules, institutional credentials, and complete production source files have been omitted or redacted. Selected representative architectural documents and code implementations are highlighted to demonstrate software engineering and leadership capabilities.

---

## Executive Summary & Problem Statement

Higher education institutions face increasing pressure to ensure new and existing academic programs align with market demand, workforce needs, and competitive landscapes. Traditionally, universities relied on costly third-party consulting services or labor-intensive manual workflows (such as *PES Markets by Gray DI*) to evaluate degree viability.

The **Program Market Research Application (PMRA)** is an enterprise web platform engineered to automate and democratize this analysis. By synthesizing public government and educational data (IPEDS, BLS, NSC) with real-time consumer search trends, PMRA generates comprehensive, deterministic market research reports across four pillars:

1. **Student Demand**: Degrees conferred, regional enrollment trends, and consumer search interest.
2. **Competitive Intensity**: Degree-granting institution density, market concentration, and program completions.
3. **Employment & Wages**: Bureau of Labor Statistics (BLS) occupational employment projections, annual wage percentiles, and CIP-to-SOC career crosswalks.
4. **Degree Level Alignment**: Award level suitability (Certificates, Associate's, Bachelor's, Master's, Doctoral).

Every metric produces a normalized **Percentile Score** (0–100%) against national or regional peer programs, linked to a real-time **Data Transparency** inspection engine for auditability.

---

## Technical Stack & Architecture

| Layer | Technologies & Frameworks | Key Responsibilities |
| :--- | :--- | :--- |
| **Frontend** | Blazor (Interactive Server / WebAssembly), HTML5, CSS3, JavaScript | Component-driven UI, real-time report filter orchestration, dynamic search term modals, responsive charts, and contextual transparency inspection. |
| **Backend API** | C#, ASP.NET Core Web API (.NET 8) | RESTful API controllers, request validation, business logic coordination, metric calculation engines, and report assembly. |
| **Data Access** | Entity Framework Core (EF Core), LINQ | Repository abstraction, schema separation, efficient relational querying, and projection mapping. |
| **Database** | PostgreSQL / SQL Server | Structured storage divided across layered schemas (`raw`, `clean`, `reporting`) to isolate uncleaned data from pre-calculated report metrics. |
| **Data Pipeline** | Python, Pandas, CLI automation | Scheduled and on-demand ingestion, ETL extraction, data validation, and CIP-SOC crosswalk standardization. |
| **External APIs** | SerpAPI (Google Trends), NCHS, Federal Data Portals | Live multi-window timeline search indexing, geographic demand analysis, and public health data retrieval. |
| **DevOps & QA** | GitHub Actions (CI/CD), xUnit, Git feature branching | Automated builds, unit and integration test suites, pull request validation, and continuous delivery. |

For detailed architectural flowcharts and data ingestion lifecycles, see:
* [Architecture Documentation](docs/architecture.md)
* [Data Collection & Pipeline Workflow](docs/data-pipeline.md)

---

## Team Leadership & Engineering Contributions

As **Team Lead** for a cross-functional team of 4 engineers, I led both the product roadmap and technical architecture from inception through delivery.

### 1. Requirements Engineering & Stakeholder Communication
* Authored the comprehensive **Requirements Analysis Document (RAD)** translating academic administration requirements into deterministic data metrics and technical user stories.
* Acted as the primary technical liaison to university stakeholders, executive leadership (Client: Dr. Jennifer Veltsos), faculty advisors, and subject matter experts (SMEs).
* Eliminated requirement ambiguities upfront—such as clarifying the difference between national and in-market peer institutions, defining fallback behaviors for low-data CIP codes, and replacing black-box AI concepts with transparent, auditable percentile ranking algorithms.

### 2. Team Coordination & Agentic Development Methodology
* Pioneered an **Agentic Development** workflow within the team, combining modern AI developer agent tooling with rigorous engineering practices (branch protections, atomic pull requests, automated test verification, and structured peer reviews).
* Led weekly planning sessions, unblocked teammates on technical dependencies, and defined clean interface contracts between the Blazor frontend, the API calculation services, and the Python data pipeline.

### 3. Core Technical Contributions
* **External API Integration (SerpAPI / Google Trends)**: Designed and implemented the complete Google Trends ingestion workflow. Formulated a 2-year regional timeline split to calculate Year-over-Year (YoY) momentum alongside a 1-year worldwide search volume baseline.
* **Interactive Frontend Components**: Engineered the Blazor `GoogleTrendsModal` enabling academic analysts to bridge the gap between technical CIP classifications and consumer search behavior, providing keyword suggestions and validation.
* **Scoring & Calculation Engine**: Contributed to the report scoring services that calculate percentile distributions and normalize disparate metrics into executive scores.
* **CI/CD & DevOps**: Structured GitHub Actions workflows ensuring continuous compilation, unit test validation, and linting on every feature branch.

---

## Showcase Code Examples

This portfolio showcases selected representative implementations:

| Directory | Topic | Description |
| :--- | :--- | :--- |
| [`examples/google-trends-serpapi/`](examples/google-trends-serpapi/) | **Google Trends / SerpAPI Service** | Production-ready C# service implementing multi-window timeline aggregation, YoY momentum calculation, and resilient HTTP handling. |
| [`examples/frontend-blazor/`](examples/frontend-blazor/) | **Blazor Modal & Query Orchestrator** | Interactive Razor component demonstrating clean state management, user guidance, suggestion chips, and event callback patterns. |
| [`examples/other-implementations.md`](examples/other-implementations.md) | **Other Architectural Highlights** | Technical overviews of the NCHS public health data integration and the automated multi-source ETL pipeline. |

---

## Visual Demonstration & Screenshots

Visual walkthroughs of the application UI, report layout, and data inspection features can be found in the screenshots directory:

👉 **[View Screenshots & UI Tour](docs/screenshots/README.md)**

---

## Key Engineering Takeaways

1. **Bridging Academic Taxonomy and Consumer Intent**: Standard academic CIP codes (e.g., `11.0701 - Computer Science`) do not reflect how prospective students search on Google. Introducing an interactive query customization modal with intelligent fallbacks solved this discrepancy without compromising report generation speed.
2. **Deterministic Transparency over Black-Box Scoring**: Higher education decisions require accountability. By replacing speculative scoring with deterministic percentile models accompanied by clickable provenance metadata, users can verify every data point against primary sources (BLS, IPEDS).
3. **Resilient Third-Party API Architecture**: External APIs can be rate-limited, volatile, or costly. Designing multi-tiered caching strategies and single-call multi-window query derivations minimized overhead while ensuring timely reporting.
