using System;
using Plugin.SecureStorage;
using SIL.Secrets;

namespace VpMobilPlusPlus.Saving;

public class SecureStorage
{
    private static readonly string SecureStorageSchoolNumber = "schoolNumber";
    private static readonly string SecureStorageUsername = "username";
    private static readonly string SecureStoragePassword = "password";
    private static readonly string AppName = "VpMobilPlusPlus";
    
    public static bool SaveCredentials(int schoolNumber, string username, string password)
    {
        if (OperatingSystem.IsLinux()) return SaveCredentialsDesktop(schoolNumber, username, password);
        
        if (OperatingSystem.IsWindows() && OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            return SaveCredentialsDesktop(schoolNumber, username, password);
        
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return SaveCredentialsMobile(schoolNumber, username, password);

        return false;
    }

    public static CredentialData? LoadCredentials()
    {
        if (OperatingSystem.IsLinux()) return LoadCredentialsDesktop();

        if (OperatingSystem.IsWindows()) return LoadCredentialsDesktop();

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return LoadCredentialsMobile();
        return null;
    }

    private static CredentialData? LoadCredentialsMobile()
    {
        if (CrossSecureStorage.IsSupported)
        {
            string secureSchoolNumber = CrossSecureStorage.Current.GetValue(SecureStorageSchoolNumber);
            string secureUsername = CrossSecureStorage.Current.GetValue(SecureStorageUsername);
            string securePassword = CrossSecureStorage.Current.GetValue(SecureStoragePassword);
            
            Console.WriteLine("Loaded Secure Storage Mobile");

            if (secureSchoolNumber == null || secureUsername == null || securePassword == null) return null;
            return new CredentialData(secureUsername, securePassword, int.Parse(secureSchoolNumber));
        }
        return null;
    }
    

    private static CredentialData? LoadCredentialsDesktop()
    {
        string? secureString = PasswordStore.GetPassword(AppName, AppName);
        if (secureString != null)
        {
            string[] split = secureString.Split(':');
            
            string username = split[0];
            string password = split[1];
            string schoolNumber = split[2];
            return new CredentialData(username, password, int.Parse(schoolNumber));
        }
        return null;
    }
    
    private static bool SaveCredentialsMobile(int schoolNumber, string username, string password)
    {
        if(CrossSecureStorage.IsSupported == false) return false;
        bool nmb = CrossSecureStorage.Current.SetValue(SecureStorageSchoolNumber, schoolNumber.ToString());
        bool name = CrossSecureStorage.Current.SetValue(SecureStorageUsername, username);
        bool pwd = CrossSecureStorage.Current.SetValue(SecureStoragePassword, password);
        Console.WriteLine("Saved Credentials Mobile");
        return nmb && name && pwd;
    }
    
    private static bool SaveCredentialsDesktop(int schoolNumber, string username, string password)
    {
        try
        {
            Console.WriteLine("Saving Credentials Desktop");
            PasswordStore.SetPassword(AppName, AppName, username + ":" + password + ":" + schoolNumber);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}

public class CredentialData
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int SchoolNumber { get; set; }
    
    public CredentialData(string username, string password, int schoolNumber)
    {
        Username = username;
        Password = password;
        SchoolNumber = schoolNumber;
    }
}