# newuuid.net

A small ASP.NET Core Razor Pages application for returning fresh UUIDs.

## Usage

Plain text UUID:

```bash
curl https://newuuid.net
```

Browser requests to `/` render an HTML page with curl instructions and a copy button.

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

## Behavior

`GET /` checks the request `Accept` header.

- Requests that explicitly accept `text/html` or `application/xhtml+xml` are served by Razor Pages.
- Other requests, including curl's default `Accept: */*`, receive `text/plain`.

UUID responses send no-cache headers.

## Deploy To AWS Lambda

Install the Lambda tooling if needed:

```bash
dotnet tool install -g Amazon.Lambda.Tools
```

Build a local deployment artifact:

```bash
make build
```

This publishes the app into `build/publish` and creates `build/newuuid-net.zip`.

Deploy the serverless stack:

```bash
AWS_PROFILE=personal-prod make deploy
```

Clean generated build output:

```bash
make clean
```

The deployment uses `src/NewUuidNet/aws-lambda-tools-defaults.json` and `serverless.template`. The template creates a Lambda function behind an API Gateway HTTP API. The Makefile calls `dotnet lambda deploy-serverless` with the project location, profile, region, and stack name; the Lambda tooling handles packaging during deployment.

Common environment overrides:

```bash
AWS_PROFILE=my-profile AWS_REGION=us-west-2 make deploy
```

```bash
STACK_NAME=newuuid-net-prod make deploy
```

```bash
CONFIGURATION=Debug BUILD_DIR=.build make build
```

Available Makefile variables:

- `PROJECT`: project file to publish, default `src/NewUuidNet/NewUuidNet.csproj`.
- `PROJECT_LOCATION`: Lambda project directory, default `src/NewUuidNet`.
- `CONFIGURATION`: .NET build configuration, default `Release`.
- `FRAMEWORK`: .NET target framework, default `net8.0`.
- `BUILD_DIR`: local artifact directory, default `build`.
- `PUBLISH_DIR`: local publish directory, default `$(BUILD_DIR)/publish`.
- `PACKAGE_FILE`: local zip artifact, default `$(BUILD_DIR)/newuuid-net.zip`.
- `AWS_PROFILE`: AWS profile for deployment, default `default`.
- `AWS_REGION`: AWS region for deployment, default `us-east-1`.
- `STACK_NAME`: CloudFormation stack name, default `newuuid-net`.
