using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;
using Il2CppScheduleOne.Money;
using System.Collections;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Audio;

[assembly: MelonInfo(typeof(RecyclerDumpster.Core), "RecyclerDumpster", "1.0.0", "ippo", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace RecyclerDumpster
{
    [HarmonyPatch(typeof(TrashItem))]
    [HarmonyPatch("AddTrash")]
    [HarmonyPatch(new Type[] { typeof(TrashItem) })]

    public class Core : MelonMod
    {
        private MoneyManager moneyManager;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("RecyclerDumpster initialized.");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.RightBracket)) // ']' key
            {
                TryRecycler();
            }
        }

        private void TryRecycler()
        {
            GameObject player = GameObject.Find("Player_Local");
            Vector3 pos = player.transform.position;
            MelonLogger.Msg($"Player Position: {player.transform.position}");
            CleanWithinVicinity(pos);
        }

        private void CleanWithinVicinity(Vector3 pos)
        {
            Vector3 docksDumpster = new Vector3(-69.39f, -0.9f, -63.50f); // center of dumpster
            Vector3 docksPosition = new Vector3(-74.0796f, -1.163f, -64.8039f); // where the player stands near the docks

            float playerVicinityRadius = 7f;
            float length = 3.66f;
            float width = 1.15f;
            float minY = -2.4f;
            float maxY = 0.89f;

            if (Vector3.Distance(pos, docksPosition) <= playerVicinityRadius)
            {
                MelonLogger.Msg("Player is near the docks.");
                TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();

                int cashValue = 0;
                foreach (TrashItem item in allTrashItems)
                {
                    if (!IsWithinRectangle(item.transform.position, docksDumpster, length, width, minY, maxY)) continue;
                    MelonLogger.Msg($"Processing: {item.ID} at {item.transform.position}");

                    if (item.ID == "trashbag")
                    {
                        TrashBag bag = item.GetComponent<TrashBag>();
                        if (bag != null)
                        {
                            foreach (TrashContent.Entry baggedItem in bag.Content.Entries)
                            {
                                int c = GetValueFromRepo(baggedItem.TrashID);
                                cashValue += c;
                                MelonLogger.Msg($"baggedTrash: {baggedItem.TrashID} price: {c}, new cash val:{cashValue}");
                            }
                        }
                        else
                        {
                            int c = GetValueFromRepo(item.ID);
                            cashValue += c;
                            MelonLogger.Msg($"trashNotBag: {item.ID} price: {c}, new cash val:{cashValue}");
                        }
                    }
                    else
                    {
                        int c = GetValueFromRepo(item.ID);
                        cashValue += c;
                        MelonLogger.Msg($"trash: {item.ID} price: {c}, new cash val:{cashValue}");
                    }
                }

                MelonLogger.Msg($"Cash grabbed total: {cashValue}");
                if (cashValue > 0)
                {
                    this.moneyManager.ChangeCashBalance(cashValue, true, false);
                    PlayCashEjectSound();
                }
            }
        }

        private bool IsWithinRectangle(Vector3 itemPos, Vector3 center, float length, float width, float minY, float maxY)
        {
            float minX = center.x - length / 2f;
            float maxX = center.x + length / 2f;
            float minZ = center.z - width / 2f;
            float maxZ = center.z + width / 2f;

            return itemPos.x >= minX && itemPos.x <= maxX &&
                   itemPos.y >= minY && itemPos.y <= maxY &&
                   itemPos.z >= minZ && itemPos.z <= maxZ;
        }

        private int GetValueFromRepo(string id)
        {
            if (TrashValueRepository.TrashValues.TryGetValue(id, out TrashValue trashItem))
            {
                return trashItem.Value;
            }
            return 0;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonCoroutines.Start(this.WaitForMoneyManager());
        }

        private IEnumerator WaitForMoneyManager()
        {
            while (this.moneyManager == null)
            {
                GameObject moneyManagerObject = GameObject.Find("Managers/@Money");
                if (moneyManagerObject != null)
                {
                    this.moneyManager = moneyManagerObject.GetComponent<MoneyManager>();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void PlayCashEjectSound()
        {
            // Find all AudioSources in the scene
            var audioSources = UnityEngine.Object.FindObjectsOfType<UnityEngine.AudioSource>();

            foreach (var audioSource in audioSources)
            {
                // Check if the clip name contains 'cash' (case-insensitive)
                if (audioSource.clip != null && audioSource.clip.name.ToLower().Contains("cash-register-kaching-sound-effect-125042"))
                {
                    // Play the sound using PlayOneShot
                    audioSource.PlayOneShot(audioSource.clip); // Play the sound once
                    MelonLoader.MelonLogger.Msg("Cash Eject Sound Played!");
                    return; // Stop after playing the sound once
                }
            }

            // If no matching audio source found
            MelonLoader.MelonLogger.Msg("No Cash Eject Sound Clip found!");
        }
    }
}
