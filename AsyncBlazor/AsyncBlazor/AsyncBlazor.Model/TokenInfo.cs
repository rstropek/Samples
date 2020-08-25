namespace AsyncBlazor.Model
{
    /// <summary>
    /// Information for token creation
    /// </summary>
    /// <remarks>
    /// In this sample, real-world authentication is not in scope.
    /// Therefore, a symmetrically signed JWT with dummy secret, 
    /// issuer and audience is used.
    /// </remarks>
    public static class TokenInfo
    {
        public const string Secret = "this-is-my-secret";
        public const string Issuer = "http://auth.acme.com";
        public const string Audience = "http://async-blazor.acme.com";
    }
}
