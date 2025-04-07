using Il2CppNewtonsoft.Json;
using Il2CppScheduleOne.Money;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace RecyclerDumpsterMod
{
    public static class RDUtility
    {
        private static AudioSource cachedAudioSource;
                    
        public static Vector3 ComputeCenter(Vector3 pos, float extentY)
        {
            // Only calculate the center for the top coverage
            float centerX = pos.x;
            float centerY = pos.y + extentY;  // Add the vertical offset above the dumpster
            float centerZ = pos.z;

            // Return the computed center as a Vector3
            return new Vector3(centerX, centerY, centerZ);
        }

        public static (Vector3 min, Vector3 max) ComputeBoundsOffset(Vector3 pos, float extentX, float extentY, float extentZ, float offsetY)
        {
            // Calculate min and max points based on the passed extents and position
            Vector3 min = new Vector3(
                pos.x - extentX,
                pos.y - extentY,  // The bottom is at pos.y
                pos.z - extentZ
            );

            Vector3 max = new Vector3(
                pos.x + extentX,
                pos.y + extentY + offsetY,  // The top is at pos.y + extentY
                pos.z + extentZ
            );

            return (min, max);
        }

        public static (Vector3 min, Vector3 max) ComputeBoundsRounded(Vector3 pos, Vector3 extent, float offset= 0f )
        {
            // Calculate min and max points based on the passed extents and position, rounded to the nearest hundredth
            Vector3 min = new Vector3(
                Mathf.Round((pos.x - extent.x) * 100) / 100,
                Mathf.Round((pos.y - extent.y) * 100) / 100,  // Adjust pos.y for min
                Mathf.Round((pos.z - extent.z) * 100) / 100
            );
                        
            Vector3 max = new Vector3(
                Mathf.Round((pos.x + extent.x) * 100) / 100,
                Mathf.Round((pos.y + extent.y + offset)   * 100) / 100,  // Adjust pos.y for max
                Mathf.Round((pos.z + extent.z) * 100) / 100
            );

            return (min, max);
        }

        public static bool IsWithinRectangle(Vector3 itemPos, Vector3 min, Vector3 max)
        {
            return itemPos.x >= min.x && itemPos.x <= max.x &&
                   itemPos.y >= min.y && itemPos.y <= max.y &&
                   itemPos.z >= min.z && itemPos.z <= max.z;
        }

        public static bool IsWithinSphere(Vector3 point, Vector3 sphereCenter, float radius)
        {
            // Calculate the distance squared between the point and the center of the sphere
            float distanceSquared = (point.x - sphereCenter.x) * (point.x - sphereCenter.x) +
                                    (point.z - sphereCenter.z) * (point.z - sphereCenter.z);

            // Compare the distance squared with the radius squared (to avoid using sqrt)
            //MelonLogger.Msg($"Distance Squared: {distanceSquared}, Radius: {radius}, Radius Squared: {radius * radius}.");
            return distanceSquared <= radius * radius;
        }
        public static int GetValueFromRepo(string id)
        {
            var trashItem = RDRepository.TrashValues.Find(item => item.ID == id);
            return trashItem != null ? trashItem.Value : 0;
        }
        public static void PlayCashEjectSound()
        {
            if (cachedAudioSource == null)
            {
                // Find all AudioSources in the scene
                var audioSources = UnityEngine.Object.FindObjectsOfType<UnityEngine.AudioSource>();

                foreach (var audioSource in audioSources)
                {
                    // Check if the clip name contains 'cash' (case-insensitive)
                    if (audioSource.clip != null && audioSource.clip.name.ToLower().Contains("cash-register-kaching-sound-effect-125042"))
                    {
                        cachedAudioSource = audioSource;
                        break;
                    }
                }
            }

            if (cachedAudioSource != null)
            {
                // Play the sound using PlayOneShot
                cachedAudioSource.PlayOneShot(cachedAudioSource.clip); // Play the sound once
                //MelonLoader.MelonLogger.Msg("Cash Eject Sound Played!");
            }
            else
            {
                // If no matching audio source found
                MelonLoader.MelonLogger.Msg("No Cash Eject Sound Clip found!");
            }
        }
        public static bool IsValidJson(string json)
        {
            try
            {
                JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void LogUnknownItem(string trashID)
        {

            MelonLoader.MelonLogger.Msg($"Not yet maintained in RecyclerDumpster.json => {trashID} - modify/notify dev.");
        }
    }
}