# OpenCart Web API Project

This repository contains a sample implementation of a web API using .NET Core 6, Entity Framework Core, and GitHub authentication.

## Getting Started

Follow these steps to set up the project on your local machine.

### Prerequisites

- [.NET Core 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Docker (Optional, but helpful if you want to containerize the application)

### Clone the Repository

```bash
git clone https://github.com/YoungMiGo123/OpenCart.git
cd OpenCart
```

### Setup Configuration
Navigate to the API project folder.
```bash
cd OpenCart.Api
```

Update OpenCartSettings. Open the appsettings.json file or your environment-specific configurations and set 
your ClientId and ClientSecret and other related details. Ensure you setup the connection string:
```bash
  "OpenCartSettings": {
    "ConnectionString": "<YOUR CONNECTION STRING>",
    "SecureOAuthSettings": {
      "ClientId": "<YOUR CLIENT ID>",
      "ClientSecret": "<YOUR CLIENT SECRET ID>",
      "RedirectUrl": "<YOUR REDIRECT URL>"
    },
    "SeqUrl": "<Your SEQ URL>"
  }
```

Default details if you using docker can be the below 

```bash
  "OpenCartSettings": {
    "ConnectionString": "Server=sqlserver;Database=OpenCart;User Id=YOURUSERNAME;Password=YOURSECUREPASSWORD;",
    "SecureOAuthSettings": {
      "ClientId": "<GithubClientID>",
      "ClientSecret": "<GithubClientSecret>",
      "RedirectUri": "http://OpenCartapi:80/swagger"
    },
    "SeqUrl": "http://seq:5341/"
  }
```

### Note on Authentication:
This project uses GitHub OAuth2 for authentication. Ensure you've registered an OAuth App on GitHub and obtained your ClientId and ClientSecret. 

The callback URL in your GitHub OAuth App configuration should be set to https://localhost:7142/SecureGithubCallbackResponse

### Authentication
Swagger UI, by default, makes HTTP calls to your endpoints. When you're triggering an OAuth 
flow (which expects redirects and interactions with external websites), HTTP calls isn't the right approach. 

Instead of using Swagger UI to initiate the OAuth flow, try accessing the Github Login endpoint directly from your browser. 
```bash
Navigate to:
https://localhost:7142/SecureLoginUsingGitHub
```
This should trigger the OAuth flow, redirecting you to GitHub for authentication.

I have intentionally excluded the 'SecureLoginUsingGitHub' and 'SecureGithubCallbackResponse' with github endpoint to prevent confusion.
```bash
[ApiExplorerSettings(IgnoreApi = true)]
```

### Database Setup
Open the solution in Visual Studio or Rider, go to package manager console and set the default project to the Infrastructure project folder.
```bash 
/OpenCart.Infrastructure
```

Run Entity Framework Core migrations to create the Migration.
```bash
Add-Migration <NameOfYourMigration>
```

Add a side note. To apply the migrations to the database are done by running the following command:
```bash
Update-Database
```

As a side note all this will be handled for you when starting the application, all current migrations to run the project will automatically be run

The above steps are optional, as long as the connection string has been passed to the application it will run as expected

### Run the Application
```bash
Click run via Visual Studio
```

OR Alternatively run from command line

```bash
dotnet run
```


### API Documentation
You can access the API documentation using Swagger UI. Open your browser and navigate to:
```bash
https://localhost:7142/swagger
```
