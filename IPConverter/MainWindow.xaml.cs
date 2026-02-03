using System.Windows;
using System.Windows.Controls;

namespace IPConverter;

public partial class MainWindow : Window
{
    private const string Placeholder = "Example: 67.173.216.42";

    public MainWindow()
    {
        InitializeComponent();
        IpAddressInput.Text = Placeholder;
        IpAddressInput.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 153, 153));
    }

    private void IpAddressInput_GotFocus(object sender, RoutedEventArgs e)
    {
        if (IpAddressInput.Text == Placeholder)
        {
            IpAddressInput.Text = "";
        }
        IpAddressInput.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
    }

    private void IpAddressInput_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(IpAddressInput.Text))
        {
            IpAddressInput.Text = Placeholder;
            IpAddressInput.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 153, 153));
        }
    }

    private void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = "";
        ResultOutput.Clear();

        string input = IpAddressInput.Text?.Trim() ?? "";

        // Don't process if placeholder is showing
        if (input == Placeholder || string.IsNullOrWhiteSpace(input))
        {
            ErrorMessage.Text = "Please enter an IP address.";
            return;
        }

        if (!System.Net.IPAddress.TryParse(input, out var ipAddress))
        {
            ErrorMessage.Text = "Invalid IP address format.";
            return;
        }

        var bytes = ipAddress.GetAddressBytes();

        if (bytes.Length != 4)
        {
            ErrorMessage.Text = "Please enter a valid IPv4 address.";
            return;
        }

        // Formula: (octet[0] × 256³) + (octet[1] × 256²) + (octet[2] × 256) + octet[3]
        long decimalValue = (bytes[0] * 16777216L) + (bytes[1] * 65536L) + (bytes[2] * 256L) + bytes[3];

        ResultOutput.Text = decimalValue.ToString();
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ResultOutput.Text))
        {
            ErrorMessage.Text = "No result to copy. Convert an IP first.";
            return;
        }

        Clipboard.SetText(ResultOutput.Text);
        ErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
        ErrorMessage.Text = "✓ Copied to clipboard!";
    }
}
