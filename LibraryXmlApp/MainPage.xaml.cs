// Головна сторінка: лише обробка подій інтерфейсу. Пошук і перетворення виконують стратегії та сервіси.
using LibraryXmlApp.Services;
using LibraryXmlApp.Strategies;

namespace LibraryXmlApp;

public partial class MainPage : ContentPage
{
    // Шаблон Strategy: список доступних способів пошуку
    private readonly List<ISearchStrategy> _strategies = new()
    {
        new SaxSearchStrategy(),
        new DomSearchStrategy(),
        new LinqSearchStrategy()
    };

    private Dictionary<string, List<string>> _attributes = new();
    private string? _filePath;
    private bool _exitConfirmed;
    private bool _closingHooked;

    public MainPage()
    {
        InitializeComponent();
        StrategyPicker.ItemsSource = _strategies.Select(s => s.Name).ToList();
        StrategyPicker.SelectedIndex = 0;
        Loaded += (s, e) => HookWindowClosing();
    }

    private async void OnChooseFileClicked(object? sender, EventArgs e)
    {
        var options = new PickOptions
        {
            PickerTitle = "Оберіть XML-файл",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xml" } }
            })
        };

        var file = await FilePicker.Default.PickAsync(options);
        if (file == null) return;

        try
        {
            _attributes = AttributeLoader.Load(file.FullPath);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка", "Не вдалося прочитати файл: " + ex.Message, "OK");
            return;
        }

        _filePath = file.FullPath;
        FileLabel.Text = file.FileName;
        ClearSearch();
        AttributePicker.ItemsSource = _attributes.Keys.ToList();
    }

    // Підгрузка значень: після вибору атрибута показуємо лише ті значення, що є у файлі
    private void OnAttributeChanged(object? sender, EventArgs e)
    {
        if (AttributePicker.SelectedItem is string name)
            ValuePicker.ItemsSource = _attributes[name];
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        if (_filePath == null
            || AttributePicker.SelectedItem is not string attribute
            || ValuePicker.SelectedItem is not string value)
        {
            await DisplayAlertAsync("Увага", "Оберіть файл, атрибут і значення.", "OK");
            return;
        }

        var strategy = _strategies[StrategyPicker.SelectedIndex];
        var books = strategy.Search(_filePath, attribute, value);

        OutputEditor.Text = $"Спосіб: {strategy.Name}. Знайдено книг: {books.Count}"
                          + Environment.NewLine + Environment.NewLine
                          + string.Join(Environment.NewLine, books);
    }

    private void OnClearClicked(object? sender, EventArgs e) => ClearSearch();

    private void ClearSearch()
    {
        OutputEditor.Text = string.Empty;
        AttributePicker.SelectedIndex = -1;
        ValuePicker.ItemsSource = null;
        StrategyPicker.SelectedIndex = 0;
    }

    private async void OnTransformClicked(object? sender, EventArgs e)
    {
        if (_filePath == null)
        {
            await DisplayAlertAsync("Увага", "Спочатку оберіть XML-файл.", "OK");
            return;
        }

        try
        {
            string htmlPath = await HtmlTransformService.TransformAsync(_filePath);
            await Launcher.Default.OpenAsync(new OpenFileRequest("library.html", new ReadOnlyFile(htmlPath)));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка", "Не вдалося виконати перетворення: " + ex.Message, "OK");
        }
    }

    private async void OnExitClicked(object? sender, EventArgs e)
    {
        if (await ConfirmExitAsync())
        {
            _exitConfirmed = true;
            Application.Current?.Quit();
        }
    }

    private Task<bool> ConfirmExitAsync() =>
        DisplayAlertAsync("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");

    // Підтвердження також при закритті вікна хрестиком (лише Windows)
    private void HookWindowClosing()
    {
#if WINDOWS
        if (_closingHooked) return;
        if (Window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            _closingHooked = true;
            nativeWindow.AppWindow.Closing += async (s, e) =>
            {
                if (_exitConfirmed) return;
                e.Cancel = true;
                if (await ConfirmExitAsync())
                {
                    _exitConfirmed = true;
                    Application.Current?.Quit();
                }
            };
        }
#endif
    }
}