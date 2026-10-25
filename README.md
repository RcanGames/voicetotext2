
# VoiceToText

Modern Windows masaüstü konuşma ve metin uygulaması.

## Özellikler

- Modern HTML, CSS ve JavaScript arayüzü
- C# ve WPF masaüstü kabuğu
- Microsoft Edge WebView2 entegrasyonu
- Metni TXT dosyası olarak kaydetme
- Ses dosyası seçme arayüzü
- GitHub Actions ile Windows x64 derlemesi

## Kullanılan teknolojiler

- C#
- .NET 8
- WPF
- Microsoft WebView2
- HTML5
- CSS3
- JavaScript
- GitHub Actions

## Proje yapısı

```text
VoiceToText/
├── VoiceToText.csproj
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── ui/
│   ├── index.html
│   ├── style.css
│   └── app.js
└── .github/
    └── workflows/
        └── build.yml
```

## Windows uygulamasını derleme

1. GitHub deposunun Actions sekmesini aç.
2. Build VoiceToText for Windows iş akışını seç.
3. Run workflow düğmesine bas.
4. Derlemenin tamamlanmasını bekle.
5. Başarılı çalışmanın altındaki Artifacts bölümünden
   `VoiceToText-Windows` dosyasını indir.
6. ZIP dosyasını açıp uygulama dosyalarını çıkar.

## Geliştirme durumu

Bu proje geliştirme aşamasındadır.

Mikrofonla konuşma tanıma ve ses dosyalarını metne dönüştürme
motorlarının ayrıca tamamlanması gerekir.

## Lisans

Lisans henüz belirlenmedi.
