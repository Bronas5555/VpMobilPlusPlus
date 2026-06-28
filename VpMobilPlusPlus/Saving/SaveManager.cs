using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media;
using VpMobilPlusPlus.UserControls.Pages;

namespace VpMobilPlusPlus.Saving;

public static class SaveManager
{
    public static Dictionary<int, List<FilterElement>> CourseFilters = new();
    public static List<ClassSelectionSaveElement> ClassList = new();
    
    public static int schoolNumber = -1;
    public static string username = "";
    public static string password = "";

    private static readonly string AppName = "VpMobilPlusPlus";
    private static readonly string fileName = "settings.json";
    
    private static bool hasLoaded = false;
    

    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true
    };


    public static async Task SaveAsync()
    {
        if (!hasLoaded)
        {
            Console.WriteLine("Loading before Saving");
            await LoadAsync();
        }
            

        string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string appFolder = Path.Combine(settingsFolder, AppName);
        Directory.CreateDirectory(appFolder);

        string saveFile = Path.Combine(appFolder, fileName);

        bool cred = SecureStorage.SaveCredentials(schoolNumber, username, password);
        
        Console.WriteLine("Credentials: " + cred);
        VpMobilPlusPlusSaveData data;
        if (cred)
        {
            data = new VpMobilPlusPlusSaveData(
                CourseFilters,
                ClassList,
                CourseColorSelectionPage.courseColors,
                -12,
                "",
                ""
            );
            Console.WriteLine("Saving without credentials");
        }
        else
        {
            data = new VpMobilPlusPlusSaveData(
                CourseFilters,
                ClassList,
                CourseColorSelectionPage.courseColors,
                schoolNumber,
                username,
                password
            );
            Console.WriteLine("Saving with credentials");
        }
        

        await using FileStream stream = new FileStream(
            saveFile,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            4096,
            useAsync: true
        );

        await JsonSerializer.SerializeAsync(
            stream,
            data,
            jsonOptions
        );
    }


    public static async Task LoadAsync()
    {
        string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = Path.Combine(settingsFolder, AppName);

        string settings = Path.Combine(appFolder, fileName);

        if (!File.Exists(settings))
        {
            hasLoaded = true;
            return;
        }


        await using FileStream stream = new FileStream(
            settings,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true
        );


        VpMobilPlusPlusSaveData? data =
            await JsonSerializer.DeserializeAsync<VpMobilPlusPlusSaveData>(
                stream,
                jsonOptions
            );


        if (data == null)
            return;

        CredentialData? credentials = SecureStorage.LoadCredentials();
        if (credentials != null)
        {
            data.Username = credentials.Username;
            data.Password = credentials.Password;
            data.SchoolNumber = credentials.SchoolNumber;
        }
        
        
        if (data.CourseFilters != null)
        {
            CourseFilters = data.CourseFilters;
            CourseFilterPage.CourseFilters = data.CourseFilters;
        }


        if (data.ClassList != null)
            ClassList = data.ClassList;


        CourseColorSelectionPage.courseColors = data.GetCourseColors();
        
        schoolNumber = data.SchoolNumber;
        username = data.Username;
        password = data.Password;

        CourseColorSelectionPage.AddNewCourses();
        
        SettingsPage.Instance.SchoolNumberBox.Text = data.SchoolNumber.ToString();
        SettingsPage.Instance.UserNameBox.Text = data.Username;
        SettingsPage.Instance.PasswordBox.Text = data.Password;
        
        hasLoaded = true;

        _ = StartPage.Instance.LoadAsync();
    }

    public static int GetSchoolNumber()
    {
        return schoolNumber;
    }
}

class VpMobilPlusPlusSaveData
{
    public Dictionary<int, List<FilterElement>> CourseFilters { get; set; }
    public List<ClassSelectionSaveElement> ClassList { get; set; }
    public Dictionary<int, Dictionary<string, uint>> CourseColors { get; set; }
    
    public int SchoolNumber { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public VpMobilPlusPlusSaveData(
        Dictionary<int, List<FilterElement>> courseFilters,
        List<ClassSelectionSaveElement> classList,
        Dictionary<int, Dictionary<string, Color>> courseColors,
        int schoolNumber,
        string username,
        string password)
    {
        CourseFilters = courseFilters;
        ClassList = classList;

        CourseColors = new Dictionary<int, Dictionary<string, uint>>();

        foreach (var classPair in courseColors)
        {
            CourseColors[classPair.Key] = new Dictionary<string, uint>();

            foreach (var colorPair in classPair.Value)
            {
                CourseColors[classPair.Key][colorPair.Key] = colorPair.Value.ToUInt32();
            }
        }
        SchoolNumber = schoolNumber;
        Username = username;
        Password = password;
        
    }

    public VpMobilPlusPlusSaveData()
    {
        CourseFilters = new Dictionary<int, List<FilterElement>>();
        ClassList = new List<ClassSelectionSaveElement>();
        CourseColors = new Dictionary<int, Dictionary<string, uint>>();
    }

    public Dictionary<int, Dictionary<string, Color>> GetCourseColors()
    {
        Dictionary<int, Dictionary<string, Color>> result = new();

        foreach (var classPair in CourseColors)
        {
            Dictionary<string, Color> classColors = new();

            foreach (var colorPair in classPair.Value)
            {
                classColors[colorPair.Key] = Color.FromUInt32(colorPair.Value);
            }

            result[classPair.Key] = classColors;
        }

        return result;
    }
}

