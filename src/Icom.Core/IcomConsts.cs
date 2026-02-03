using Icom.Debugging;

namespace Icom;

public class IcomConsts
{
    public const string LocalizationSourceName = "Icom";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "35810135deee4f838e70fcf4ae03c24e";
}
