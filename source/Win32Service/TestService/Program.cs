﻿using PeanutButter.EasyArgs;
using PeanutButter.INI;
using PeanutButter.ServiceShell;

namespace TestService
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static int Main(string[] args)
        {
            // hack: since we're kinda taking over args here
            // we want --help to be useful, so parse to _all_
            // options and discard
            var opts = args.ParseTo<ServiceOptions>(
                out _,
                new ParserOptions()
                {
                    IgnoreUnknownSwitches = true
                });
            if (opts.Install)
            {
                SaveIniValue(
                    TotallyNotInterestingService.SECTION_DELAY,
                    nameof(opts.StartDelay),
                    opts.StartDelay.ToString()
                );
                SaveIniValue(
                    TotallyNotInterestingService.SECTION_DELAY,
                    nameof(opts.PauseDelay),
                    opts.PauseDelay.ToString()
                );
                SaveIniValue(
                    TotallyNotInterestingService.SECTION_DELAY,
                    nameof(opts.StopDelay),
                    opts.StopDelay.ToString()
                );
            }

            TotallyNotInterestingService.Options = opts;

            return Shell.RunMain<TotallyNotInterestingService>(
                args
            );
        }

        private static void SaveIniValue(
            string section,
            string setting,
            string value
        )
        {
            var ini = new INIFile(TotallyNotInterestingService.IniFilePath);
            ini.SetValue(section, setting, value);
            ini.Persist();
        }
    }
}