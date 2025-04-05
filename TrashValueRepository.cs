using System.Collections.Generic;
public static class TrashValueRepository
{
    public static readonly Dictionary<string, TrashValue> TrashValues = new Dictionary<string, TrashValue>
    {
        { "waterbottle", new TrashValue { ID = "waterbottle", Type = "plastic", Value = 2 } },
        { "cigarette", new TrashValue { ID = "cigarette", Type = "others", Value = 1 } },
        { "energydrink", new TrashValue { ID = "energydrink", Type = "metal", Value = 2 } },
        { "usedcigarette", new TrashValue { ID = "usedcigarette", Type = "others", Value = 1 } },
        { "litter1", new TrashValue { ID = "litter1", Type = "others", Value = 1 } },
        { "crushedcuke", new TrashValue { ID = "crushedcuke", Type = "metal", Value = 2 } },
        { "pipe", new TrashValue { ID = "pipe", Type = "glass", Value = 3 } },
        { "cuke", new TrashValue { ID = "cuke", Type = "metal", Value = 2 } },
        { "plantscrap", new TrashValue { ID = "plantscrap", Type = "other", Value = 1 } },
        { "cigarettebox", new TrashValue { ID = "cigarettebox", Type = "other", Value = 1 } },
        { "motoroil", new TrashValue { ID = "motoroil", Type = "metal", Value = 2 } },
        { "bong", new TrashValue { ID = "bong", Type = "glass", Value = 3 } },
        { "syringe", new TrashValue { ID = "syringe", Type = "plastic", Value = 3 } },
        { "addy", new TrashValue { ID = "addy", Type = "plastic", Value = 2 } },
        { "glassbottle", new TrashValue { ID = "glassbottle", Type = "glass", Value = 3 } },
        { "coffeecup", new TrashValue { ID = "coffeecup", Type = "others", Value = 1 } }
    };
}

public class TrashValue
{
    public string ID { get; set; }
    public string Type { get; set; }
    public int Value { get; set; }
   
}