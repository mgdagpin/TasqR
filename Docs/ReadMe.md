#### Nuget Local Environment
https://dev.azure.com/sprikiwikiltd/Marcelius/_packaging?_a=feed&feed=nuget.sprikiwiki.ltd


#### Pack the library
Project -> Properties -> Packages (tab) -> update the version patch (segment)

#### enerate your personal token (PAT)
https://dev.azure.com/sprikiwikiltd/_usersSettings/tokens

#### Procedure PAT
New Token -> Name (Optional) -> Scopes: **Packaging** Read, Write & Manage -> Create


#### Push Procedure
Open a command prompt (cmd) to *bin/release/* path

#### Push Command
 nuget push TasqR.**{VERSION}**.nupkg -Source https://pkgs.dev.azure.com/sprikiwikiltd/_packaging/nuget.sprikiwiki.ltd/nuget/v3/index.json -ApiKey **{Personal Token (PAT)}**



----
#### Using Package Manager Console
```
dotnet nuget push TasqR.1.0.6.nupkg -s https://pkgs.dev.azure.com/sprikiwikiltd/_packaging/nuget.sprikiwiki.ltd/nuget/v3/index.json -k {PAT}
```