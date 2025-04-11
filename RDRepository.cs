using Harmony;
using Il2CppScheduleOne.Trash;
//using Il2CppVLB;
using MelonLoader;
using MelonLoader.Utils;
using System.Collections.Generic;
//using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using static RecyclerDumpsterMod.RDUtility;

namespace RecyclerDumpsterMod
{
    public static class RDRepository
    {
        public static List<TrashValue> TrashValues;// = new Dictionary<string, TrashValue>();
        public static readonly List<RecycableDumpster> RecycableDumpsters;
        private const float Y_OFFSET = 0.65f;  // Vertical offset above the dumpster
        public static Configurables _CONFIG;

        static RDRepository()
        {
            RecycableDumpsters = new List<RecycableDumpster>();
            PopulateRecycableDumpsters();

            _CONFIG = LoadConfig();
            LoadTrashJson();
        }

        private static Configurables LoadConfig()
        {
            string configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "RecyclerDumpster.cfg");
            if (!File.Exists(configPath))
            {
                Configurables defaultConfig = new Configurables();
                File.WriteAllText(configPath, "# Unity KeyCode documentation: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/KeyCode.html\n\n[TrashValueConfig]\nGlass = 30\nPlastic = 2\nMetal = 2\nOthers = 1\n\n[Config]\nHotKey = Delete");
                return defaultConfig;
            }

            Configurables config = new Configurables();
            string[] lines = File.ReadAllLines(configPath);
            string currentSection = string.Empty;

            foreach (string line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Trim('[', ']');
                    continue;
                }

