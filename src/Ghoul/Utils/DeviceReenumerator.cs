using System.Runtime.InteropServices;

namespace Ghoul.Utils
{
    public interface IDeviceReenumerator
    {
        bool Reenumerate();
    }

    public class DeviceReenumerator
        : IDeviceReenumerator
    {
        // refactored from https://stackoverflow.com/questions/2181525/is-there-an-api-call-to-start-a-scan-for-hardware-devices#2836197
        //    causes device re-enumeration, which can make monitors which are plugged in but
        //    not currently available be detected
        private const int CM_LOCATE_DEVNODE_NORMAL = 0x00000000;
        private const int CM_REENUMERATE_NORMAL = 0x00000000;
        private const int CR_SUCCESS = 0x00000000;

        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Locate_DevNodeA(ref int pdnDevInst, string pDeviceID, int ulFlags);

        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Reenumerate_DevNode(int dnDevInst, int ulFlags);

        public bool Reenumerate()
        {
            int pdnDevInst = 0;

            return CM_Locate_DevNodeA(ref pdnDevInst, null, CM_LOCATE_DEVNODE_NORMAL) == CR_SUCCESS &&
                   CM_Reenumerate_DevNode(pdnDevInst, CM_REENUMERATE_NORMAL) == CR_SUCCESS;
        }
    }
}