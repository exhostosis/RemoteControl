﻿using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bots.Telegram.ApiObjects.Request;
using Bots.Telegram.ApiObjects.Response;

namespace Bots.Telegram;

internal class TelegramBotApiWrapper
{
    private readonly string _requestUri;

    private readonly HttpClient _client = new();

    private int? _lastUpdateId = null;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public TelegramBotApiWrapper(string uri, string apikey)
    {
        _requestUri = $"{uri}{apikey}/";
    }

    private async Task<string> SendBotApiRequest(string method, object parameters)
    {
        var content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json");

        var req = new HttpRequestMessage(HttpMethod.Post, _requestUri + method)
        {
            Content = content
        };

        var response = await _client.SendAsync(req);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Error sending request to bot api", null, response.StatusCode);
        }
            
        return await response.Content.ReadAsStringAsync();
    }

    private static ReplyKeyboardMarkup GenerateKeyboardMarkup(IEnumerable<IEnumerable<string>> buttons)
    {
        return new ReplyKeyboardMarkup
        {
            ResizeKeyboard = true,
            Keyboard = buttons.Select(x => x.Select(y => new KeyboardButton { Text = y }).ToArray()).ToArray()
        };
    }

    public async Task<UpdateResponse> GetUpdates()
    {
        var parameters = new GetUpdatesParameters();

        if (_lastUpdateId.HasValue)
        {
            parameters.Offset = _lastUpdateId + 1;
        }

        var responseString = await SendBotApiRequest(ApiMethods.GetUpdates, parameters);

        var parsedResponse = JsonSerializer.Deserialize<UpdateResponse>(responseString) ?? throw new JsonException("Cannot parse api response");

        _lastUpdateId = parsedResponse.Result.LastOrDefault()?.UpdateId;

        return parsedResponse;
    }

    public async Task<string> SendResponse(int chatId, string message, IEnumerable<IEnumerable<string>>? buttons = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message can not be empty", nameof(message));

        var parameters = new SendMessageParameters
        {
            ChatId = chatId,
            Text = message,
            ReplyMarkup = buttons == null ? null : GenerateKeyboardMarkup(buttons)
        };

        return await SendBotApiRequest(ApiMethods.SendMessage, parameters);
    }
}