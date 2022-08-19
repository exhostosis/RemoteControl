﻿using System.Net;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace RemoteControlApp.Controllers
{
    [Controller("keyboard")]
    public class KeyboardController: BaseController
    {
        private readonly IKeyboardControlProvider _input;

        public KeyboardController(IKeyboardControlProvider input, ILogger logger) : base(logger)
        {
            _input = input;
        }

        [Action("back")]
        public string? Back(string _)
        {
            _input.KeyPress(KeysEnum.ArrowLeft);

            return "done";
        }

        [Action("forth")]
        public string? Forth(string _)
        {
            _input.KeyPress(KeysEnum.ArrowRight);

            return "done";
        }

        [Action("pause")]
        public string? Pause(string _)
        {
            _input.KeyPress(KeysEnum.MediaPlayPause);

            return "done";
        }

        [Action("mediaback")]
        public string? MediaBack(string _)
        {
            _input.KeyPress(KeysEnum.MediaPrev);

            return "done";
        }

        [Action("mediaforth")]
        public string? MediaForth(string _)
        {
            _input.KeyPress(KeysEnum.MediaNext);

            return "done";
        }

        [Action("text")]
        public string? TextInput(string param)
        {
            _input.TextInput(WebUtility.UrlDecode(param));
            _input.KeyPress(KeysEnum.Enter);

            return "done";
        }
    }
}
