# Application Screenshots & UI Showcase Guide

This guide outlines recommended screenshots to capture from the **PMRA** application to visually demonstrate key features, UI architecture, and analytical capabilities in this portfolio.

Place captured image files into this directory (`docs/screenshots/`) using the suggested filenames below.

---

## Recommended Screenshots

### 1. Main Dashboard & Report Header
* **Filename**: `01_dashboard_header.png`
* **What to capture**:
  * Top navigation bar, institution branding, and report parameter selector.
  * Inputs: Program selection (CIP Combobox), Market selection (e.g., Regional vs. National), Award Level dropdown, and Rubric toggle.
  * Executive Summary banner showing the overall composite score, program status badge, and generation timestamp.
* **Why it matters**: Demonstrates clean layout, intuitive parameter configuration, and executive presentation of complex academic data.

---

### 2. Google Trends Search Term Modal
* **Filename**: `02_google_trends_modal.png`
* **What to capture**:
  * The dialog pop-up (`GoogleTrendsModal.razor`) triggered during report creation.
  * Context chips displaying selected Program, Market, and Geo Code.
  * The search term input box with suggestion pills (e.g., `"computer science degree"`, `"software engineering program"`).
  * Informational guidance explaining why real-world search terms replace formal CIP codes.
* **Why it matters**: Demonstrates problem-solving in UX—bridging academic taxonomy with consumer search behavior and giving the user control over external API query parameters.

---

### 3. Student Demand & Google Trends Trends Panel
* **Filename**: `03_student_demand_panel.png`
* **What to capture**:
  * The Student Demand report section showing degree conferral trends, regional enrollment graphs, and Google search interest indices.
  * Metrics: Current 1-year search index, Prior Year index, Year-over-Year (YoY) Change %, and Worldwide interest score.
  * Normalized percentile score badges.
* **Why it matters**: Visually validates the SerpAPI / Google Trends integration and multi-window analytical calculations in action.

---

### 4. Competitive Intensity & Market Density
* **Filename**: `04_competitive_intensity.png`
* **What to capture**:
  * The Competitive Intensity panel displaying peer institution counts within the chosen market.
  * Market share breakdown, program completions distribution, and institution density metrics.
  * Percentile scoring reflecting competitive saturation.
* **Why it matters**: Highlights comparative market analysis algorithms and data visualization.

---

### 5. Employment & Wage Projections Panel
* **Filename**: `05_employment_wages.png`
* **What to capture**:
  * The Employment section showing BLS occupational wage tiers (10th percentile, 25th, Median, 75th, 90th percentile).
  * SOC career crosswalk table linking the academic degree to real-world employment opportunities and projected 10-year job openings.
* **Why it matters**: Showcases multi-dataset crosswalking (CIP to SOC) and complex labor market calculations.

---

### 6. Interactive Data Transparency Drawer
* **Filename**: `06_data_transparency_drawer.png`
* **What to capture**:
  * The dynamic inspector panel in the bottom right of the UI when a user clicks on any metric card.
  * Detailed metadata: Source name (e.g., *NCES IPEDS 2023-24* or *BLS OEWS May 2024*), collection date, exact formula used, percentile benchmark population, and known limitations.
* **Why it matters**: Demonstrates engineering rigor and the "Data Transparency" core requirement—eliminating black-box ambiguity for university stakeholders.

---

## Screenshot Optimization Tips
* **Resolution**: Capture at standard 1080p (1920x1080) or 1440p for crisp readability on high-DPI displays.
* **Sanitization**: Ensure no confidential internal student data, private API keys, or internal development URLs (e.g. `localhost:5001`) are exposed in address bars or inspector tools. Use clean window captures.
* **Format**: PNG format is recommended for sharp text and UI boundaries.
