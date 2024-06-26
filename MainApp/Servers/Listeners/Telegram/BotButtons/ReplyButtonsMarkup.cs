﻿namespace MainApp.Servers.Listeners.Telegram.BotButtons;

internal class ReplyButtonsMarkup(IEnumerable<IEnumerable<SingleButton>> items) : IButtonsMarkup
{
    public IEnumerable<IEnumerable<SingleButton>> Items { get; set; } = items;

    public bool Resize { get; set; }
    public bool Persistent { get; set; }
    public bool OneTime { get; set; }
}