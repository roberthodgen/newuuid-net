# newuuid.net

Instant UUID v4 generation for developers.

`newuuid.net` gives you a fresh random UUID v4 as quickly as possible: copy one from the browser, or fetch one directly from your terminal with `curl`. It is built for one-off IDs, scripts, CLIs, CI jobs, and developer tooling that just need a valid UUID without signup, API keys, JSON parsing, or extra routes.

## Quick Use

```bash
curl https://newuuid.net
```

Example response:

```text
2f5f0c6d-8e77-4d7e-9d61-6f9e08f1f8c2
```

## Why Use It?

- Fresh UUID v4 on every request.
- Plain text response for terminal and script usage.
- Browser page with a copy button for quick manual use.
- No account, API key, tracking parameter, or request body required.
- No JSON to parse when all you need is the ID.

## Browser And curl Behavior

`GET /` checks the request `Accept` header so the same URL works well for humans and command-line tools.

- Requests that explicitly accept `text/html` or `application/xhtml+xml` receive the browser page.
- Other requests, including curl's default `Accept: */*`, receive `text/plain`.
- Plain text responses contain only the UUID and a trailing newline.
- UUID responses send no-cache headers so every request can return a fresh value.

## Local Development

```bash
dotnet run
```

Then test plain text behavior:

```bash
curl http://localhost:5001
```

Test browser-style HTML behavior:

```bash
curl -H "Accept: text/html" http://localhost:5001
```

## Build

```bash
make build
```

This publishes the app into `build/publish` and creates `build/newuuid-net.zip`.

Clean generated build output:

```bash
make clean
```

## Deploy To AWS Lambda

Install the Lambda tooling if needed:

```bash
dotnet tool install -g Amazon.Lambda.Tools
```

Deploy the serverless stack:

```bash
AWS_PROFILE=personal-prod make deploy
```

## Makefile Variables

- `PROJECT`: project file to publish, default `src/NewUuidNet/NewUuidNet.csproj`.
- `PROJECT_LOCATION`: Lambda project directory, default `src/NewUuidNet`.
- `CONFIGURATION`: .NET build configuration, default `Release`.
- `FRAMEWORK`: .NET target framework, default `net10.0`.
- `BUILD_DIR`: local artifact directory, default `build`.
- `PUBLISH_DIR`: local publish directory, default `$(BUILD_DIR)/publish`.
- `PACKAGE_FILE`: local zip artifact, default `$(BUILD_DIR)/newuuid-net.zip`.
- `AWS_PROFILE`: AWS profile for deployment, default `default`.
- `AWS_REGION`: AWS region, default `us-east-1`.
- `STACK_NAME`: CloudFormation stack name, default `newuuid-net`.
