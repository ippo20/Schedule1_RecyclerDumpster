//using System.Reflection;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;
using Il2CppScheduleOne.Money;
using System.Collections;
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
            if (this._localPlayer == null)
            {
                this._localPlayer = GameObject.Find("Player_Local");
            }

            if (this._localPlayer != null)
            {
                Vector3 pos = this._localPlayer.transform.position;
                RDProcessor.GenerateClickableDumpster(_recDumpCache, pos);

                if (Input.GetKeyDown(KeyCode.RightBracket)) // ']' key
                {
                    //TryRecycler(pos);
                    //DDD();
                }
            }
        }
        //private void DDD()
        //{
        //    MelonLogger.Msg($"DDD: Start");
        //    // Replace 'Dumpster' with your desired class
        //    var test = GameObject.FindObjectsOfType<Dumpster>();
        //    foreach (var dumpster in test)
        //    {
        //        Type type = dumpster.GetType();
        //        PropertyInfo[] properties = type.GetProperties();
        //        MethodInfo[] methods = type.GetMethods();

        //        MelonLogger.Msg($"Dumpster: {dumpster.name}");

        //        // List all properties
        //        MelonLogger.Msg("Properties:");
        //        foreach (PropertyInfo property in properties)
        //        {
        //            try
        //            {
        //                object value = property.GetValue(dumpster, null);
        //                MelonLogger.Msg($"  {property.Name}: {value}");
        //                base.LoggerInstance.Msg($"  {property.Name}: {value}");
        //            }
        //            catch
        //            {
        //                MelonLogger.Msg($"  {property.Name}: (unavailable)");
        //                base.LoggerInstance.Msg($"  {property.Name}: (unavailable)");
        //            }
        //        }
                

        //        // List all methods
        //        MelonLogger.Msg("Methods:");
        //        foreach (MethodInfo method in methods)
        //        {
        //            MelonLogger.Msg($"  {method.Name}");
        //        }
        //    }
        //}
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
            //if (_recDumpCache.Count <= 0)
            //{
            //    RDProcessor.CacheGameObject(_recDumpCache);
            //    MelonLoader.MelonLogger.Msg($"CacheGameObject [{_recDumpCache.Count}]");
            //}
        }

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
