
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceToText;

public partial class MainWindow : Window
{
    private string UiPath =>
        Path.Combine(AppContext.BaseDirectory, "ui", "index.html");

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(
        object sender, RoutedEventArgs e)
    {
        try
        {
            await Browser.EnsureCoreWebView2Async();

            Browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
            Browser.CoreWebView2.Settings.IsStatusBarEnabled = false;

            Browser.CoreWebView2.WebMessageReceived +=
                CoreWebView2_WebMessageReceived;

            if (!File.Exists(UiPath))
            {
                MessageBox.Show(
                    $"Arayüz dosyası bulunamadı:\n{UiPath}",
                    "VoiceToText",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Browser.CoreWebView2.Navigate(
                new Uri(UiPath).AbsoluteUri);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Arayüz başlatılamadı.\n\n" + ex.Message,
                "VoiceToText",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void CoreWebView2_WebMessageReceived(
        object? sender,
        CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            using var document =
                JsonDocument.Parse(e.WebMessageAsJson);

            var root = document.RootElement;

            if (!root.TryGetProperty("action", out var actionElement))
                return;

            string action = actionElement.GetString() ?? "";

            switch (action)
            {
                case "saveText":
                {
                    string text =
                        root.TryGetProperty("text", out var textElement)
                        ? textElement.GetString() ?? ""
                        : "";

                    SaveText(text);
                    break;
                }

                case "selectAudio":
                    SelectAudio();
                    break;

                case "startSpeech":
                    await SendStatusAsync(
                        "Konuşma tanıma motoru henüz bağlanmadı.");
                    break;

                case "stopSpeech":
                    await SendStatusAsync("Dikte durduruldu.");
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "VoiceToText",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void SaveText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            MessageBox.Show(
                "Kaydedilecek metin yok.",
                "VoiceToText");
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Metni kaydet",
            FileName = "VoiceToText_Metin.txt",
            DefaultExt = ".txt",
            Filter = "Metin dosyası (*.txt)|*.txt"
        };

        if (dialog.ShowDialog() != true)
            return;

        File.WriteAllText(
            dialog.FileName,
            text,
            new System.Text.UTF8Encoding(true));

        _ = SendStatusAsync("Metin kaydedildi.");
    }

    private void SelectAudio()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Ses dosyası seç",
            Filter =
                "Ses dosyaları|*.wav;*.mp3;*.m4a;*.flac;*.ogg;*.wma|" +
                "Tüm dosyalar|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            _ = SendStatusAsync(
                "Dosya seçildi: " +
                Path.GetFileName(dialog.FileName) +
                ". Ses çözümleme motoru henüz bağlanmadı.");
        }
    }

    private async Task SendStatusAsync(string text)
    {
        if (Browser.CoreWebView2 is null)
            return;

        string json = JsonSerializer.Serialize(new
        {
            action = "status",
            text
        });

        await Browser.CoreWebView2.PostWebMessageAsJson(json);
    }
}
