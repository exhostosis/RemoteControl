﻿using System.Text.RegularExpressions;

namespace RemoteControl.App.Utility
{
    internal static class MyPoint
    {

        //(?<=[xy]:[ ]*) match starts with 'x' or 'y' then ':' and zero or more ' '
        //[-0-9]+ main pattern. one or more '-' or numbers
        //(?=[,} ]) match ends with ',', '}', ' ', or ']'
        private static readonly Regex CoordRegex = new("[-0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool TryGetCoords(string input, out int x, out int y)
        {
            x = 0;
            y = 0;

            try
            {
                var matches = CoordRegex.Matches(input);

                x = Convert.ToInt32(matches[0].Value);
                y = Convert.ToInt32(matches[1].Value);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
