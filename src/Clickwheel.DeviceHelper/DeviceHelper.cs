namespace Clickwheel.DeviceHelper;

public static class DeviceHelper
{
    public static string GetExtendedSysInfoFromDrive(string driveLetter)
    {
        return DeviceXml.Get(driveLetter);
    }
}