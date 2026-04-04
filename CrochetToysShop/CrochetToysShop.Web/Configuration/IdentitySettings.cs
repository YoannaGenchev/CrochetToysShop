public sealed class IdentitySettings
{
    public const string SectionName = "Identity";

    public SignInSettings SignIn { get; set; } = new();
    public PasswordSettings Password { get; set; } = new();

    public sealed class SignInSettings
    {
        public bool RequireConfirmedAccount { get; set; }
    }

    public sealed class PasswordSettings
    {
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public int RequiredLength { get; set; }
    }
}