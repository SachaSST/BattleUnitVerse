using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

    public class LobbyTopPanel : MonoBehaviour
    {
        private readonly string connectionStatusMessage = "Connection Status: "; //

        [Header("UI References")]
        public TextMeshProUGUI ConnectionStatusText;

        #region UNITY

        public void Update()
        {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }
        #endregion
    }
