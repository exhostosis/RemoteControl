﻿using System;
using System.Net;
using System.Threading.Tasks;
using RemoteControl.Core.Enums;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Services;
using RemoteControl.Core.Utility;

namespace RemoteControl.Core.Controllers
{
    internal partial class ApiController
    {
        private readonly IAudioService _audioService;
        private readonly IInputService _inputService;
        private readonly Point _point;

        public ApiController()
        {
            _inputService = new InputsimService();
            _audioService = new AudioService();
            _point = new Point();
        }

        private int ProcessAudio(string value)
        {
            if (value == "init") return _audioService.Volume;

            if (!int.TryParse(value, out var result)) return 0;

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            _audioService.Volume = result;

            _audioService.Mute(result == 0);

            return 0;
        }

        private void ProcessKeyboard(string value)
        {
            switch (value)
            {
                case "back":
                    _inputService.KeyPress(KeysEnum.Back);
                    break;
                case "forth":
                    _inputService.KeyPress(KeysEnum.Forth);
                    break;
                case "pause":
                    _inputService.KeyPress(KeysEnum.Pause);
                    break;
                case "mediaback":
                    _inputService.KeyPress(KeysEnum.MediaBack); 
                    break;
                case "mediaforth":
                    _inputService.KeyPress(KeysEnum.MediaForth);
                    break;
            }
        }

        private void ProcessText(string text)
        {
            _inputService.TextInput(text);
            _inputService.KeyPress(KeysEnum.Enter);
        }

        
        private void ProcessMouse(string value)
        {
            switch (value)
            {
                case "left":
                    _inputService.MouseKeyPress();
                    break;
                case "right":
                    _inputService.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "middle":
                    _inputService.MouseKeyPress(MouseKeysEnum.Middle);
                    break;
                case "up":
                    _inputService.MouseWheel(true);
                    break;
                case "down":
                    _inputService.MouseWheel(false);
                    break;
                case "dragstart":
                    _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    });
                    break;
                case "dragstop":
                    _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    break;
                default:
                    if (_point.TrySetCoords(WebUtility.UrlDecode(value).Replace("\"", "")))
                    {
                        _inputService.MouseMove(_point);
                    }
                    break;
            }
        }
    }
}
