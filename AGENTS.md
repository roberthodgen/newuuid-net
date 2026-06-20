# AGENTS.md

## Project

Single-project .NET 10 app (`src/NewUuidNet/`). Serving UUID v4 via ASP.NET Core Razor Pages, hosted on AWS Lambda.

## Commands

- `dotnet run` — local dev server on `localhost:5001` (sets `ASPNETCORE_ENVIRONMENT=Development`)
- `make build` — publishes + packages Lambda zip into `build/newuuid-net.zip`
- `make deploy` — builds and deploys to AWS (requires `AWS_PROFILE` env var, default `personal-prod`)
- `make clean` — removes `build/` and project `obj/`
- Override Makefile vars: `AWS_PROFILE=xxx AWS_REGION=us-west-2 make deploy`

## Deploy Prerequisites

- `dotnet tool install -g Amazon.Lambda.Tools` (one-time)
- Local deploys require AWS credentials configured via `AWS_PROFILE`

## Deployment

- Pushes to `main` deploy automatically via `.github/workflows/deploy.yml`
- GitHub Actions assumes AWS IAM role `newuuid-deploy` via OIDC; no stored AWS keys are used
- CI deploys with `AWS_PROFILE` empty so `dotnet lambda deploy-serverless` uses OIDC-provided environment credentials
- Local fallback deploy: `AWS_PROFILE=personal-prod make deploy`
- Deployment targets AWS region `us-east-1`, CloudFormation stack `newuuid-net`, S3 bucket `newuuid-net`
- Full IAM setup and troubleshooting notes are in `DEPLOY.md`

## Architecture

- `GET /` has content negotiation: plain text UUID for `curl`/scripts, HTML page for browsers
- Middleware in `Program.cs` short-circuits non-HTML requests — does NOT go through Razor Pages
- HTML page (`Pages/Index.cshtml`) generates UUID in code-behind (`Index.cshtml.cs`)
- Both responses send no-cache headers
- Deployed as AWS Lambda via SAM (`serverless.template`), HttpApi events for `/` and `/{proxy+}`

## Gotchas

- No tests or lint/typecheck; the only GitHub workflow is deployment on `main`
- `build/` is gitignored and generated locally/CI; always run `make build` before relying on `build/newuuid-net.zip`
- Dev port is `5001` (not the default `5000`)
