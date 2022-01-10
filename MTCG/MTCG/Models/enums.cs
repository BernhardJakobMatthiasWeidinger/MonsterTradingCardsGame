using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MTCG.Models {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ElementType {
        fire,
        water,
        normal
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonsterType {
        goblin,
        dragon,
        wizard,
        ork,
        knight,
        kraken,
        elf,
        troll
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardType {
        monster,
        spell
    }
}
