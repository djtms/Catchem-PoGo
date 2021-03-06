﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Catchem
{
    static class Adb
    {
        public static async Task<DeviceData> GetDeviceData()
        {
            var dd = new DeviceData();
            foreach (var field in typeof(DeviceData).GetFields())
            {
                var args = field.GetCustomAttribute<AdbArgumentsAttribute>();
                if (args == null) continue;
                var lcmdInfo1 = new ProcessStartInfo(@"adb\adb.exe")
                {
                    Arguments = args.Arguments,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                var cmd2 = new Process { StartInfo = lcmdInfo1 };
                var output = new StringBuilder();
                var error = new StringBuilder();
                cmd2.OutputDataReceived += (o, ef) => output.Append(ef.Data);
                cmd2.ErrorDataReceived += (o, ef) => error.Append(ef.Data);
                cmd2.Start();
                cmd2.BeginOutputReadLine();
                cmd2.BeginErrorReadLine();
                cmd2.WaitForExit();
                cmd2.Close();
                field.SetValue(dd, output.ToString());
                cmd2.Dispose();
                await Task.Delay(10);
            }
            return dd;
        }
    }

    internal class AdbArgumentsAttribute : Attribute
    {
        public string Arguments;

        public AdbArgumentsAttribute(string args)
        {
            Arguments = args;
        }
    }

    internal class DeviceData
    {
        [AdbArguments("shell settings get secure android_id")]
        public string DeviceId = "";
        [AdbArguments("shell getprop ro.product.board")]
        public string AndroidBoardName = "";
        [AdbArguments("shell getprop ro.boot.bootloader")]
        public string AndroidBootloader = "";
        [AdbArguments("shell getprop ro.product.brand")]
        public string DeviceBrand = "";
        [AdbArguments("shell getprop ro.product.model")]
        public string DeviceModel = "";
        [AdbArguments("shell getprop ro.product.name")]
        public string DeviceModelIdentifier = "";
        [AdbArguments("shell getprop ro.product.manufacturer")]
        public string HardwareManufacturer = "";
        [AdbArguments("shell getprop ro.product.model")]
        public string HardwareModel = "";
        [AdbArguments("shell getprop ro.product.name")]
        public string FirmwareBrand = "";
        [AdbArguments("shell getprop ro.build.tags")]
        public string FirmwareTags = "";
        [AdbArguments("shell getprop ro.build.type")]
        public string FirmwareType = "";
        [AdbArguments("shell getprop ro.build.fingerprint")]
        public string FirmwareFingerprint = "";
    }
}
