# Other Subsystems (Summaries)

These are brief technical summaries of two other subsystems we built for PMRA. To respect university ownership, full production code for these modules is not published in this showcase repository.

---

## 1. NCHS Public Health Data Integration

### The Problem
When evaluating healthcare programs (like Nursing, Healthcare Informatics, or Public Health), standard economic metrics only tell part of the story. Student and workforce demand in healthcare is often driven by regional public health needs, disease prevalence, and county health worker shortages.

### What We Built
* **REST Ingestion**: Built a C# service that queries public health metrics from the CDC and the National Center for Health Statistics (NCHS) open data APIs (data.cdc.gov).
* **County-to-Market Aggregation**: NCHS data is typically released at county or state levels. We mapped these records into our application's geographic market definitions (e.g., aggregating Minnesota counties to reflect regional healthcare service areas).
* **Caching**: Because government health datasets update infrequently (typically annually or bi-annually), we cached responses locally in PostgreSQL so that report generation remains instantaneous.

---

## 2. Multi-Source ETL Pipeline (IPEDS / BLS / NSC)

### The Problem
Generating a complete market report requires joining data across three massive federal sources:
* **IPEDS**: National Center for Education Statistics data covering degree completions and program conferrals from over 6,000 colleges.
* **BLS OEWS**: Bureau of Labor Statistics data providing occupational employment counts and wage percentiles (10th, 25th, Median, 75th, 90th).
* **National Student Clearinghouse (NSC)**: Benchmark data for national and regional enrollment trends.

Downloading and joining these multi-gigabyte files manually was the bottleneck our client wanted to eliminate.

### What We Built
* **Python Ingestion Scripts**: Built standalone Python modules to download, unpack, and validate each source dataset independently.
* **The CIP-SOC Crosswalk**: Program completions use CIP codes, while wage statistics use SOC codes. We wrote crosswalk mapping scripts that connect each academic degree to its relevant career paths, handling both one-to-many and many-to-many relationships.
* **Pipeline Run Logging**: Every pipeline run writes a record with timestamp, row counts, and status to a `PipelineRunLog` table. This allows the web API to detect when cached reports are out of date and need to be recalculated.
