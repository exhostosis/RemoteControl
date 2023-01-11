﻿using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls.Panels;

internal class ServerPanel : ProcessorPanel
{
    private readonly Label _schemeLabel = new()
    {
        Text = @"Scheme",
        Location = new Point(6, 36),
        Size = new Size(50, 15)
    };

    private readonly TextBox _schemeTextBox = new()
    {
        Location = new Point(60, 32),
        Size = new Size(53, 23),
        BorderStyle = BorderStyle.FixedSingle
    };

    private readonly Label _hostLabel = new()
    {
        Text = @"Host",
        Location = new Point(120, 36),
        Size = new Size(32, 15)
    };

    private readonly TextBox _hostTextBox = new()
    {
        Location = new Point(155, 32),
        Size = new Size(143, 23),
        BorderStyle = BorderStyle.FixedSingle
    };

    private readonly Label _portLabel = new()
    {
        Text = @"Port",
        Location = new Point(304, 36),
        Size = new Size(29, 15)
    };

    private readonly TextBox _portTextBox = new()
    {
        Location = new Point(338, 32),
        Size = new Size(35, 23),
        BorderStyle = BorderStyle.FixedSingle
    };

    private readonly IDisposable _unsubscriber;

    public ServerPanel(ServerProcessor processor) : base(processor)
    {
        _schemeTextBox.Text = processor.CurrentConfig.Scheme;
        _hostTextBox.Text = processor.CurrentConfig.Host;
        _portTextBox.Text = processor.CurrentConfig.Port.ToString();

        _schemeTextBox.TextChanged += EnableUpdateButton;
        _hostTextBox.TextChanged += EnableUpdateButton;
        _portTextBox.TextChanged += EnableUpdateButton;

        _portTextBox.TextChanged += (sender, args) =>
        {
            _portTextBox.Text = new string(_portTextBox.Text.ToCharArray().Where(char.IsDigit).ToArray());
            if (_portTextBox.Text.Length > 5)
                _portTextBox.Text = _portTextBox.Text[..5];

            _portTextBox.SelectionStart = _portTextBox.Text.Length;
            _portTextBox.SelectionLength = 0;
        };

        Controls.AddRange(new Control[]
        {
            _schemeLabel,
            _schemeTextBox,
            _hostLabel,
            _hostTextBox,
            _portLabel,
            _portTextBox
        });

        _unsubscriber = processor.Subscribe(new Observer<ServerConfig>(ConfigChanged));

        Disposed += LocalDispose;
    }

    private void ConfigChanged(ServerConfig config)
    {
        NameTextBox.Text = config.Name;
        AutostartBox.Checked = config.Autostart;
        _schemeTextBox.Text = config.Scheme;
        _hostTextBox.Text = config.Host;
        _portTextBox.Text = config.Port.ToString();
    }

    protected override void UpdateButtonClick(object? sender, EventArgs e)
    {
        ApplyTheme(Theme);

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            NameLabel.ForeColor = Color.OrangeRed;
            return;
        }

        if (!int.TryParse(_portTextBox.Text, out var port))
        {
            _portLabel.ForeColor = Color.OrangeRed;
            return;
        }

        Uri? uri;
        try
        {
            uri = new UriBuilder(_schemeTextBox.Text, _hostTextBox.Text, port).Uri;
        }
        catch
        {
            _schemeLabel.ForeColor = Color.OrangeRed;
            _portLabel.ForeColor = Color.OrangeRed;
            _hostLabel.ForeColor = Color.OrangeRed;

            return;
        }

        var config = new ServerConfig
        {
            Autostart = AutostartBox.Checked,
            Uri = uri,
            Name = NameTextBox.Text
        };

        RaiseUpdateButtonClickedEvent(config);

        UpdateButton.Enabled = false;
    }

    private void LocalDispose(object? sender, EventArgs e)
    {
        _unsubscriber.Dispose();
    }
}