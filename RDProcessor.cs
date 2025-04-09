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
        static readonly List<RecycableDumpster> RecycableDumpsters = RDRepository.RecycableDumpsters;

        static UnityAction unityAction = DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate        {
            //MelonLoader.MelonLogger.Msg($"Test E");
            DoRecycle();
        }));

        public static int CleanWithinVicinity()
        {
            Vector3 playerPos = PlayerSingleton<AppsCanvas>.Instance.transform.position;
            RecycableDumpster dumpster = new();
            foreach (RecycableDumpster rd in RecycableDumpsters)
            {
                if (Vector3.Distance(playerPos, rd.Position) <= FOV_RAD)
                {
                    dumpster = rd;
                    //MelonLogger.Msg($"Found dumpster: {rd.Name} at {rd.Position}.");
                    break;
                }
            }
            if (dumpster.Name == null)
            {
                //MelonLoader.MelonLogger.Msg($"No dumpster found within vicinity.");
                return 0;
            }
            TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();
           
            int cashValue = 0;
            foreach (TrashItem item in allTrashItems)
            {
                if (!IsWithinRectangle(item.transform.position, dumpster.Min, dumpster.Max))
                    continue;

                //MelonLogger.Msg($"Processing: {item.ID} at {item.transform.position}");
                int c = 0;
                if (item.ID == "trashbag")
                {
                    TrashBag bag = item.GetComponent<TrashBag>();
                    if (bag != null)
                    {
                        //bagsProcessed++;
                        int ctot = 0;
                        foreach (TrashContent.Entry baggedItem in bag.Content.Entries)
                        {
                            c = 0;
                            c = GetValueFromRepo(baggedItem.TrashID);
                            if (c == 0)
                            {
                                LogUnknownItem(baggedItem.TrashID);
                                c = 1; //give consolation;
                            }
                            ctot += c;
                        }
                        c = ctot;
                        cashValue += c;
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
                    }
                }
                else
                {
                    c = GetValueFromRepo(item.ID);
                    cashValue += c;
                }

                if (c <= 0)
                {
                    //MelonLoader.MelonLogger.Msg($"Item {item.ID} is not yet configured in JSON... report to dev... ignoring for now.");
                    LogUnknownItem(item.ID);
                    c = 1; //give consolation;
                    cashValue += c;
                }

                item.DestroyTrash();

            }

            if (cashValue > 0)
            {
                return cashValue;
            }

            return 0;
        }
        public static void objAttachAction(GameObject obj, PositionAdjustment adj)
        {
            //MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster => Collider (2) @{obj.transform.position}");
            if (obj.GetComponent<InteractableObject>() == null)
            {
                InteractableObject intObj = obj.AddComponent<InteractableObject>();
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

        public static void DoRecycle()
        {
            int addCash = CleanWithinVicinity();
            if (addCash > 0)
            {
                Core.ChangeCashBalance(addCash);
                MelonLogger.Msg($"Trash2Cash: {addCash}");
                //RDUtility.PlayCashEjectSound();
            }
        }

        public static void GenerateClickableDumpster()
        {
            //MelonLogger.Msg($"GenerateClickableDumpster start");            

            Vector3 zeroPos = new Vector3(0, 0, 0);
            List<RecycableDumpster> localDumpsters = new List<RecycableDumpster>(RecycableDumpsters);
            for (int i = localDumpsters.Count - 1; i >= 0; i--)
            {
                RecycableDumpster localDumpster = localDumpsters[i];
                //MelonLogger.Msg($"GenerateClickableDumpster => localDumpsters[{i}] {localDumpster.Name} at {localDumpster.Position}");
                if (localDumpster.ButtonPosition == zeroPos)
                {
                    localDumpsters.RemoveAt(i);
                    //MelonLogger.Msg($"GenerateClickableDumpster removing localdump");
                }
            }

            GameObject btnObj = GameObject.Find("Button");
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (obj.name == "Collider (2)")
                {
                    for (int i = 0; i < localDumpsters.Count; i++)
                    {
                        RecycableDumpster localDumpster = localDumpsters[i];
                        float dist = Vector3.Distance(obj.transform.position, localDumpster.ButtonPosition);                            
                        if (dist < 1f)
                        {
                            obj.tag = $"button_{localDumpster.ID}";                            ;
                            localDumpsters.RemoveAt(i);
                            objAttachAction(obj, localDumpster.ButtonPositionAdjustment);
                            break;
                        }
                    }
                }
                if (localDumpsters.Count == 0)
                {
                    break;
                }
            }

        }
    }
}