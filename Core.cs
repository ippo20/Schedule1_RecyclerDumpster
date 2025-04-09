//using System.Reflection;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;
using Il2CppScheduleOne.Money;
using System.Collections;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using Il2CppToolBuddy.ThirdParty.VectorGraphics;
using Il2CppFluffyUnderware.Curvy.Generator;
using Il2CppInterop.Runtime;
using static UnityEngine.RemoteConfigSettingsHelper;
using UnityEngine.Events;

using Il2CppScheduleOne.Interaction;

[assembly: MelonInfo(typeof(RecyclerDumpsterMod.Core), "RecyclerDumpster", "1.0.0", "ippo", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace RecyclerDumpsterMod
{
    public class Core : MelonMod
    {

        private static MoneyManager _moneyManager;

        private GameObject _localPlayer;
        private List<CreditsTo> _thx = new List<CreditsTo>()
        {
            new CreditsTo() { ModName = "TrashDestroyer", Author = "heimy2000" },
            new CreditsTo() { ModName = "Golden Touch", Author = "Drakobine" },
            new CreditsTo() { ModName = "Tips and guide", Author = "Schdule One Modding Server" },
        };

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("RecyclerDumpster initialized");
            
                string s = "Credits to the following: ";
            foreach (CreditsTo thx in _thx)
            {
                s+=$"{thx.ModName} - {thx.Author} | ";
            }
            MelonLogger.Msg(s);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(RDRepository._CONFIG.HotKey))
            {
                if (this._localPlayer == null)
                {
                    this._localPlayer = GameObject.Find("Player_Local");
                }

                if (this._localPlayer != null)
                {
                    RDProcessor.DoRecycle();
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {            

            MelonCoroutines.Start(this.WaitForMoneyManager());

            MelonCoroutines.Start(this.WaitForPlayer(sceneName));
           
        }
        private IEnumerator WaitForPlayer(string sceneName)
        {
            if (sceneName != "Main")
                yield break;

            while (PlayerSingleton<AppsCanvas>.Instance == null)
                yield return null;

            MelonLogger.Msg($"Player loaded in Scene:{sceneName}");
            RDProcessor.GenerateClickableDumpster();            

        }
        private IEnumerator WaitForMoneyManager()
        {
            while (_moneyManager == null)
            {
                GameObject moneyManagerObject = GameObject.Find("Managers/@Money");
                if (moneyManagerObject != null)
                {
                    _moneyManager = moneyManagerObject.GetComponent<MoneyManager>();
                }
                yield return new WaitForSeconds(1f);
            }
        }
        public static void ChangeCashBalance(int amount)
        {
            if (_moneyManager != null)
            {
                _moneyManager.ChangeCashBalance(amount, true, true);
            }
            else
            {
                MelonLogger.Error("MoneyManager is not initialized.");
            }
        }

        private class CreditsTo()
        {
            public string ModName { get; set; }
            public string Author { get; set; }
        }

    }
}
