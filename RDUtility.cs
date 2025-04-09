using Il2CppNewtonsoft.Json;
using Il2CppScheduleOne.Money;
using MelonLoader;
using System.Collections;
using UnityEngine;
using static RecyclerDumpsterMod.RDRepository;

namespace RecyclerDumpsterMod
{
    public static class RDUtility
    {                           
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
      
        public static int GetValueFromRepo(string id)
        {
            var trashItem = RDRepository.TrashValues.Find(item => item.ID == id);
            return trashItem != null ? trashItem.Value : 0;
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

        public static Vector3 AdjustPosition(Vector3 currentPosition, PositionAdjustment adjustment)
        {
            if (adjustment.Forward > 0f)
            {
                currentPosition += Vector3.forward * adjustment.Forward;
            }
            if (adjustment.Up > 0f)
            {
                currentPosition += Vector3.up * adjustment.Up;
            }
            if (adjustment.Right > 0f)
            {
                currentPosition += Vector3.right * adjustment.Right;
            }
            return currentPosition;
        }

        internal static void LogUnknownItem(string trashID)
        {

            MelonLoader.MelonLogger.Msg($"Not yet maintained in RecyclerDumpster.json => {trashID} - modify/notify dev - defaulting to 1$.");
        }

    }
}