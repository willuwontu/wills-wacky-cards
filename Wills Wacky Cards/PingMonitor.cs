using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnboundLib;
using ExitGames.Client.Photon;

namespace WWC
{
    class PingMonitor : MonoBehaviourPunCallbacks
    {
        public Dictionary<int, bool> ConnectedPlayers = new Dictionary<int, bool>();
        public Dictionary<int, int> Ping = new Dictionary<int, int>();
        public Action<int, int> PingUpdateAction;

        private int pingUpdate = 0;

        private void Start()
        {
            if (!PhotonNetwork.OfflineMode && PhotonNetwork.CurrentRoom != null)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    ConnectedPlayers.Add(player.ActorNumber, true);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!PhotonNetwork.OfflineMode)
            {
                pingUpdate = pingUpdate + 1;
            }

            if (pingUpdate > 99)
            {
                pingUpdate = 0;
                RPCA_UpdatePings();
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnJoinedRoom()
        {
            ConnectedPlayers = new Dictionary<int, bool>();
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                ConnectedPlayers.Add(player.ActorNumber, true);
                Ping.Add(player.ActorNumber, 0);
            }

            this.ExecuteAfterSeconds(0.5f, () =>
            {
                this.photonView.RPC(nameof(RPCA_UpdatePings), RpcTarget.Others);
                RPCA_UpdatePings();
            });
        }

        public override void OnLeftRoom()
        {
            ConnectedPlayers.Clear();
            ConnectedPlayers = new Dictionary<int, bool>();
            Ping.Clear();
            Ping = new Dictionary<int, int>();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            ConnectedPlayers[otherPlayer.ActorNumber] = false;
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue("Ping", out var ping))
            {
                Ping[targetPlayer.ActorNumber] = (int)ping;

                if (PingUpdateAction != null)
                {
                    try
                    {
                        PingUpdateAction(targetPlayer.ActorNumber, (int)ping);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
        }

        public Player[] GetPlayersWithActorNumber(int actorNumber)
        {
            return PlayerManager.instance.players.Where((player) => player.data.view.OwnerActorNr == actorNumber).ToArray();
        }

        [PunRPC]
        private void RPCA_UpdatePings()
        {
            Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (customProperties.ContainsKey("Ping"))
            {
                customProperties["Ping"] = PhotonNetwork.GetPing();
            }
            else
            {
                customProperties.Add("Ping", PhotonNetwork.GetPing());
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
        }
    }
}
