namespace CarHunters.Core.Settings
{
	public static class SettingUriService
	{
        public static string BaseUri => "api.spy.by";
        public static int Port => 443;

        public static string SecureHeader => "Authorization";
        public static string Bearer => "Bearer";

	    public static string PrivateRestUri => $"{BaseUri}";
	    public static string PublicRestUri => $"{BaseUri}";
	}
}
