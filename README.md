# TINO
I designed and built TINO while working at Marelli Kechnec Slovakia s.r.o. This is a Windows Forms app for interactive data analysis and shop-floor reference management, backed by SQL Server and DevExpress grids for fast browsing, filtering, and editing of production/reference tables.

## Features
- Multi-table browsing with saved grid layouts
- Add / delete rows with lightweight input validation
- Context-aware editors (combo boxes, popup selectors)
- One-click export to Excel (.xlsx)
- Responsive UI with progress cursors and status labels

## Modules (very short)
- **BMO (BMOForm)** – Main UI: active table management, fetch/refresh, layout save/restore, add/delete, export.
- **Helpers.HelperClass** – DB wrappers: `callQuery`, `callCmd` for SQL Server access.
- **AddNewRowForm** – Dialog for inserting records; returns column values as a dictionary.
- **OtherTableSelector** – Table switcher (e.g., *EReferences*, *CompPrice*, *EStation*).
- **AOI analytics models** – Types and helpers (`AOIQty`, `GroupedData`, `AllAOIInfoPlacement*`) to group AOI data and compute counts/PPM.

## Tech Stack
C# · .NET (WinForms) · DevExpress · SQL Server
