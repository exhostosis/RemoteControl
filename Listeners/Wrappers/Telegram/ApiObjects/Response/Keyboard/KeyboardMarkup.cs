﻿using System.Text.Json.Serialization;

namespace Listeners.Wrappers.Telegram.ApiObjects.Response.Keyboard;

[JsonDerivedType(typeof(ReplyKeyboardMarkup), "reply")]
[JsonDerivedType(typeof(ReplyKeyboardRemove), "remove")]
[JsonDerivedType(typeof(InlineKeyboardMarkup), "inline")]
[JsonDerivedType(typeof(KeyboardForceReply), "force")]
public abstract class KeyboardMarkup
{
}