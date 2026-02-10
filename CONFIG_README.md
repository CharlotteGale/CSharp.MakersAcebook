# Local Configuration

## Setting Up `appsettings.Development.json`
> Note: This is your secrets file and must always be in `.gitignore`

In removing the secrets, there is now a small config step that's need to connect to your database.

- Add `appsettings.Development.json` to `Acebook/` from the project root
    * `touch Acebook/appsettings.Development.json` 
- Add the `"ConnectionStrings"` to `appsettings.Development.json`.      
    * It should look like this:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Username=<YOUR USERNAME>;Password=1234;Database=acebook_csharp_test"
  }
}
```
> Note: If you're unsure what your username should be, type `whoami` into the terminal and copy & paste that in place of `<YOUR USERNAME>`

## Getting Chromedriver to Work
The Chromedriver we're told to install from the README is looking for a newer version of Chrome than what's available. 

- In `Acebook.Tests.csproj` add the following line:
    * `<PackageReference Include="Selenium.WebDriver.ChromeDriver" Version="144.0.6367.7800" />`

    Then run:
    * `dotnet restore`

- Remove the global installation of Chromedriver with the following steps:
    * `which chromedriver`

> This gives a path to where Selenium is getting the version of Chromedriver. Mine looked like this: `/opt/homebrew/bin/chromedriver`.

- Next:
    * `rm <PATH_TO_YOUR_CHROMEDRIVER>`

Now, Selenium is forced to use the Chromedriver in the NuGet packages rather than the global installation.

## Generating a Cert for Secure Connection
Selenium/Chromedriver have been trying to run the tests on a secure connection but there are no certs for localhost on the machine, so it's been throwing a connection error.
Quick and simple steps to rectify this one.

1. Just a sanity check to clear any certs if you've created them in the past:       
    * `dotnet dev-certs https --clean`
    > This will require a password in the terminal.
2. Generate a new local cert:
    * `dotnet dev-certs https --trust`
    > This will also want a password, a popup from the OS.
3. A success message will be generated in the terminal if all goes well:
    * `Successfully created and trusted a new HTTPS certificate.`

Now, when a server is started with `dotnet run` or `dotnet watch` the secure connection will be established and the tests will fail where they're meant to fail.