                if (currentSection == "TrashValueConfig")
                {
                    if (line.StartsWith("Glass"))
                        config.Glass = int.Parse(line.Split('=')[1].Trim());
                    else if (line.StartsWith("Plastic"))
                        config.Plastic = int.Parse(line.Split('=')[1].Trim());
                    else if (line.StartsWith("Metal"))
                        config.Metal = int.Parse(line.Split('=')[1].Trim());
                    else if (line.StartsWith("Others"))
                        config.Others = int.Parse(line.Split('=')[1].Trim());
                }
                else if (currentSection == "Config")
                {
                    if (line.StartsWith("HotKey"))
                        config.HotKey = (KeyCode)Enum.Parse(typeof(KeyCode), line.Split('=')[1].Trim());
                }
            }
            return config;
        }

        public static void LoadTrashJson()
        {
            string jsonPath = Path.Combine(MelonEnvironment.UserDataDirectory, "RecyclerDumpster.json");
            if (!File.Exists(jsonPath))
            {
                MelonLogger.Error("RecyclerDumpster.json not found in UserData directory.");
                throw new FileNotFoundException("RecyclerDumpster.json not found in UserData directory.");
            }

            string jsonContent = File.ReadAllText(jsonPath);
            try
            {
                if(!IsValidJson(jsonContent)) throw new JsonException("Invalid JSON format.");
                TrashValues = JsonConvert.DeserializeObject<List<TrashValue>>(jsonContent);
                if (TrashValues == null)
                {
                    MelonLogger.Error("RecyclerDumpster.json is invalid or missing TrashValues.");
                    throw new InvalidOperationException("RecyclerDumpster.json is invalid or missing TrashValues.");
                }
                PopulateTrashValues(TrashValues);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Unexpected error: {ex.Message}", true);
                throw;
            }
        }

        public static void PopulateTrashValues(List<TrashValue> trashValues)
        {
            foreach (TrashValue trash in trashValues)
            {
                switch (trash.Type.ToLower())
                {
                    case "glass":
                        trash.Value = _CONFIG.Glass;
                        break;
                    case "plastic":
                        trash.Value = _CONFIG.Plastic;
                        break;
                    case "metal":
                        trash.Value = _CONFIG.Metal;
                        break;
                    case "others":
                        trash.Value = _CONFIG.Others;
                        break;
                    default:
                        trash.Value = 0; // or some default value
                        break;
                }
            }            
        }

        private static void PopulateRecycableDumpsters()
        {
            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "motel1",
                Name = "Motel Dumpster Front",
                Position = new Vector3(-55.0832f, 0.8732f, 101.296f),
                Extent = new Vector3(0.8303f, 0.8169f, 1.8517f),
                ButtonPosition = new Vector3(-55.1001f, 0.5019f, 99.905f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    Forward = -0.5f,
                    Up = 0.85f,
                    //Right = -0f
                }

            });

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "motel2",
                Name = "Motel Dumpster Side",
                Position = new Vector3(-71.0901f, 0.8001f, 79.7941f),
                Extent = new Vector3(1.8671f, 0.8001f, 0.8526f),
                ButtonPosition = new Vector3(-69.6993f, 0.429f, 79.7664f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    //Forward = 0f,
                    Up = 0.85f,
                    Right = 0.5f
                }
            });

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "sweatshop",
                Name = "Sweat Shop",
                Position = new Vector3(-57.864f, -3.1999f, 151.147f),
                Extent = new Vector3(1.8517f, 0.8001f, 0.825f),
                ButtonPosition = new Vector3(-59.255f, -3.571f, 151.147f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    //Forward = 0.5f,
                    Up = 0.85f,
                    Right = -0.5f
                }

            });

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "bungalow",
                Name = "Bungalow",
                Position = new Vector3(-161.2222f, -3.1986f, 119.2805f),
                Extent = new Vector3(0.9006f, 0.8013f, 1.8811f),
                ButtonPosition = new Vector3(-161.2989f, -3.5709f, 117.8972f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    Forward = -0.5f,
                    Up = 0.85f,
                    //Right = -0f
                }
            });

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "barn",
                Name = "Barn",
                Position = new Vector3(181.6679f, 0.8001f, -16.536f),
                Extent = new Vector3(0.825f, 0.8001f, 1.8517f),
                ButtonPosition = new Vector3(181.668f, 0.429f, -15.145f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    Forward = 0.5f,
                    Up = 0.85f,
                    //Right = -0f
                }
            });

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "docks",
                Name = "Docks Warehouse",
                Position = new Vector3(-69.5058f, -1.6199f, -63.5107f),
                Extent = new Vector3(2.012f, 0.8001f, 1.6332f),
                ButtonPosition = new Vector3(-70.7109f, -1.991f, -64.207f),
                ButtonPositionAdjustment = new PositionAdjustment
                {
                    Forward = -0.1f,
                    Up = 0.85f,
                    Right = -0.25f
                }
            });

            for (int i = 0; i < RecycableDumpsters.Count; i++)
            {
                (Vector3 min, Vector3 max) bounds = ComputeBoundsRounded(
                    RecycableDumpsters[i].Position,
                    RecycableDumpsters[i].Extent,
                    Y_OFFSET
                );

                RecycableDumpsters[i].Min = bounds.min;
                RecycableDumpsters[i].Max = bounds.max;
            }
        }

        public class TrashValue
        {
            public string ID { get; set; }
            public string Type { get; set; }
            public int Value { get; set; }
        }

        public class Configurables
        {
            public int Glass { get; set; } = 3;
            public int Plastic { get; set; } = 2;
            public int Metal { get; set; } = 2;
            public int Others { get; set; } = 1;
            public KeyCode HotKey = KeyCode.Delete; // Default HotKey
        }

        [Serializable]
        public class RecycableDumpster
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 Extent { get; set; }
            public Vector3 ButtonPosition { get; set; } = new Vector3(0, 0, 0); // Default value
            public PositionAdjustment ButtonPositionAdjustment { get; set; } = new PositionAdjustment();
            public Vector3 Min { get; set; }
            public Vector3 Max { get; set; }
        }
        [Serializable]
        public class PositionAdjustment
        {
            // Forward adjustment (positive moves forward, negative moves backward)
            public float Forward { get; set; } = 0f;

            // Upward adjustment (positive moves upward, negative moves downward)
            public float Up { get; set; } = 0f;

            // Rightward adjustment (positive moves right, negative moves left)
            public float Right { get; set; } = 0f;
        }
    }
}