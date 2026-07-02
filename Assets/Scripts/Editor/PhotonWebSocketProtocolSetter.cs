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
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            if (!PhotonAppSettings.TryGetGlobal(out var settings))
                return;

            if (settings.AppSettings.Protocol == ConnectionProtocol.WebSocket)
                return;

            settings.AppSettings.Protocol = ConnectionProtocol.WebSocket;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
