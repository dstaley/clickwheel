using System.Runtime.InteropServices;
using Windows.Win32.Storage.IscsiDisc;

namespace Clickwheel.DeviceHelper
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ScsiPassThroughWithBuffers
    {
        public SCSI_PASS_THROUGH spt;
        public uint Filler;
        public fixed byte ucSenseBuf[32];
        public fixed byte ucDataBuf[255];
    }
}