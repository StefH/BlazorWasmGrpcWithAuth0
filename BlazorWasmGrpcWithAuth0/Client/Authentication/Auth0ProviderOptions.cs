using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication
{
    public class Auth0ProviderOptions : OidcProviderOptions
    {
        /// <summary>
        /// Gets or sets the audience of the application.
        /// </summary>
        [JsonPropertyName("audience")]
        public string Audience { get; set; }
    }
}