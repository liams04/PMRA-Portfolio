# Additional Implementation Summaries

This document provides high-level architectural overviews of two additional core subsystems developed for the **Program Market Research Application (PMRA)**. To respect organizational data agreements, proprietary code for these implementations is not published in this public repository.

---

## 1. NCHS Public Health Data Integration

### Problem Statement & Domain Relevance
When evaluating healthcare-oriented academic programs (such as *Nursing*, *Healthcare Informatics*, and *Public Health Services*), regional economic data alone does not capture the public health imperatives that drive healthcare workforce hiring. To enrich program viability analysis, PMRA integrates health metrics published by the **National Center for Health Statistics (NCHS)** and the **Centers for Disease Control and Prevention (CDC)**.

### Architectural Solution
* **RESTful External Retrieval**: Built a resilient C# service communicating with federal open data APIs (data.cdc.gov / Socrata Open Data API) using strongly typed HTTP client factories.
* **Geographic Alignment**: Transformed county-level and state-level public health datasets (e.g., chronic disease burdens, provider-to-population ratios, rural health shortage designations) to match university market boundaries (Minnesota, Upper Midwest, and National).
* **Caching & Fallback Strategy**: Given that federal health statistical datasets update on irregular annual or multi-year cycles, implemented an asynchronous caching layer (`IMemoryCache` backed by database staging) to ensure instant report rendering without blocking on federal API uptime.

### Technical Takeaway
Demonstrates experience integrating non-commercial government REST APIs, dealing with variable health record schemas, and synthesizing disparate health statistics into meaningful academic program context.

---

## 2. Automated Multi-Source ETL Pipeline (IPEDS / BLS / NSC)

### Problem Statement
A comprehensive market research report requires aggregating data across multiple massive government datasets:
* **IPEDS (Integrated Postsecondary Education Data System)**: Over 6,000 postsecondary institutions reporting annual completions, degrees conferred, and enrollment trends.
* **BLS OEWS (Occupational Employment and Wage Statistics)**: Hundreds of thousands of metropolitan and state wage distributions across thousands of Standard Occupational Classification (SOC) occupations.
* **National Student Clearinghouse (NSC)**: Aggregate enrollment momentum benchmarks.

Manually downloading, parsing, and joining these gigabyte-scale datasets is slow, error-prone, and unsustainable for continuous academic decision-making.

### Architectural Solution
* **Modular Python Ingest Engine**: Created an automated ingestion suite using Python and Pandas. Each data source implements a unified ingestion interface with custom parsers for differing CSV schemas, fixed-width text files, and Excel releases.
* **Semantic CIP-to-SOC Crosswalk Engine**: Engineered the translation layer bridging academic Classification of Instructional Programs (CIP) with federal Standard Occupational Classification (SOC) codes. Handled one-to-many and many-to-many crosswalk relationships using weighted distribution models to accurately project career outcomes.
* **Automated Run-Logging & Integrity Verification**: Every pipeline execution writes an immutable run record detailing download checksums, parsed row counts, and schema validation checks. This run log automatically drives the web application's cache invalidation engine.

### Technical Takeaway
Demonstrates expertise in large-scale data modeling, data pipeline orchestration, schema migration, and cross-domain data standardizations.
