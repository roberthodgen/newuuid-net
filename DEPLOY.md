# Deployment

This app deploys automatically to AWS Lambda when changes are pushed to the `main` branch.

## How it works

A GitHub Actions workflow (`.github/workflows/deploy.yml`) triggers on every push to `main`. It:

1. Checks out the code
2. Assumes an AWS IAM role via OIDC (no stored credentials)
3. Installs .NET 10 and `Amazon.Lambda.Tools`
4. Builds `src/NewUuidNet/` and packages `build/newuuid-net.zip`
5. Deploys the SAM/CloudFormation stack `newuuid-net` using `serverless.template`

Only the `main` branch can trigger deployment. Feature branches and PRs do not deploy.

## Deployment settings

- AWS account: `048044547730`
- AWS region: `us-east-1`
- IAM role: `newuuid-deploy`
- CloudFormation stack: `newuuid-net`
- Deployment bucket: `newuuid-net`
- Lambda project: `src/NewUuidNet/`
- Package artifact: `build/newuuid-net.zip`
- SAM template: `serverless.template`

## IAM setup (one-time)

The workflow assumes an IAM role via GitHub's OIDC provider.

### Configuring IAM to trust GitHub

To use GitHub's OIDC provider, first set up federation in your AWS account by creating an IAM Identity Provider with these details:

- Provider Type: `OIDC`
- Provider URL: `https://token.actions.githubusercontent.com`
- Audience: `sts.amazonaws.com`

### 1. Create the IAM role

In the AWS Console, go to **IAM > Roles > Create role**:

- **Trusted entity**: Web identity
- **Identity provider**: `token.actions.githubusercontent.com`
- **Audience**: `sts.amazonaws.com`

Then click **Next** and add the policy from step 2 below. Name the role `newuuid-deploy`.

Alternatively, create the role via CLI:

```bash
aws iam create-role \
  --role-name newuuid-deploy \
  --assume-role-policy-document file://trust-policy.json
```

Where `trust-policy.json` restricts access to `main` only:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Federated": "arn:aws:iam::048044547730:oidc-provider/token.actions.githubusercontent.com"
      },
      "Action": "sts:AssumeRoleWithWebIdentity",
      "Condition": {
        "StringEquals": {
          "token.actions.githubusercontent.com:aud": "sts.amazonaws.com",
          "token.actions.githubusercontent.com:sub": "repo:roberthodgen/newuuid-com:ref:refs/heads/main"
        }
      }
    }
  ]
}
```

The `StringEquals` condition on `sub` ensures only pushes to `main` can assume this role. Feature branches, forks, and pull requests are blocked.

### 2. Attach the IAM policy

Attach this inline policy to the `newuuid-deploy` role. It allows the workflow to upload Lambda deployment artifacts, deploy the `newuuid-net` CloudFormation stack, and let CloudFormation manage the Lambda/API Gateway resources defined by `serverless.template`.

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:PutObject",
        "s3:DeleteObject",
        "s3:ListBucket"
      ],
      "Resource": [
        "arn:aws:s3:::newuuid-net",
        "arn:aws:s3:::newuuid-net/*"
      ]
    },
    {
      "Effect": "Allow",
      "Action": [
        "cloudformation:CreateChangeSet",
        "cloudformation:CreateStack",
        "cloudformation:DeleteChangeSet",
        "cloudformation:DescribeChangeSet",
        "cloudformation:DescribeStackEvents",
        "cloudformation:DescribeStacks",
        "cloudformation:ExecuteChangeSet",
        "cloudformation:GetTemplateSummary",
        "cloudformation:UpdateStack"
      ],
      "Resource": "arn:aws:cloudformation:us-east-1:048044547730:stack/newuuid-net/*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "cloudformation:CreateChangeSet",
        "cloudformation:DescribeChangeSet",
        "cloudformation:ExecuteChangeSet"
      ],
      "Resource": "arn:aws:cloudformation:us-east-1:aws:transform/Serverless-2016-10-31"
    },
    {
      "Effect": "Allow",
      "Action": [
        "lambda:AddPermission",
        "lambda:CreateAlias",
        "lambda:CreateFunction",
        "lambda:DeleteAlias",
        "lambda:DeleteFunction",
        "lambda:GetAlias",
        "lambda:GetFunction",
        "lambda:GetFunctionConfiguration",
        "lambda:ListVersionsByFunction",
        "lambda:PublishVersion",
        "lambda:RemovePermission",
        "lambda:UpdateAlias",
        "lambda:UpdateFunctionCode",
        "lambda:UpdateFunctionConfiguration"
      ],
      "Resource": "arn:aws:lambda:us-east-1:048044547730:function:newuuid-net-*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "apigateway:*"
      ],
      "Resource": "arn:aws:apigateway:us-east-1::/apis/kkunh73aga"
    },
    {
      "Effect": "Allow",
      "Action": [
        "iam:AttachRolePolicy",
        "iam:CreateRole",
        "iam:DeleteRole",
        "iam:DetachRolePolicy",
        "iam:GetRole",
        "iam:PassRole",
        "iam:PutRolePolicy",
        "iam:DeleteRolePolicy"
      ],
      "Resource": "arn:aws:iam::048044547730:role/newuuid-net-*"
    }
  ]
}
```

### 3. Verify

After creating the role, push a change to `main` and check **Actions > Deploy** in the GitHub UI. The job should build the .NET app and update the `newuuid-net` stack.

## Manual deployment (fallback)

The `Makefile` is still available for local deployment using your AWS CLI profile:

```bash
AWS_PROFILE=personal-prod make deploy
```

This builds `build/newuuid-net.zip` and deploys the `newuuid-net` CloudFormation stack to `us-east-1` using the `newuuid-net` S3 bucket.

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| `Role arn:aws:iam::...:newuuid-deploy cannot be assumed` | Role doesn't exist or trust policy is wrong | Verify the role exists and the OIDC trust policy matches the repo/branch |
| `AccessDenied` on S3 | Policy is missing bucket permissions or the bucket name is wrong | Verify access to `arn:aws:s3:::newuuid-net` and `arn:aws:s3:::newuuid-net/*` |
| `AccessDenied` on CloudFormation, Lambda, API Gateway, or IAM | Deployment policy is missing an action/resource required by SAM | Check the failed action in the workflow logs and adjust the inline policy |
| Workflow deploys but the site is unavailable | Stack update failed or API endpoint/custom domain mapping needs attention | Check the CloudFormation stack events and Lambda logs |
| Workflow doesn't trigger on push | Branch name mismatch | The workflow only triggers on `main` |
