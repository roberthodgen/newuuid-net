# newuuid.net

A small ASP.NET Core Razor Pages application for returning fresh UUIDs.

## Usage

Plain text UUID:

```bash
curl https://newuuid.net
```

Explicit UUID endpoint:

```bash
curl https://newuuid.net/uuid
```

Browser requests to `/` render an HTML page with curl instructions and a copy button.

## Local Development

```bash
dotnet run --project src/NewUuidNet
```

Then test plain text behavior:

```bash
curl http://localhost:5000
curl http://localhost:5000/uuid
```

Test browser-style HTML behavior:

```bash
curl -H "Accept: text/html" http://localhost:5000
```

## Behavior

`GET /` checks the request `Accept` header.

Requests that explicitly accept `text/html` or `application/xhtml+xml` are served by Razor Pages. Other requests, including curl's default `Accept: */*`, receive `text/plain`.

`GET /uuid` always returns `text/plain`.

UUID responses send no-cache headers.

## Deploy To AWS Lambda

Install the Lambda tooling if needed:

```bash
dotnet tool install -g Amazon.Lambda.Tools
```

Deploy the serverless stack:

```bash
dotnet lambda deploy-serverless --project-location src/NewUuidNet
```

The deployment uses `serverless.template` and creates a Lambda function behind an API Gateway HTTP API.
