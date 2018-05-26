# Linkster

Linkster is a simple URL shortner (think bit.ly) built on ASP.NET Core and a AWS DynamoDB backend.

The original intention behind this project was a challenge to myself to build something in an evening using a "new"
AWS service (new to me). It's still very rough, but I plan on adding to it.

When you visit the `/short-links` page, it will check for a Dynamo DB table called "ShortLinks" and create it if not.  This relies on you having an [AWS credentials file](https://docs.aws.amazon.com/cli/latest/userguide/cli-config-files.html).

Currently, the "short link" is being generated from the first 7 characters of a GUID; this needs to be updated.

Some new feature / update ideas:

- Authentication for managing links
- Being able to specify your own "short" link
- Keep track of how many times a link was clicked and when it was clicked
- Ability to delete links
- Change to a serverless model
  - Create a serverless API (Lambda)
  - Create a front-end (probably Angular or Vue), host in S3 behind Cloudfront

### Resources

[Pollster project from AWS Labs' NDC Oslo talk](https://github.com/awslabs/aws-sdk-net-samples/tree/master/Talks/ndcoslo-2017/Pollster)