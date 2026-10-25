# ApplicationPortfolio

ApplicationPortfolio is a multi-tenant enterprise application portfolio and estate-management application built with Blazor Server and Clean Architecture. It manages IT systems and organizational systems, capabilities, dependencies, attributes, documents, and system logs. The solution also includes estate documents, assets, beneficiaries, and inheritance calculations.

## Build and run

Install the .NET SDK required by the repository, then build and test with `dotnet build CleanArchitecture.Blazor.slnx` and `dotnet test CleanArchitecture.Blazor.slnx`. Start the UI from `src/Server.UI`.

Database providers and migrators are under `src/Migrators`; configure database and identity settings locally or via environment variables. Do not commit secrets.
