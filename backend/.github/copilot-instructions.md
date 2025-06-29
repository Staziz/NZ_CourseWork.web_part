<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

This is a .NET 8 Web API backend for a waste sorting system. Please follow best practices for:
- JWT authentication and role-based authorization (admin, service_personnel)
- Endpoints for user management, waste item processing (image upload, forwarding to Python API), statistics, contamination alerts, and manual classification
- Use EF Core with SQLite and seed with mock/demo data
- CORS enabled for React frontend
- All endpoints should be RESTful and return appropriate status codes and error messages
