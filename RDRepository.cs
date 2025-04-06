using Harmony;
using MelonLoader.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RecyclerDumpsterMod
{
    public static class RDRepository
    {
        public static readonly Dictionary<string, TrashValue> TrashValues;
        public static readonly List<RecycableDumpster> RecycableDumpsters;
        private const float EXTENT_X = 0.683f;
        private const float EXTENT_Y = 0.5f;  // Vertical coverage only for the top
        private const float EXTENT_Z = 0.683f;

        static RDRepository()
        {
            RecycableDumpsters = new List<RecycableDumpster>();
            PopulateRecycableDumpsters();

            var config = LoadConfig();
            TrashValues = new Dictionary<string, TrashValue>
            {
                { "waterbottle", new TrashValue { ID = "waterbottle", Type = "plastic", Value = config.Plastic } },
                { "cigarette", new TrashValue { ID = "cigarette", Type = "others", Value = config.Others } },
                { "energydrink", new TrashValue { ID = "energydrink", Type = "metal", Value = config.Metal } },
                { "usedcigarette", new TrashValue { ID = "usedcigarette", Type = "others", Value = config.Others } },
                { "litter1", new TrashValue { ID = "litter1", Type = "others", Value = config.Others } },
                { "crushedcuke", new TrashValue { ID = "crushedcuke", Type = "metal", Value = config.Metal } },
                { "pipe", new TrashValue { ID = "pipe", Type = "glass", Value = config.Glass } },
                { "cuke", new TrashValue { ID = "cuke", Type = "metal", Value = config.Metal } },
                { "plantscrap", new TrashValue { ID = "plantscrap", Type = "others", Value = config.Others } },
                { "cigarettebox", new TrashValue { ID = "cigarettebox", Type = "others", Value = config.Others } },
                { "motoroil", new TrashValue { ID = "motoroil", Type = "metal", Value = config.Metal } },
                { "bong", new TrashValue { ID = "bong", Type = "glass", Value = config.Glass } },
                { "syringe", new TrashValue { ID = "syringe", Type = "plastic", Value = config.Plastic } },
                { "addy", new TrashValue { ID = "addy", Type = "plastic", Value = config.Plastic } },
                { "glassbottle", new TrashValue { ID = "glassbottle", Type = "glass", Value = config.Glass } },
                { "coffeecup", new TrashValue { ID = "coffeecup", Type = "others", Value = config.Others } }
            };
        }

        private static TrashValueConfig LoadConfig()
        {
            string configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "RecyclerDumpster.cfg");
            if (!File.Exists(configPath))
            {
                var defaultConfig = new TrashValueConfig();
                File.WriteAllText(configPath, "[TrashValueConfig]\nGlass = 3\nPlastic = 2\nMetal = 2\nOthers = 1");
                return defaultConfig;
            }

            var config = new TrashValueConfig();
            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
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
            return config;
        }

        private static void PopulateRecycableDumpsters()
        {
            //RecycableDumpsters.Add(new RecycableDumpster
            //{
            //    ID = "motel",
            //    Name = "Motel Dumpster",
            //    Position = new Vector3(0, 0, 0)
            //});

            //RecycableDumpsters.Add(new RecycableDumpster
            //{
            //    ID = "sweatshop",
            //    Name = "Sweat Shop",
            //    Position = new Vector3(1, 1, 1)
            //});

            //RecycableDumpsters.Add(new RecycableDumpster
            //{
            //    ID = "bungalow",
            //    Name = "Bungalow",
            //    Position = new Vector3(2, 2, 2)
            //});

            //RecycableDumpsters.Add(new RecycableDumpster
            //{
            //    ID = "barn",
            //    Name = "Barn",
            //    Position = new Vector3(3, 3, 3)
            //});

            RecycableDumpsters.Add(new RecycableDumpster
            {
                ID = "docks",
                Name = "Docks Warehouse",
                Position = new Vector3(-69.4872f, -1.614f, -63.5005f)
            });

            for (int i = 0; i < RecycableDumpsters.Count; i++)
            {
                (Vector3 min, Vector3 max) bounds = RDUtility.ComputeBoundsRounded(
                    RecycableDumpsters[i].Position,
                    EXTENT_X,
                    EXTENT_Y,
                    EXTENT_Z
                );

                RecycableDumpsters[i].Min = bounds.min;
                RecycableDumpsters[i].Max = bounds.max;
            }
        }

        public static (float, float, float) GetExtents()
        {
            return (EXTENT_X, EXTENT_Y, EXTENT_Z);
        }

        public class TrashValue
        {
            public string ID { get; set; }
            public string Type { get; set; }
            public int Value { get; set; }
        }

        public class TrashValueConfig
        {
            public int Glass { get; set; } = 3;
            public int Plastic { get; set; } = 2;
            public int Metal { get; set; } = 2;
            public int Others { get; set; } = 1;
        }

        [Serializable]
        public class RecycableDumpster
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 Min { get; set; }
            public Vector3 Max { get; set; }
        }
    }
}