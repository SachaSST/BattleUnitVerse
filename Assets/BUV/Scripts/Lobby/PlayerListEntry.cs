// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using TMPro;
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI PlayerNameText;

        public Image PlayerColorImage;
        public Image PlayerColorImage2;

        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady;
        public void Listener(){
            isPlayerReady = !isPlayerReady; // toggle the player's ready state
            //activate the ready button (setplayerready) if the player is ready 
            if (isPlayerReady){
                SetPlayerReady(!isPlayerReady);
            }
            else{
                SetPlayerReady(isPlayerReady);
            }
            Hashtable props = new Hashtable() {{BUVGame.PLAYER_READY, isPlayerReady}}; // set the player's ready state
            PhotonNetwork.LocalPlayer.SetCustomProperties(props); // set the player's custom properties
            if (PhotonNetwork.IsMasterClient) // if the local player is the master client
            {
                Object.FindFirstObjectByType<LobbyMainPanel>().LocalPlayerPropertiesUpdated(); // update the local player's properties in the lobby main panel
            }
                    
        }

        #region UNITY
        


        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }


        public void Start()
        {
            
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId) // if this is not the local player
            {
                Debug.Log("PlayerListEntry:Start() this is not the local player");
            }
            else
            {   
                Hashtable initialProps = new Hashtable() {{BUVGame.PLAYER_READY, isPlayerReady}, {BUVGame.PLAYER_LIVES, BUVGame.PLAYER_MAX_LIVES}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

            }
        }

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            Debug.Log("PlayerListEntry:Initialize() ownerId: " + ownerId);
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId) // if the player is the owner of this entry
                {
                    if (BUVGame.GetColor(p.GetPlayerNumber()) == Color.red)
                    {   PlayerColorImage.enabled = false;
                        PlayerColorImage2.enabled = true;
                    }
                    else
                    {
                        PlayerColorImage2.enabled = false;
                        PlayerColorImage.enabled = true;
                    }
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
        PlayerReadyImage.color = playerReady ? Color.green : Color.red;
        }
    }
