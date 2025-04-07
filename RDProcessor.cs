using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
namespace RecyclerDumpsterMod
{
    public static class RDProcessor
    {
        //const float BUFFER_TOP = 1.3f;  // Vertical coverage only for the top
        //const float Y_OFFSET = 0.85f * BUFFER_TOP;  // Vertical offset above the dumpster
        //const float EXTENT_X = 0.683f;  // Horizontal X extent
        //const float EXTENT_Y = 0.5f;  // Vertical Y extent
        //const float EXTENT_Z = 0.683f;  // Horizontal Z extent
        const float FOV_RAD = 7f; // FOV radius for interaction
        static readonly List<RDRepository.RecycableDumpster> RecycableDumpsters = RDRepository.RecycableDumpsters; 


        public static int CleanWithinVicinity(Vector3 playerPos)
        {
            //MelonLoader.MelonLogger.Msg($"CleanWithinVicinity");
            //Vector3 docksDumpster = new Vector3(-69.79865f, -2.1692f, -63.61f);
            //Vector3 docksPosition = new(-74.0796f, -1.163f, -64.8039f); // where the player stands near the docks
                        
            Vector3 dumpsterPos = new Vector3(0f, 0f, 0f);
            RDRepository.RecycableDumpster dumpster = new();
            foreach (RDRepository.RecycableDumpster rd in RecycableDumpsters)
            {
                if (Vector3.Distance(playerPos, rd.Position) <= FOV_RAD)
                {
                    dumpster = rd;
                    //MelonLogger.Msg($"Found dumpster: {rd.Name} at {rd.Position}.");
                    break;
                }
            }
            if(dumpster.Name== null)
            {
                MelonLoader.MelonLogger.Msg($"No dumpster found within vicinity.");
                return 0;
            }
            //Vector3 dumpsterPos = new(-69.4872f, -1.614f, -63.5005f);
            //Vector3 center = RDUtility.ComputeCenter(dumpsterPos, Y_OFFSET);
            //(Vector3 min, Vector3 max) = RDUtility.ComputeBoundsOffset(dumpsterPos, EXTENT_X, EXTENT_Y, EXTENT_Z, Y_OFFSET);

            //MelonLogger.Msg($"Player is near the dumpsterPos.{dumpster.Position}");
            TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();
            //int normalItemsProcessed = 0;
            //int bagsProcessed = 0;
            //int itemsInBagsProcessed = 0;
            //int bagsNotBagsProcessed = 0;


            int cashValue = 0;
            foreach (TrashItem item in allTrashItems)
            {

                //if (!RDUtility.IsWithinRectangle(item.transform.position, docksDumpster, length, width, minY, maxY))
                if (!RDUtility.IsWithinRectangle(item.transform.position, dumpster.Min, dumpster.Max))
                {
                    //Vector3 itemPos = item.lastPosition;
                    //float radiusC = 2.5f;

                    //if (!RDUtility.IsWithinSphere(itemPos, dumpster.Position, radiusC)) continue;
                    //MelonLogger.Msg($"----------------------");
                    //MelonLogger.Msg($"Not Detected: {item.ID} at {item.transform.position}");
                    //MelonLogger.Msg($"dumpsterPos: {dumpster.Position}");
                    //MelonLogger.Msg($"bound: {dumpster.Min} {dumpster.Max}");
                    //MelonLogger.Msg($"----------------------");
                    continue;
                }

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
                            c = RDUtility.GetValueFromRepo(baggedItem.TrashID);
                            if (c == 0)
                            {
                                RDUtility.LogUnknownItem(baggedItem.TrashID);
                                c = 1; //give consolation;
                            }
                            ctot += c;

                            //itemsInBagsProcessed++;
                            //MelonLogger.Msg($"baggedTrash: {baggedItem.TrashID} price: {c}, new cash val:{cashValue}");
                        }
                        c = ctot;
                        cashValue += c;
                    }
                    else
                    {
                        c = RDUtility.GetValueFromRepo(item.ID);
                        if (c == 0)
                        {
                            RDUtility.LogUnknownItem(item.ID);
                            c = 1; //give consolation;
                        }
                        cashValue += c;
                        //bagsNotBagsProcessed++;
                        //MelonLogger.Msg($"trashNotBag: {item.ID} price: {c}, new cash val:{cashValue}");
                    }
                }
                else
                {
                    c = RDUtility.GetValueFromRepo(item.ID);
                    cashValue += c;
                    //normalItemsProcessed++;
                    //MelonLogger.Msg($"trash: {item.ID} price: {c}, new cash val:{cashValue}");
                }
                //MelonLogger.Msg($"Normal items processed: {normalItemsProcessed}");
                //MelonLogger.Msg($"Bags processed: {bagsProcessed}");
                //MelonLogger.Msg($"Items in bags processed: {itemsInBagsProcessed}");
                //MelonLogger.Msg($"Bags not bags processed: {bagsNotBagsProcessed}");
                //MelonLogger.Msg($"Cash Total: {cashValue}");
                if (c<= 0)
                {
                    MelonLoader.MelonLogger.Msg($"Item {item.ID} is not yet configured in code... ignoring...report to dev.");
                    continue;
                }
                else
                { 
                    GameObject.Destroy(item.gameObject); 
                }
                
            }

            MelonLogger.Msg($"Recycled Cash Total: {cashValue}");
            if (cashValue > 0)
            {
                return cashValue;
            }
            
            return 0;
        }

        /*
        public static void CacheGameObject(List<GameObject> __recDumpCache)
        {

            MelonLoader.MelonLogger.Msg($"CacheGameObject");
            __recDumpCache.Clear();
            const string tag = "CanRecycle";
            List<RDRepository.RecycableDumpster> localDumpsters = new List<RDRepository.RecycableDumpster>(RDRepository.RecycableDumpsters);

            int k = 0;
            
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (obj.name == "Bin")
                {
                    for (int i = 0; i < localDumpsters.Count; i++)
                    {
                        //MelonLoader.MelonLogger.Msg($"CacheGameObject|obj[{j}]|for[{i}] {obj.CompareTag(tag)} => {obj.tag}");
                        RDRepository.RecycableDumpster localDumpster = localDumpsters[i];

                        if (RDUtility.IsWithinRectangle(obj.transform.position, localDumpster.Min, localDumpster.Max))
                        {
                            MelonLoader.MelonLogger.Msg($"{obj.name} -- {++k} were Processed.");
                            obj.tag = tag;
                            __recDumpCache.Add(obj); // Add to the list
                            localDumpsters.RemoveAt(i); // Remove the accessed dumpster
                            break; // Exit the loop once we find a matching dumpster
                        }
                        //else
                        //{
                        //    MelonLoader.MelonLogger.Msg($"------------------NOT PROC-----------------------");
                        //    MelonLoader.MelonLogger.Msg($"{obj.name} -- {obj.transform.position}.");
                        //    MelonLoader.MelonLogger.Msg($"{localDumpster.Position} -- {localDumpster.Min} - {localDumpster.Max} were NOT ! ! ! Processed.");

                        //}
                    }
                }
            }
        }

        public static void GenerateClickableDumpster(List<GameObject> __recDumpCache, Vector3 playerPos)
        {
            try
            {
                //MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster");
                const string strInteraction = "Process Trash";
                const string tag = "CanDoRecycle";
                if (__recDumpCache.Count <= 0)
                {
                    CacheGameObject(__recDumpCache);
                    MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster => CacheGameObject [{__recDumpCache.Count}]");
                }
                int i = 0;
                foreach (GameObject gameObject in __recDumpCache)
                {
                    i++;

                    //MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster => foreach [{i}]");
                    //if (Vector3.Distance(playerPos, gameObject.transform.position) <= FOV_RAD)
                    //{
                        if (gameObject.GetComponent<InteractableObject>() == null)
                        {

                            InteractableObject intObj = gameObject.AddComponent<InteractableObject>();
                            intObj.SetMessage(strInteraction);
                            intObj.tag = tag;
                            intObj.SetInteractionType(0);
                            intObj.MaxInteractionRange = FOV_RAD;// / 2f;
                            intObj.RequiresUniqueClick = false;
                            intObj.Priority = 1;
                            intObj.LimitInteractionAngle = false;
                            intObj.AngleLimit = 0f;
                            UnityAction unityAction = DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate
                            {
                                MelonLoader.MelonLogger.Msg($"Test E");
                                CleanWithinVicinity(playerPos);//possible to gameObject instead???
                            }));
                            intObj.onInteractStart.AddListener(unityAction);

                            MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster for {gameObject.transform.position}");
                        }
                    //}
                    //else
                    //{
                    //    InteractableObject component = gameObject.GetComponent<InteractableObject>();
                    //    if (component != null)
                    //    {
                    //        MelonLoader.MelonLogger.Msg($"GenerateClickableDumpster => Destroyed");
                    //        UnityEngine.Object.Destroy(component);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
                defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
                MelonLogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
            }
        }
        */
    }
}