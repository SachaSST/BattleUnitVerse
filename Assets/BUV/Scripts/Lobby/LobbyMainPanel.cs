using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {

        [Header("Login Panel")]
        [SerializeField] private GameObject LoginPanel;
    
        [SerializeField] private TMP_InputField PlayerEmailInput;
        [SerializeField] private TMP_InputField PlayerPasswordInput;

        [Header("Menu Panel")]
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private Button JoinSelectRoomButton;

        [SerializeField] private TextMeshProUGUI PlayerNameText;
        [SerializeField] private TextMeshProUGUI PlayerLvlText;
        [SerializeField] private Slider SliderLvl;
        [SerializeField] private TextMeshProUGUI PlayerLvlSlide;

        [Header("Other Panel")]
        [SerializeField] private GameObject ShopPanel;
        [SerializeField] private GameObject InventoryPanel;

        [Header("Selection Panel")]
        [SerializeField] private GameObject SelectionPanel;

        [Header("Create Room Panel")]
        [SerializeField] private GameObject CreateRoomPanel;

        [SerializeField] private TMP_InputField RoomNameInputField;
        [SerializeField] private TMP_InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        [SerializeField] private GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        [SerializeField] private GameObject RoomListPanel;

        [SerializeField] private GameObject RoomListContent;
        [SerializeField] private GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        [SerializeField] private GameObject InsideRoomPanel;
        [SerializeField] private GameObject VerticalLayoutGroup;
        [SerializeField] private Button StartGameButton;
        [SerializeField] private GameObject PlayerListEntryPrefab;

        [Header("Settings Panel")]
        [SerializeField] private GameObject SettingsPanel;
        
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;



        #region UNITY

        public void Awake() // Start
        {

            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();

            // le nombre max de carractere pour le mail et le mot de passe est de 20 pour le mail et 12 pour le mdp
            PlayerEmailInput.characterLimit = 20;
            PlayerPasswordInput.characterLimit = 12;

            // rendre le panel Lobby invisible
            SetActivePanel(LoginPanel.name);
            
        }

        //methode pour ce déconneter et partir sur la page de login
        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
            SetActivePanel(LoginPanel.name);
        }
        
        public void ExitGameButtonClicked()
        {
            //quitter le jeu
            PhotonNetwork.Disconnect();
            Application.Quit();
        }
        public void closedSettingsPanel()
        {
            //desactive le panel des settings
            SettingsPanel.gameObject.SetActive(false);        
        }
        public void OpenSettingsPanel()
        {
            //active le panel des settings
            SettingsPanel.gameObject.SetActive(true);
        }

        public void SetTextXP()
        {
            //mettre à jour le niveau du joueur
            PlayerLvlSlide.text = (int)(SliderLvl.value*400) + "/400";
 
        }

        public void OpenShopPanel()
        {
            SetActivePanel(ShopPanel.name);
        }

        public void OpenInventoryPanel()
        {
            SetActivePanel(InventoryPanel.name);
        }

        public void CloseShopPanel()
        {
            SetActivePanel(MenuPanel.name);
        }

        public void CloseInventoryPanel()
        {
            SetActivePanel(MenuPanel.name);
        }
    

        
        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(MenuPanel.name);
            PlayerNameText.text = PhotonNetwork.NickName;
            PlayerLvlText.text = "10";
            SliderLvl.value = (float)0.3;

        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();
            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions {MaxPlayers = 4};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();


            SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(VerticalLayoutGroup.transform);
                // Get component rect transform de entry
                RectTransform rectTransform = entry.GetComponent<RectTransform>();
                // set le z de entry à 0
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f); 
                //set le scale de entry à 0.85
                entry.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(BUVGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);//

            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {BUVGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }
            //faire que le joueur qui a quitté la room ne soit plus ready
            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(VerticalLayoutGroup.transform);
            // Get component rect transform de entry
            RectTransform rectTransform = entry.GetComponent<RectTransform>();
            // set le z de entry à 0
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f); 
            entry.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(BUVGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }
        

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 4); // 2 et 4 sont les valeurs min et max de joueurs

            RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000 }; // PlayerTtl est le temps en ms avant qu'un joueur soit retiré de la room

            PhotonNetwork.CreateRoom(roomName, options, null); // On crée la room
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            
        }

        public void OnLoginButtonClicked()
        {
            //le player Name est le début du mail sans tout ce qui est après le @
            string playerName = PlayerEmailInput.text;
            if (!playerName.Equals(""))
            {
                PhotonNetwork.NickName = playerName;
                
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
            
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("BUV-GameScene");
        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(BUVGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        
        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            MenuPanel.SetActive(activePanel.Equals(MenuPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));     
            ShopPanel.SetActive(activePanel.Equals(ShopPanel.name));   
            InventoryPanel.SetActive(activePanel.Equals(InventoryPanel.name));     

        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);
                entry.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                entry.transform.localPosition = Vector3.zero;
                roomListEntries.Add(info.Name, entry);
            }
        }
    }