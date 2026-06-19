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
- AWS credentials configured via `AWS_PROFILE`

## Architecture

- `GET /` has content negotiation: plain text UUID for `curl`/scripts, HTML page for browsers
- Middleware in `Program.cs` short-circuits non-HTML requests — does NOT go through Razor Pages
- HTML page (`Pages/Index.cshtml`) generates UUID in code-behind (`Index.cshtml.cs`)
- Both responses send no-cache headers
- Deployed as AWS Lambda via SAM (`serverless.template`), HttpApi events for `/` and `/{proxy+}`

## Gotchas

- No tests, no CI, no lint/typecheck — nothing to verify changes against
- `build/` is gitignored; the committed `build/newuuid-net.zip` is stale — always run `make build`
- Dev port is `5001` (not the default `5000`)
