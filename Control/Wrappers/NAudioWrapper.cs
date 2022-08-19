﻿using NAudio.CoreAudioApi;
using System.Text.RegularExpressions;
using Shared.Control;
using Shared.Logging.Interfaces;

namespace Control.Wrappers
{
    public class NAudioWrapper: BaseWrapper, IAudioControl
    {
        private MMDevice _defaultDevice;
        private readonly IEnumerable<MMDevice> _devices;

        private static readonly Regex GuidRegex =
            new ("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public NAudioWrapper(ILogger logger): base(logger)
        {
            var enumerator = new MMDeviceEnumerator();

            _devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        private static Guid GetGuid(string input) => new(GuidRegex.Match(input).Value);

        public int GetVolume() => (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

        public void SetVolume(int volume) => _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;

        public void Mute() => _defaultDevice.AudioEndpointVolume.Mute = true;

        public void Unmute() => _defaultDevice.AudioEndpointVolume.Mute = false;

        public IReadOnlyCollection<IAudioDevice> GetDevices()
        {
            return _devices.Select(x =>
                new AudioDevice
                {
                    Id = GetGuid(x.ID),
                    IsCurrentControlDevice = x.ID == _defaultDevice.ID,
                    Name = x.FriendlyName
                }).ToList();
        }

        public IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id)
        {
            _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
            return GetDevices();
        }
    }
}
