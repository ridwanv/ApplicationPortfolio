## Project overview / description

An internal web application that catalogs, governs, and operates an organization’s applications and systems across their lifecycle. It centralizes ownership, dependencies, compliance status, and operational health to improve visibility, accountability, and decision-making.

## Target audience

- IT administrators and platform engineers
- Application owners and product teams
- Security, risk, and compliance officers
- Procurement/finance partners and technology leadership

## Primary benefits / features

- Central application and system catalog (ownership, contacts, SLAs, environments)
- Lifecycle management: intake → approval → deployment → change → retirement
- Dependency and integration mapping (systems, services, data flows)
- Compliance and risk tracking (controls, attestations, evidence, audit trail)
- Access control and permissions aligned to roles/teams
- Health, incidents, and change records (or integration with ITSM tools)
- Cost/usage insights and metadata (tags, cost centers, business capability mapping)
- Search, reporting, and dashboards for portfolio, risk, and operational views
- Notifications and workflow automation (reviews, renewals, ownership changes)
- Open API/webhooks for integrations with CI/CD, CMDB, identity, and monitoring

## High-level tech/architecture used

- Clean Architecture layering: Domain, Application (CQRS/MediatR), Infrastructure, UI
- Blazor Server UI with component-based pages per feature
- EF Core for data access to a relational database (SQL Server/PostgreSQL)
- Authentication/authorization via ASP.NET Core Identity and policy-based permissions
- Caching (e.g., FusionCache) and background jobs as needed; container-ready deployment



