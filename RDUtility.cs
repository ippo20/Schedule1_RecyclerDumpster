using Il2CppScheduleOne.Money;
using System.Collections;
using UnityEngine;

namespace RecyclerDumpsterMod
{
    public static class RDUtility
    {
        private static AudioSource cachedAudioSource;
                    
        private static readonly (float extentX, float extentY, float extentZ) extents = RDRepository.GetExtents();

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

        public static (Vector3 min, Vector3 max) ComputeBoundsRounded(Vector3 pos, float extentX, float extentY, float extentZ)
        {
            // Calculate min and max points based on the passed extents and position, rounded to the nearest hundredth
            Vector3 min = new Vector3(
                Mathf.Round((pos.x - extentX) * 100) / 100,
                Mathf.Round((pos.y - extentY) * 100) / 100,  // Adjust pos.y for min
                Mathf.Round((pos.z - extentZ) * 100) / 100
            );
            
            Vector3 max = new Vector3(
                Mathf.Round((pos.x + extentX) * 100) / 100,
                Mathf.Round((pos.y + extentY) * 100) / 100,  // Adjust pos.y for max
                Mathf.Round((pos.z + extentZ) * 100) / 100
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
            return distanceSquared <= radius * radius;
        }
        public static int GetValueFromRepo(string id)
        {
            int val = 0;
            if (RDRepository.TrashValues.TryGetValue(id, out RDRepository.TrashValue trashItem))
            {
                val = trashItem.Value;
            }
           
            MelonLoader.MelonLogger.Msg($"Item ID {id} = {val}.");
            
            return val;
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

    }
}