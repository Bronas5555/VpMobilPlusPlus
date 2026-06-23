using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.UserControls.Controls;
using VpMobilPlusPlus.Views;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.UserControls.Pages;

public partial class StartPage : UserControl
{

    public static StartPage Instance;

    public List<ClassSelectionPinnableElement> ClassListElements = new List<ClassSelectionPinnableElement>();
    public List<ClassSelectionPinnableElement> ClassListElementsPinned = new List<ClassSelectionPinnableElement>();
    public StartPage()
    {
        InitializeComponent();
        Instance = this;

        _ = Init();
        
        
        SettingsButton.Click += (sender, args) =>
        {
            MainWindow.DisplayPage(UiPages.SettingsPage);
        };

    }

    private async Task Init()
    {
        await SaveManager.LoadAsync();
        await LoadAsync();
    }
    
    public async Task LoadAsync()
    {
        try
        {
            VPlan? newestPlan = await PlanProvider.GetNewest();

            if (newestPlan?.plan.Klassen.Kl == null)
                return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ClassListElements.Clear();
                ClassListElementsPinned.Clear();
                
                int i = 0;
                Console.WriteLine("Start: " + newestPlan.plan.Klassen.Kl.Count);

                foreach (var kl in newestPlan.plan.Klassen.Kl)
                {
                    var element = new ClassSelectionPinnableElement(kl.Kurz, i);

                    var saveElement = SaveManager.ClassList?
                        .FirstOrDefault(x => x.Name == kl.Kurz);

                    if (saveElement != null)
                        element.PinnedBox.IsChecked = saveElement.IsPinned;

                    if (element.PinnedBox.IsChecked == false)
                        ClassListElements.Add(element);
                    else
                        ClassListElementsPinned.Add(element);

                    element.canFire = true;
                    i++;
                }

                ReloadClassListElements();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                TextBlock block = new TextBlock();
                block.Text = ex.Message + "| " + ex.InnerException;
                block.TextWrapping = TextWrapping.Wrap;
                ClassList.Children.Add(block);
            });
            
        }
    }

    public static void DisplayError(string error)
    {
        TextBlock block = new TextBlock();
        block.Text = error;
        block.TextWrapping = TextWrapping.Wrap;
        Instance.ClassList.Children.Add(block);
    }

    public static void PinClassListElement(ClassSelectionPinnableElement element)
    {
        Instance.ClassListElements.Remove(element);
        Instance.ClassListElementsPinned.Add(element);
        
        ReloadClassListElements();

    }
    public static void UnpinClassListElement(ClassSelectionPinnableElement element)
    {
        Instance.ClassListElements.Add(element);
        Instance.ClassListElementsPinned.Remove(element);
        
        ReloadClassListElements();
    }

    private static void ReloadClassListElements()
    {
        Instance.ClassList.Children.Clear();

        Instance.ClassListElementsPinned = Instance.ClassListElementsPinned
            .OrderBy(x =>
            {
                var match = Regex.Match(x.ClassNameBlock.Text, @"^\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            })
            .ThenBy(x => x.ClassNameBlock.Text)
            .ToList();
        
        Instance.ClassListElements = Instance.ClassListElements
            .OrderBy(x =>
            {
                var match = Regex.Match(x.ClassNameBlock.Text, @"^\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            })
            .ThenBy(x => x.ClassNameBlock.Text)
            .ToList();

        Instance.ClassList.Children.Clear();
        
        Instance.ClassList.Children.AddRange(Instance.ClassListElementsPinned);
        Instance.ClassList.Children.AddRange(Instance.ClassListElements);
        
        SaveManager.ClassList.Clear();
        foreach (var element in Instance.ClassListElements)
        {
            SaveManager.ClassList.Add(new ClassSelectionSaveElement(element.ClassNameBlock.Text, false));
        }

        foreach (var element in Instance.ClassListElementsPinned)
        {
            SaveManager.ClassList.Add(new ClassSelectionSaveElement(element.ClassNameBlock.Text, true));
        }
        
        _ = SaveManager.SaveAsync();
    }
}

public class ClassSelectionSaveElement
{
    public string Name;
    public bool IsPinned;

    public ClassSelectionSaveElement(string name, bool isPinned)
    {
        Name = name;
        IsPinned = isPinned;
    }
}