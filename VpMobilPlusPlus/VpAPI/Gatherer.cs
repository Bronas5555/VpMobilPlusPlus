using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.UserControls.Pages;
using VpMobilPlusPlus.Util;

namespace VpMobilPlusPlus.VpAPI;

public class Gatherer
{
    public static bool GatherDate(DateOnly date)
    {
        string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{SaveManager.username}:{SaveManager.password}"));

        var handler = new HttpClientHandler();
        using HttpClient client = new HttpClient(handler);

        int schoolNumber = SaveManager.GetSchoolNumber();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        string url = "https://www.stundenplan24.de/" + schoolNumber + "/mobil/mobdaten/PlanKl" +
                     date.Year.ToString("00") + date.Month.ToString("00") + date.Day.ToString("00") + ".xml";
        Console.WriteLine(url);
        HttpResponseMessage response;
        try
        {
            response = client.GetAsync(url).Result;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            StartPage.DisplayError(e.Message);
            return false;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine("Plan not found");
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"HTTP error: {(int)response.StatusCode} {response.ReasonPhrase}");
            StartPage.DisplayError($"HTTP error: {(int)response.StatusCode} {response.ReasonPhrase}");
            return false;
        }

        using Stream stream = response.Content.ReadAsStreamAsync().Result;
        XmlSerializer serializer = new XmlSerializer(typeof(VpMobil));
        VpMobil vpMobil = (VpMobil)serializer.Deserialize(stream)!;
        VPlan vplan = new VPlan(vpMobil);


        ListUtil.AddOrUpdate(PlanProvider.plans, vplan);
        return true;
    }
}