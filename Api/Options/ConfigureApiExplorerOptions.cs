using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

namespace Api.Options;

public class ConfigureApiExplorerOptions : IConfigureOptions<ApiExplorerOptions>
{
    public void Configure(ApiExplorerOptions options)
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    }
}