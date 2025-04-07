//using System.Reflection;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;
using Il2CppScheduleOne.Money;
using System.Collections;
using static RecyclerDumpsterMod.RDRepository;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using Il2CppToolBuddy.ThirdParty.VectorGraphics;
////using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;
//using UnityEngine.Playables;
//using Il2CppScheduleOne.ObjectScripts;

//using Il2CppSystem.Collections.Generic;

[assembly: MelonInfo(typeof(RecyclerDumpsterMod.Core), "RecyclerDumpster", "1.0.0", "ippo", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace RecyclerDumpsterMod
{
    [HarmonyPatch(typeof(TrashItem))]
    [HarmonyPatch("AddTrash")]
    [HarmonyPatch(new Type[] { typeof(TrashItem) })]
    public class Core : MelonMod
    {
        private MoneyManager _moneyManager;
        private List<GameObject> _recDumpCache = new List<GameObject>();
        private GameObject _localPlayer;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("RecyclerDumpster initialized.");
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
                    Vector3 pos = this._localPlayer.transform.position;
                    TryRecycler(pos);
                }
            }
        }
        private void TryRecycler(Vector3 pos)
        {
            //MelonLogger.Msg($"Player Position: {player.transform.position}");
            int addCash = RDProcessor.CleanWithinVicinity(pos);
            if (addCash > 0)
            {
                _moneyManager.ChangeCashBalance(addCash, true, false);
                RDUtility.PlayCashEjectSound();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {            

            MelonCoroutines.Start(this.WaitForMoneyManager());

            //MelonCoroutines.Start(this.WaitForPlayer(sceneName));
           
        }
        //private IEnumerator WaitForPlayer(string sceneName)
        //{
        //    if (sceneName != "Main")
        //        yield break;

        //    while (PlayerSingleton<AppsCanvas>.Instance == null)
        //    {
        //        yield return null;
        //    }
        //    MelonLogger.Msg($"Player loaded in Scene:{sceneName}");
                     

        //    while (PlayerSingleton<AppsCanvas>.Instance == null)
        //        yield return null;

        //    AppsCanvas player = PlayerSingleton<AppsCanvas>.Instance;

        //    Vector3 pos = player.transform.position;
        //    MelonLogger.Msg($"{pos} == player pos");
        //    if (PlayerSingleton<AppsCanvas>.Instance != null)
        //    {
        //        RDProcessor.GenerateClickableDumpster(_recDumpCache, pos);
        //    }

        //}
        private IEnumerator WaitForMoneyManager()
        {
            while (this._moneyManager == null)
            {
                GameObject moneyManagerObject = GameObject.Find("Managers/@Money");
                if (moneyManagerObject != null)
                {
                    this._moneyManager = moneyManagerObject.GetComponent<MoneyManager>();
                }
                yield return new WaitForSeconds(1f);
            }
        }
        

    }
}
