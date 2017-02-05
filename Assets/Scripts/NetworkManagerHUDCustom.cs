//#if ENABLE_UNET

using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkManagerHUD")]
	[RequireComponent(typeof(NetworkManager))]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class NetworkManagerHUDCustom : MonoBehaviour
	{
		public NetworkManager manager;
        public GameObject startUI;
        public Text unitCounter;
        int units = 0;
        public bool showUnits;
        
        //NetworkMatch
		// Runtime variable
		bool showServer = false;
        

		void Awake()
		{
			manager = GetComponent<NetworkManager>();
            unitCounter.gameObject.SetActive(false);
        }

        public void AddUnit()
        {
            if (units == 0 && showUnits)
            {
                unitCounter.gameObject.SetActive(true);
            }
            unitCounter.text = "" + ++units;
        }

        #region Network Connections
        public void StartMatchMaking()
        {
            NetworkManager.singleton.StartMatchMaker();
            NetworkManager.singleton.matchMaker.ListMatches(0, 20, "Match", false, 0, 1, OnMatchList);
            Debug.Log("Searching");
            startUI.SetActive(false);
        }

        public void StartHost()
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                manager.StartHost();
                startUI.SetActive(false);
            }
        }

        public void StartClient()
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                manager.StartClient();
                manager.networkAddress = manager.networkAddress;
                startUI.SetActive(false);
            }
        }

        public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                print("Created Match");
                MatchInfo hostInfo = matchInfo;
                NetworkServer.Listen(hostInfo, 9000);
                NetworkManager.singleton.StartHost(hostInfo);
                     
            }
            else
            {
                Debug.LogError("Failed to create match");
            }
        }

        public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Match Joined");
                MatchInfo hostInfo = matchInfo;
                NetworkManager.singleton.StartClient(hostInfo);

                //OnConnect();
            }
            else
            {
                Debug.Log("ERROR : Match Join Failure");
            }
        }

        public virtual void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
        {
            if (success)
            {
                if (matchList.Count != 0)
                {
                    Debug.Log("Matches Found");
                    NetworkManager.singleton.matchMaker.JoinMatch(matchList[0].networkId, "", "", "", 0, 1, OnMatchJoined);
                }
                else
                {
                    Debug.Log("No Matches Found");
                    Debug.Log("Creating Match");
                    NetworkManager.singleton.matchMaker.CreateMatch("Match", 2, true, "", "", "", 0, 1, OnMatchCreate);
                }
            }
            else
            {
                Debug.Log("ERROR : Match Search Failure");
            }
        }
        #endregion
    }
}
