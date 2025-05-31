//using System.Reflection;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.Money;
using System.Collections;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using Il2CppToolBuddy.ThirdParty.VectorGraphics;
using Il2CppFluffyUnderware.Curvy.Generator;
using Il2CppInterop.Runtime;
using static UnityEngine.RemoteConfigSettingsHelper;
using UnityEngine.Events;
using static RecyclerDumpsterMod.RDUtility;
using Il2CppScheduleOne.Interaction;

using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.ItemFramework;


[assembly: MelonInfo(typeof(RecyclerDumpsterMod.Core), "RecyclerDumpster", "3.1.0", "ippo", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace RecyclerDumpsterMod
{
    public class Core : MelonMod
    {
#if DEBUG
        public static readonly bool DEBUG = true;
#else
        public static readonly bool DEBUG = false;
#endif
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
            Log("RecyclerDumpster initialized");
            
                string s = "Credits to the following: ";
            foreach (CreditsTo thx in _thx)
            {
                s+=$"{thx.ModName} - {thx.Author} | ";
            }
            Log(s, true);
        }
       
        public override void OnUpdate()
        {
            // Check if the HotKey is pressed
            if (Input.GetKeyDown(RDRepository._CONFIG.HotKey))
            {
                // Check if the modifier key is held down
                KeyCode modifierKey = RDRepository._CONFIG.ModifierKey; // Retrieve the configured modifier key
                bool isModifierKeyPressed = Input.GetKey(modifierKey);  // Check if the modifier key is pressed

                // If modifier key is set and pressed along with HotKey, it's an "All" action
                bool isAll = (modifierKey != KeyCode.None) && isModifierKeyPressed;

                // Perform the action, passing isAll to determine whether it's a normal or "All" action
                RDProcessor.DoRecycle(isAll);
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

            Log($"Player loaded in Scene:{sceneName}");
            RDProcessor.PatchDumpsters();

            var player = PlayerSingleton<AppsCanvas>.Instance;

            GameObject moneyManagerObject = GameObject.Find("Managers/@Money");            
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
        private static void Postfix(TrashItem __instance)
        {
            TrashRemovalVolume trashRemovalVolume = __instance.GetComponent<TrashRemovalVolume>();
            Log($"{trashRemovalVolume.RemovalChance}");

        }

        public static void ChangeCashBalance(int amount)
        {
            if (_moneyManager != null)
            {
                _moneyManager.ChangeCashBalance(amount, true, true);
            }
            else
            {
                LogError("MoneyManager is not initialized.");
            }
        }

        private class CreditsTo()
        {
            public string ModName { get; set; }
            public string Author { get; set; }
        }

    }
}
