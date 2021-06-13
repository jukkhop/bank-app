<p align="center">
  <img src="https://raw.githubusercontent.com/jukkhop/bank-app/master/frontend/public/logo192.png" />
  <h3 align="center">Bank App</h3>
  <h4 align="center"><a href="https://bank-app.purelogic.xyz/">bank-app.purelogic.xyz</a></h4>
</p>

## Disclaimer

This project is a personal exercise in programming and for trying different approaches. As of currently, this software cannot be used to do anything useful.

## Tools and libraries used

Miscellaneous

- AWS as cloud provider (API Gateway, CloudFront, Lambda, RDS, Route53, S3, VPC)
- [Terraform](https://github.com/hashicorp/terraform) for infrastructure-as-code
- [git-crypt](https://github.com/AGWA/git-crypt) for encryption

Backend

- F#, .NET Core, PostgreSQL
- [FSharp.Json](https://github.com/vsapronov/FSharp.Json) as JSON library
- [Npgsql](https://github.com/npgsql/npgsql) as Postgres connector library
- [xUnit](https://github.com/xunit/xunit) as testing framework
- [FSUnit](https://github.com/fsprojects/FsUnit) as assertion library
- [Foq](https://github.com/fsprojects/Foq) as mocking library

Frontend

- React, TypeScript, ESLint, Prettier
- [RxJs](https://github.com/ReactiveX/rxjs) for reactive fetching and rendering (in combination with [React Hooks](https://reactjs.org/docs/hooks-overview.html))
- [Bulma](https://github.com/jgthms/bulma) as CSS framework
- [React Hook Form](https://github.com/react-hook-form/react-hook-form) as form library
- [react-bulma-components](https://github.com/couds/react-bulma-components) for UI components based on Bulma
- [react-fontawesome](https://github.com/FortAwesome/react-fontawesome) for icons
- [date-fns](https://github.com/date-fns/date-fns) as date/time library

## Local development

#### Backend

Currently there is no option to run the actual API locally. In order to test the frontend against the backend, deploy manually to `dev` environment first.

To run unit tests:

- Install [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1) and [Docker](https://www.docker.com/get-started)
- Change directory `cd database`
- Up the database `docker-compose up -d`
- Change directory `cd backend/tests`
- Install deps `dotnet restore`
- Run tests `dotnet test`

#### Frontend

- Change directory `cd frontend`
- Install deps `npm install`
- Adjust `REACT_APP_BACKEND_URL` in `.env` as needed
- Run `npm run start`
- Your browser should automatically open at http://localhost:3000/

## Cloud deployment

Environment-specific variables and secrets are kept in the `environment` folder. Secrets are encrypted, make sure to run `git-crypt unlock` to decrypt them.

#### Manual deployment (infrastructure)

- Install [Terraform](https://www.terraform.io/)
- Change directory `cd infrastructure`
- Deploy `./deploy.sh <env>`

#### Manual deployment (backend)

- Install [Serverless](https://www.serverless.com/)
- Change directory `cd backend`
- Build `./build.sh`
- Deploy `./deploy.sh <env>`

#### Manual deployment (frontend)

- Install [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html)
- Change directory `cd frontend`
- Build `./build.sh <env>`
- Deploy `./deploy.sh <env>`

#### Run database migrations

- Install [Flyway CLI](https://flywaydb.org/documentation/usage/commandline/)
- Change directory `cd database`
- Execute `./run-migrations.sh <env>`

#### Automated deployment

Coming later.
