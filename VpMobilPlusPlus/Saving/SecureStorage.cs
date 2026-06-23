using System;
using Meziantou.Framework.Win32;
using Plugin.SecureStorage;

namespace VpMobilPlusPlus.Saving;

public class SecureStorage
{
    private static readonly string SecureStorageSchoolNumber = "schoolNumber";
    private static readonly string SecureStorageUsername = "username";
    private static readonly string SecureStoragePassword = "password";
    
    public static bool SaveCredentials(int schoolNumber, string username, string password)
    {
        if (OperatingSystem.IsLinux()) return SaveCredentialsLinux();
        
        if (OperatingSystem.IsWindows() && OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            return SaveCredentialsWindows(schoolNumber, username, password);
        
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return SaveCredentialsMobile(schoolNumber, username, password);

        return false;
    }

    public static CredentialData? LoadCredentials()
    {
        if (OperatingSystem.IsLinux()) return LoadCredentialsLinux();

        if (OperatingSystem.IsWindows() && OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            return LoadCredentialsWindows();

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
            return new CredentialData(secureUsername, securePassword, int.Parse(secureSchoolNumber));
        }
        return null;
    }

    private static CredentialData? LoadCredentialsWindows()
    {

        return null;
    }

    private static CredentialData? LoadCredentialsLinux()
    {
        
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
    
    private static bool SaveCredentialsWindows(int schoolNumber, string username, string password)
    {
        if (!OperatingSystem.IsWindows() || !OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600)) return false;
        CredentialManager.WriteCredential(
            "VpMobilPlusPlus",
            username,
            password + ":" + schoolNumber,
            persistence: CredentialPersistence.LocalMachine);
        return false;
    }
    
    private static bool SaveCredentialsLinux()
    {

        return false;
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