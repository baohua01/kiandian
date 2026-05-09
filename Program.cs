using System.Globalization;
using System.Windows.Forms;

namespace WinCalculator;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new CalculatorForm());
    }
}

public sealed class CalculatorForm : Form
{
    private readonly TextBox _display;
    private double? _firstOperand;
    private string? _operation;
    private bool _isNewInput = true;

    public CalculatorForm()
    {
        Text = "桌面计算器";
        Width = 340;
        Height = 500;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        _display = new TextBox
        {
            ReadOnly = true,
            Text = "0",
            TextAlign = HorizontalAlignment.Right,
            Font = new System.Drawing.Font("Segoe UI", 24F),
            Dock = DockStyle.Top,
            Height = 70
        };

        Controls.Add(_display);
        Controls.Add(CreateButtonsPanel());
    }

    private TableLayoutPanel CreateButtonsPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5,
            Padding = new Padding(8)
        };

        for (int i = 0; i < 4; i++)
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        for (int i = 0; i < 5; i++)
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        string[,] keys =
        {
            { "C", "⌫", "%", "÷" },
            { "7", "8", "9", "×" },
            { "4", "5", "6", "-" },
            { "1", "2", "3", "+" },
            { "±", "0", ".", "=" }
        };

        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                var key = keys[r, c];
                var button = new Button
                {
                    Text = key,
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Segoe UI", 16F)
                };
                button.Click += (_, _) => HandleInput(key);
                panel.Controls.Add(button, c, r);
            }
        }

        return panel;
    }

    private void HandleInput(string key)
    {
        if (char.IsDigit(key, 0))
        {
            AppendDigit(key);
            return;
        }

        switch (key)
        {
            case ".":
                AppendDecimalPoint();
                break;
            case "C":
                ClearAll();
                break;
            case "⌫":
                Backspace();
                break;
            case "±":
                ToggleSign();
                break;
            case "%":
                ApplyPercent();
                break;
            case "+":
            case "-":
            case "×":
            case "÷":
                SetOperation(key);
                break;
            case "=":
                ComputeResult();
                break;
        }
    }

    private void AppendDigit(string digit)
    {
        if (_isNewInput)
        {
            _display.Text = digit;
            _isNewInput = false;
            return;
        }

        _display.Text = _display.Text == "0" ? digit : _display.Text + digit;
    }

    private void AppendDecimalPoint()
    {
        if (_isNewInput)
        {
            _display.Text = "0.";
            _isNewInput = false;
            return;
        }

        if (!_display.Text.Contains('.'))
            _display.Text += ".";
    }

    private void ClearAll()
    {
        _display.Text = "0";
        _firstOperand = null;
        _operation = null;
        _isNewInput = true;
    }

    private void Backspace()
    {
        if (_isNewInput || _display.Text.Length <= 1)
        {
            _display.Text = "0";
            return;
        }

        _display.Text = _display.Text[..^1];
    }

    private void ToggleSign()
    {
        if (_display.Text == "0") return;
        _display.Text = _display.Text.StartsWith("-") ? _display.Text[1..] : "-" + _display.Text;
    }

    private void ApplyPercent()
    {
        if (!TryGetDisplayValue(out var value)) return;
        value /= 100;
        SetDisplay(value);
    }

    private void SetOperation(string op)
    {
        if (TryGetDisplayValue(out var value))
        {
            if (_firstOperand.HasValue && !_isNewInput)
            {
                ComputeResult();
                TryGetDisplayValue(out value);
            }
            _firstOperand = value;
            _operation = op;
            _isNewInput = true;
        }
    }

    private void ComputeResult()
    {
        if (!_firstOperand.HasValue || _operation is null) return;
        if (!TryGetDisplayValue(out var secondOperand)) return;

        double result = _operation switch
        {
            "+" => _firstOperand.Value + secondOperand,
            "-" => _firstOperand.Value - secondOperand,
            "×" => _firstOperand.Value * secondOperand,
            "÷" => secondOperand == 0 ? double.NaN : _firstOperand.Value / secondOperand,
            _ => secondOperand
        };

        SetDisplay(result);
        _firstOperand = null;
        _operation = null;
        _isNewInput = true;
    }

    private bool TryGetDisplayValue(out double value)
    {
        return double.TryParse(_display.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private void SetDisplay(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            _display.Text = "错误";
            _isNewInput = true;
            return;
        }

        _display.Text = value.ToString("G15", CultureInfo.InvariantCulture);
    }
}
