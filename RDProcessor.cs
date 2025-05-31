using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UI;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.Money;
using MelonLoader;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using UnityEngine.UIElements;
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Interaction;
using System.Runtime.CompilerServices;
using Harmony;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using Il2CppFishNet.Demo.AdditiveScenes;
using Il2CppFluffyUnderware.Curvy.Generator;
using static UnityEngine.RemoteConfigSettingsHelper;
using UnityEngine.Rendering.PostProcessing;
using Il2CppSystem.Xml.Schema;
using static RecyclerDumpsterMod.RDRepository;
using static RecyclerDumpsterMod.RDUtility;
namespace RecyclerDumpsterMod
{
    public static class RDProcessor
    { 
        const float FOV_RAD = 7f; // FOV radius for interaction
        const string COLLIDER_NAME = "Collider (2)";
        const string TRASH_REM_NAME = "Trash remover";
        static readonly List<RecycableDumpster> RecycableDumpsters = RDRepository.RecycableDumpsters;

        static UnityAction unityAction = DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate        {
            //MelonLoader.Log($"Test E");
            DoRecycle();
        }));

        public static int CleanAllProperties()
        {
            int total = 0;
            List<string> ownedProperties = RDUtility.GetOwnedPropertyNames();

            foreach (var dumpster in RecycableDumpsters)
            {
                if (dumpster.ButtonPosition == Vector3.zero)
                    continue;

                if (!ownedProperties.Contains(dumpster.PropertyName))
                    continue;

                int value = ProcessTrashInArea(dumpster.Min, dumpster.Max);
                if (value > 0)
                    Log($"processed {dumpster.Name} : for : {value}");

                total += value;
                Log($"---------running cash total: {total}");
            }

            return total;
        }

        public static int CleanWithinVicinity()
        {

            List<string> ownedProperties = RDUtility.GetOwnedPropertyNames();
            Vector3 playerPos = PlayerSingleton<AppsCanvas>.Instance.transform.position;
            //RecycableDumpster dumpster = RecycableDumpsters.FirstOrDefault(rd => Vector3.Distance(playerPos, rd.Position) <= FOV_RAD);
            RecycableDumpster dumpster = RecycableDumpsters.FirstOrDefault(rd => 
                Vector3.Distance(playerPos, rd.Position) <= FOV_RAD && ownedProperties.Contains(rd.PropertyName));

            if (dumpster == null || dumpster.Name == null)
                return 0;

            int cash = ProcessTrashInArea(dumpster.Min, dumpster.Max);
            if (cash > 0)
                Log($"processed: paid : {cash}");

            return cash;
        }

        private static int ProcessTrashInArea(Vector3 min, Vector3 max)
        {
            TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();
            int cashValue = 0;
            int trashCnt = 0;

            foreach (TrashItem item in allTrashItems)
            {
                if (!IsWithinRectangle(item.transform.position, min, max))
                    continue;

                int c = 0;
                if (item.ID == "trashbag")
                {
                    ++trashCnt;
                    TrashBag bag = item.GetComponent<TrashBag>();
                    if (bag != null)
                    {
                        int ctot = 0;
                        int cCnt = 0;

                        foreach (TrashContent.Entry baggedItem in bag.Content.Entries)
                        {
                            c = GetValueFromRepo(baggedItem.TrashID);
                            if (c == 0)
                            {
                                LogUnknownItem(baggedItem.TrashID);
                                c = 1;
                            }

                            ctot += c * baggedItem.Quantity;
                            Log($"{item.ID}[{trashCnt}] Item[{++cCnt} {baggedItem.TrashID}] = {c} * {baggedItem.Quantity} = {ctot}");
                        }

                        cashValue += ctot;
                        Log($"{item.ID}[{trashCnt}] : subtotal {ctot}");
                        Log($"---------running cash subtotal: {cashValue}");
                    }
                    else
                    {
                        c = GetValueFromRepo(item.ID);
                        if (c == 0)
                        {
                            LogUnknownItem(item.ID);
                            c = 1;
                        }

                        cashValue += c;
                        Log($"Error Trashbag[{++trashCnt}] :          : {cashValue}");
                    }
                }
                else
                {
                    c = GetValueFromRepo(item.ID);
                    cashValue += c;
                    Log($"UnbaggedTrash[{++trashCnt}] Item [{item.ID}]:          : {cashValue}");
                }

                if (c <= 0)
                {
                    LogUnknownItem(item.ID);
                    c = 1;
                    cashValue += c;
                }

                item.DestroyTrash();
            }

            return cashValue;
        }

        public static void objAttachAction(GameObject gameObj, PositionAdjustment adj)
        {
            GameObject obj = GameObject.Instantiate(gameObj, gameObj.transform.position, gameObj.transform.rotation);
            obj.name = "RecyclerDumpsterInteractionObject";
            //MelonLoader.Log($"GenerateClickableDumpster => Collider (2) @{obj.transform.position}");
            if (obj.GetComponent<InteractableObject>() == null)
            {

                InteractableObject intObj = obj.AddComponent<InteractableObject>();
                var collider = intObj.GetComponent<Collider>();
                if (collider != null) collider.isTrigger = true;
                if (collider is BoxCollider box)
                {
                    box.size *= 5f; // or modify specific axis: box.size = new Vector3(5, 1, 1);
                }
                Vector3 position = intObj.GetComponent<InteractableObject>().transform.position;
                intObj.GetComponent<InteractableObject>().transform.position = AdjustPosition(position, adj);
                intObj.GetComponent<InteractableObject>().SetMessage("Sell Trash");
                intObj.GetComponent<InteractableObject>().SetInteractionType(0);
                intObj.GetComponent<InteractableObject>().MaxInteractionRange = 5;
                intObj.GetComponent<InteractableObject>().RequiresUniqueClick = false;
                intObj.GetComponent<InteractableObject>().Priority = 1;
                intObj.GetComponent<InteractableObject>().LimitInteractionAngle = false;
                intObj.GetComponent<InteractableObject>().AngleLimit = 0f;
                intObj.onInteractStart.AddListener(unityAction);
            }
            
        }

        public static void DoRecycle(bool all = false)
        {
            int addCash = all ? CleanAllProperties() : CleanWithinVicinity();  // Choose method based on 'all'

            if (addCash > 0)
            {
                Core.ChangeCashBalance(addCash);
                Log($"Trash2Cash: {addCash}");
                // RDUtility.PlayCashEjectSound();  // Optional
            }
        }
        public static void PatchDumpsters()
        {
            Log("PatchDumpsters start");

            Vector3 zeroPos = Vector3.zero;
            List<RecycableDumpster> localDumpsters = new List<RecycableDumpster>(RecycableDumpsters);

            for (int i = localDumpsters.Count - 1; i >= 0; i--)
            {
                var dumpster = localDumpsters[i];
                if (dumpster.ButtonPosition == zeroPos)
                {
                    localDumpsters.RemoveAt(i);
                    continue;
                }

                Collider[] nearby = Physics.OverlapSphere(dumpster.ButtonPosition, 1f);
                GameObject trashObj = null;
                GameObject colliderObj = null;

                foreach (var col in nearby)
                {
                    var obj = col.gameObject;

                    if (trashObj == null && obj.name == TRASH_REM_NAME)
                        trashObj = obj;

                    if (colliderObj == null && obj.name == COLLIDER_NAME)
                        colliderObj = obj;

                    if (trashObj != null && colliderObj != null)
                        break;
                }

                if (RDRepository._CONFIG.DisableAutoTrashRemoval)
                {
                    if (trashObj != null)
                    {
                        var trashRemover = trashObj.GetComponent<Il2CppScheduleOne.Trash.TrashRemovalVolume>();
                        if (trashRemover != null)
                        {
                            trashRemover.RemovalChance = 0f;
                            Log($"Set RemovalChance = 0 for: {trashObj.name} of {dumpster.Name}");
                        }
                    }
                }

                if (colliderObj != null)
                {
                    colliderObj.tag = $"button_{dumpster.ID}";
                    objAttachAction(colliderObj, dumpster.ButtonPositionAdjustment);
                }

                if (trashObj != null || colliderObj != null)
                {
                    localDumpsters.RemoveAt(i);
                }
            }
        }

    }
}