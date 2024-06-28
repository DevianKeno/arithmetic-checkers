using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aritche.UI;

using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Aritche.EOS
{
    public class EOSFriendsUI : MonoBehaviour, IEOSOnConnectLogin
    {
        public bool IsSearching;
        List<GameObject> friendEntries = new();

        EOSFriendsManager friendsManager;
        [SerializeField] GameObject friendEntriesContainer;
        [SerializeField] GameObject friendEntryPrefab;

        internal void Initialize()
        {
            friendsManager ??= EOSManager.Instance.GetOrCreateManager<EOSFriendsManager>();
            SearchForFriends();
        }

        void Update()
        {
            if (IsSearching)
            {
                if (friendsManager.GetCachedSearchResults(out Dictionary<EpicAccountId, FriendData> searchResults))
                {
                    RefreshUI(searchResults.Values);
                    IsSearching = false;
                }
            }
        }

        public void SearchForFriends()
        {
            IsSearching = true;
            StartCoroutine(nameof(SearchForFriendsCoroutine));
        }

        IEnumerator SearchForFriendsCoroutine()
        {
            yield return new WaitForSeconds(5);
            IsSearching = false;
        }

        void RefreshUI(Dictionary<EpicAccountId, FriendData>.ValueCollection values)
        {
            foreach (GameObject go in friendEntries)
            {
                Destroy(go);
            }
            friendEntries.Clear();

            foreach (FriendData friendData in values)
            {
                var go = Instantiate(friendEntryPrefab, friendEntriesContainer.transform);
                FriendEntryUI friendEntryUI = go.GetComponent<FriendEntryUI>();

                friendEntryUI.DisplayName = friendData.Name;
            }
        }

        public void OnConnectLogin(LoginCallbackInfo loginCallbackInfo)
        {
            Initialize();
        }
    }
}