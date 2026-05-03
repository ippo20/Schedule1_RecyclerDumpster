using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.Money;
using System.Collections;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.ItemFramework;
using System.Collections.Generic;
using static RecyclerDumpsterMod.RDUtility;
//using Application.version;

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
        private const string _supportedGameVersion = "v0.4.5f1";
        private static MoneyManager _moneyManager;

        private readonly List<CreditsTo> _thx = new()
        {
            new CreditsTo() { ModName = "TrashDestroyer", Author = "heimy2000" },
            new CreditsTo() { ModName = "Golden Touch", Author = "Drakobine" },
            new CreditsTo() { ModName = "Tips and guide", Author = "Schdule One Modding Server" },
        };

        public override void OnInitializeMelon()
        {
            CheckGameVersion();
            Log("RecyclerDumpster initialized", true);

            string s = "Credits to the following: ";
            foreach (CreditsTo thx in _thx)
                s += $"{thx.ModName} - {thx.Author} | ";

            Log(s, true);
        }
        private void CheckGameVersion()
        {
            string current = Application.version;

            if (!RDUtility.TryParseVersion(current, out Version currentV) ||
                !RDUtility.TryParseVersion(_supportedGameVersion, out Version supportedV))
            {
                Log($"Could not parse version. Current={current}, Supported={_supportedGameVersion}", true);
                return;
            }

            int majorDiff = currentV.Major - supportedV.Major;
            int minorDiff = currentV.Minor - supportedV.Minor;

            // allow same major, and small minor drift
            if (currentV.Major != supportedV.Major || minorDiff > 1)
            {
                Log("======================================",true);
                Log("RecyclerDumpster may be outdated", true);
                Log($"Last Build Tested: {_supportedGameVersion}",true);
                Log($"Current Build:   {current}", true);
                Log("Expect issues until mod is updated.", true);
                Log("You may contact the dev thru discord or kofi.", true);
                Log("======================================", true);
            }
            else
            {
                Log("=======---RecyclerDumpster---=========", true);
                Log($"Game version compatible ({current})", true);
                Log("======================================", true);
            }
        } 

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(RDRepository._CONFIG.HotKey))
            {
                KeyCode modifierKey = RDRepository._CONFIG.ModifierKey;
                bool isModifierKeyPressed = Input.GetKey(modifierKey);
                bool isAll = (modifierKey != KeyCode.None) && isModifierKeyPressed;

                RDProcessor.DoRecycle(isAll);
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Main")
                return;

            MelonCoroutines.Start(WaitForGameReady(sceneName));
        }

        private IEnumerator WaitForGameReady(string sceneName)
        {
            while (MoneyManager.Instance == null)
                yield return null;

            _moneyManager = MoneyManager.Instance;
            Log("MoneyManager initialized");

            RDProcessor.PatchDumpsters();
            Log($"Player loaded in Scene:{sceneName}");
        }

        private static void Postfix(TrashItem __instance)
        {
            TrashRemovalVolume trashRemovalVolume = __instance.GetComponent<TrashRemovalVolume>();
            Log($"{trashRemovalVolume.RemovalChance}");
        }

        public static void ChangeCashBalance(int amount)
        {
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.ChangeCashBalance(amount, true, true);
            }
            else
            {
                LogError("MoneyManager is not initialized.");
            }
        }

        private class CreditsTo
        {
            public string ModName { get; set; }
            public string Author { get; set; }
        }
    }
}