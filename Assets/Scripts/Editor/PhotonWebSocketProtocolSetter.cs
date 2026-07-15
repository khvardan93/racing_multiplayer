using ExitGames.Client.Photon;
using Fusion.Photon.Realtime;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace RacerCar.Editor
{
    // Forces WebSocket transport in PhotonAppSettings for WebGL builds, since UDP/TCP sockets aren't available in the browser.
    public class PhotonWebSocketProtocolSetter : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!PhotonAppSettings.TryGetGlobal(out var settings))
                return;

            var protocol = report.summary.platform == BuildTarget.WebGL
                ? ConnectionProtocol.WebSocket
                : ConnectionProtocol.Udp;
            
            if (settings.AppSettings.Protocol == protocol)
                return;

            settings.AppSettings.Protocol = protocol;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
