## Storyboard

```bash
dotnet new console
dotnet add package OpenAI --version 2.0.0-beta.5
dotnet add package Azure.AI.OpenAI --version 2.0.0-beta.2
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
dotnet user-secrets init
dotnet user-secrets set "OpenAI_Key" "sk-proj-67Z..."
dotnet user-secrets set "Azure_OpenAI_Endpoint" "https://oai-rstropek-sweden.openai.azure.com/"
dotnet user-secrets set "Azure_OpenAI_Key" "119..."
```

```xml
<UserSecretsId>896d9df4-3224-4d2b-bf5f-b91b3ac101ec</UserSecretsId>
```

## Links

* [OpenAI NuGet](https://www.nuget.org/packages/OpenAI/)
* [Azure OpenAI NuGet](https://www.nuget.org/packages/Azure.AI.OpenAI/)
* [OpenAI Code Examples](https://github.com/openai/openai-dotnet/tree/main/examples)

