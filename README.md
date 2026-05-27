# Event & Venue Management System — README

## Overview
A Windows Forms desktop application built with C# and SQL Server Express that manages events, venues, patrons, staff, and tickets. It provides a full GUI for inserting, updating, deleting, and querying data across all tables.

---

## Requirements
- Windows OS
- Visual Studio 2019 or later
- SQL Server Express (instance name: `.\SQLEXPRESS`)
- .NET Framework 4.7.2 or later

---

## Database Setup
1. Open **SQL Server Management Studio (SSMS)**
2. Connect to `.\SQLEXPRESS`
3. Create the database and tables by running your DDL script
4. Make sure the database is named exactly: `EventVenueManagment` (note the original spelling)

---

## Connection String
Located in `DBHelper.cs`:
```
Data Source=.\SQLEXPRESS;Initial Catalog=EventVenueManagment;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;
```
If your SQL Server instance has a different name, update `Data Source=` accordingly.

---

## How to Run
1. Open `EventVeneuApp.sln` in Visual Studio
2. Build the solution — **Ctrl + Shift + B**
3. Run with **F5**
4. Make sure no previous instance of the app is running before rebuilding

---

## Features

**Events tab** — Insert, update name, delete, and view all events with date and venue.

**Patrons tab** — Insert, update email, delete, and view all patrons.

**Venues tab** — Insert, update capacity, delete, and view all venues.

**Staff tab** — Insert, update role, delete, and view all staff members.

**Tickets tab** — Issue and delete tickets linked to patrons and events.

**Ticket Types tab** — Manage ticket categories with price and seat count per event.

**Reports tab** — 5 pre-built JOIN queries:
- Tickets with patron name, event name, and ticket type
- Events with venue location and capacity
- Staff assigned to each event
- Total tickets purchased per patron
- All ticket types and prices per event

---

## Known Issues
- The `VENUE` table has a typo in the column name: `Loacation` instead of `Location` — this is intentional to match the original DDL and is handled consistently throughout the code
- Input panels use fixed positioning, so on very small screens some buttons may be clipped — use the docked layout fix if this occurs

---

## Project Structure
```
EventVeneuApp/
├── MainForm.cs       — All UI tabs and event handlers
├── DBHelper.cs       — Database connection and query helpers
├── App.config        — Application configuration
└── Program.cs        — Entry point
```

---

## Authors
Built as a database systems course project covering INSERT, UPDATE, DELETE, single-table SELECT, and multi-table JOIN queries with a full Windows Forms GUI.
