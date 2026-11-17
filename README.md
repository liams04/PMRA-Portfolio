# Program Market Research Application (PMRA) — Project Overview

> **Portfolio Note**: This repository is a sanitized overview of the **Program Market Research Application (PMRA)**, a web application my team and I built for **Minnesota State University, Mankato**. Because the primary codebase belongs to the university, proprietary business logic, internal connection strings, and full source trees are omitted here. Instead, this repo covers the application's architecture, data workflow, and a few representative code samples I wrote.

---

## What is PMRA?

When universities consider launching a new academic degree or reviewing an existing one, academic deans and program directors need to know:
* Is there genuine student demand?
* How saturated is the market with competing schools?
* What are the employment projections and realistic wages for graduates?
* Which award level makes sense (certificate, bachelor's, master's)?

Historically, this meant paying for expensive consulting platforms (like Gray DI) or having faculty spend weeks manually digging through federal spreadsheets. 

**PMRA** is an internal web tool that automates this entire research process. A user selects an academic program (by CIP code), a geographic market (e.g., Minnesota, Upper Midwest, or National), and an award level. The system pulls from preprocessed federal education/labor data plus live Google Trends search data to calculate a comprehensive, structured market report in seconds.

Each metric receives a percentile score against peer programs, and every single number on the screen can be clicked to view its exact formula, source dataset, and date in a side drawer (we called this our **Data Transparency** feature).

---

## Tech Stack

* **Frontend**: Blazor (Interactive Server / WebAssembly), C#, HTML5, CSS
* **Backend**: ASP.NET Core Web API (.NET 8), C#
* **ORM & Database**: Entity Framework Core, PostgreSQL (using 3 isolated schemas: `raw`, `clean`, `reporting`)
* **Data Processing**: Python, Pandas (CLI scripts for semi-annual government data updates)
* **External APIs**: SerpAPI (Google Trends), CDC/NCHS health data
* **DevOps**: GitHub Actions (CI build/test validation), Git feature branching

---

## My Role: Team Lead

I served as the **Team Lead** for our 4-person development team. 

### What I Owned & Delivered:
1. **Requirements & Client Communication**:
   * Worked directly with our client (Dr. Jennifer Veltsos) and university subject matter experts to understand their existing manual workflows.
   * Wrote the **Requirements Analysis Document (RAD)**, turning broad administrative requests into specific formulas, schema definitions, and UI requirements.
   * Kept the scope grounded—for example, advising against adding speculative "AI narrative summaries" in favor of deterministic, verifiable percentile calculations that university leadership could trust.

2. **Team Coordination & Development Workflow**:
   * We used an **Agentic Development** approach—integrating AI coding assistants into our day-to-day workflow while maintaining standard engineering hygiene: feature branching, code reviews, clean interfaces, and GitHub Actions CI checks to catch regressions.
   * Coordinated interface contracts between the Blazor frontend, the .NET Web API, and the Python data pipeline so team members could work in parallel without blocking one another.

3. **Key Technical Contributions**:
   * **Google Trends / SerpAPI Integration**: Wrote the backend service that queries Google Trends via SerpAPI, handling multi-year windows and calculating YoY search momentum.
   * **Interactive Blazor Components**: Built the `GoogleTrendsModal` dialog, query suggestion logic, and report filter interactions.
   * **Data Pipeline & Crosswalks**: Worked on the logic that bridges academic degree classifications (CIP codes) to Bureau of Labor Statistics career codes (SOC codes).

---

## Repository Contents

* **[Architecture Overview](docs/architecture.md)**: High-level diagram showing how Blazor, ASP.NET Core, PostgreSQL, and external services fit together.
* **[Data Pipeline & Collection Flow](docs/data-pipeline.md)**: How federal data (IPEDS, BLS, NSC) is downloaded, cleaned, crosswalked, and cached.
* **[Screenshots & UI Walkthrough](docs/screenshots/README.md)**: Visual tour of the main dashboard, report sections, and data transparency inspector.
* **Code Examples**:
  * **[Google Trends / SerpAPI Service](examples/google-trends-serpapi/)**: C# backend service handling HTTP requests, 2-year timeline splitting, and YoY change calculations.
  * **[Blazor Query Modal Component](examples/frontend-blazor/)**: Razor component handling user query customization, suggestions, and keyboard shortcuts.
  * **[Other Implementation Summaries](examples/other-implementations.md)**: Brief overviews of the NCHS public health data integration and the Python ETL pipeline.
