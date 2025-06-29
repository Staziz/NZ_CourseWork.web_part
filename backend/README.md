# Waste Sorting System Backend (.NET 8)

This is a .NET 8 Web API backend for the "Sort Waste using Simulated Hyperspectral Vision" system. It provides:
- JWT authentication (admin, service_personnel roles)
- User management (admin only)
- Waste item processing (image upload, forwards to Python API)
- Statistics (accuracy, volume by category, daily/weekly/monthly summaries)
- Contamination alerts (list, assign, close)
- Manual classification of unclassified waste
- EF Core with SQLite and mock/demo data

## Getting Started

1. **Install .NET 8 SDK**
2. **Restore dependencies:**
   ```
dotnet restore
   ```
3. **Run the backend:**
   ```
dotnet run --project backend
   ```

## Project Structure
- `Controllers/` - API endpoints
- `Models/` - Data models
- `Data/` - EF Core context and seed
- `Services/` - Business logic
- `.github/copilot-instructions.md` - Copilot custom instructions

## Notes
- CORS is enabled for the React frontend.
- The backend communicates with the Python API for image classification.
- All endpoints are RESTful and secured by role-based authorization.

---
For more details, see the code and comments in each file.
