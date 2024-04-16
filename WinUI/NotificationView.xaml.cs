using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;

namespace WinUI;

public sealed partial class NotificationPage
{
    private readonly ViewModel _viewModel = ViewModelProvider.GetViewModel();
    private readonly App _app;

    public NotificationPage()
    {
        this.InitializeComponent();

        _app = Application.Current as App ?? throw new NullReferenceException();
    }

    [RelayCommand]
    void LeftClick()
    {
        var window = _app.MainWindow;

        if(window.Visible)
            window.Hide();
        else
            window.Show();
    }

    [RelayCommand]
    void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}