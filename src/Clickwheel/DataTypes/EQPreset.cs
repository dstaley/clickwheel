using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Clickwheel.Exceptions;
using Clickwheel.Parsers;

namespace Clickwheel.DataTypes
{
    public sealed record EQPreset 
    {
        private static readonly Dictionary<int, EQPreset> Presets = new();
        
        public static readonly EQPreset Acoustic = new(100);
        public static readonly EQPreset BassBooster = new(101);
        public static readonly EQPreset BassReducer = new(102);
        public static readonly EQPreset Classical = new(103);
        public static readonly EQPreset Dance = new(104);
        public static readonly EQPreset Deep = new(105);
        public static readonly EQPreset Electronic = new(106);
        public static readonly EQPreset Flat = new(107);
        public static readonly EQPreset HipHop = new(108);
        public static readonly EQPreset Jazz = new(109);
        public static readonly EQPreset Latin = new(110);
        public static readonly EQPreset Loudness = new(111);
        public static readonly EQPreset Lounge = new(112);
        public static readonly EQPreset Piano = new(113);
        public static readonly EQPreset Pop = new(114);
        public static readonly EQPreset RhythmAndBlues = new(115);
        public static readonly EQPreset Rock = new(116);
        public static readonly EQPreset SmallSpeakers = new(117);
        public static readonly EQPreset SpokenWord = new(118);
        public static readonly EQPreset TrebleBooster = new(119);
        public static readonly EQPreset TrebleReducer = new(120);
        public static readonly EQPreset VocalBooster = new(121);

        public string Name { get; init; }
        public int ID { get; init; }

        private EQPreset(int id, [CallerMemberName] string name = "") : this(name, id) {}
        private EQPreset(string name, int id)
        {
            Name = name;
            ID = id;
            Presets.Add(id, this);
        }
        
        private const string _magicString = "#!#";

        public static string EncodeAsString(EQPreset preset)
        {
            if (preset == null)
                return "";
            
            return $"{_magicString}{preset.ID}{_magicString}";
        }

        public static EQPreset DecodeFromString(string input)
        {
            if (input == "") return null;
            
            if (   input.Length != 9 // three digit code, plus prefix and suffix
                || !input.StartsWith(_magicString)
                || !input.EndsWith(_magicString)
                || !int.TryParse(input.Substring(3, 3), out var id)
            ) throw new ParseException($"\"{input}\" is not a valid EQ Preset ID.", null);

            return Presets.GetValueOrDefault(id) ?? new EQPreset("Unknown EQ Preset", id);
        }
    }
}