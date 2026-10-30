using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
    [Serializable, NetSerializable]
    public sealed class GasVentScrubberData : IAtmosDeviceData
    {
        public bool Enabled { get; set; }
        public bool Dirty { get; set; }
        public bool IgnoreAlarms { get; set; } = false;
        public HashSet<Gas> FilterGases { get; set; } = new(DefaultFilterGases);
        public Dictionary<Gas, float> FilterGasLimits { get; set; } = new(DefaultFilterGasLimits); // Persistence: Scrub above percentage
        public ScrubberPumpDirection PumpDirection { get; set; } = ScrubberPumpDirection.Scrubbing;
        public float VolumeRate { get; set; } = 200f;
        public bool WideNet { get; set; } = false;
        public bool AirAlarmPanicWireCut { get; set; }
        public string Name { get; set; } = "";

        public static HashSet<Gas> DefaultFilterGases = new()
        {
            Gas.Nitrogen, // Persistence: Limited by the filter limits below, will only scrub if above 80%
            Gas.CarbonDioxide,
            Gas.Plasma,
            Gas.Tritium,
            Gas.WaterVapor,
            Gas.Ammonia,
            Gas.NitrousOxide,
            Gas.Frezon,
            Gas.Methane,
            Gas.Chlorine,
            Gas.Fluorine,
            Gas.ChlorineTrifluoride,
            Gas.Hydrogen
        };

        // Start Persistence: Scrub above percentage
        public static Dictionary<Gas, float> DefaultFilterGasLimits = new()
        {
            { Gas.Nitrogen, 80 }
        };

        // Continue Persistence: Environmental Siphoning
        public float EnvironmentalSiphonRate = 200f;

        public ScrubberEnvironmentalSiphonData[] EnvironmentalSiphonData = (ScrubberEnvironmentalSiphonData[])DefaultEnvironmentalSiphonData.Clone();

        private static readonly ScrubberEnvironmentalSiphonData[] _defaultEnvironmentalSiphonData =
        [
            new(ScrubberEnvironmentalSiphonFilterType.Pressure, false, 0, ScrubberEnvironmentalSiphonComparisonDirection.Below, ScrubberEnvironmentalSiphonComparisonAction.Prevent, 90f),
            new(ScrubberEnvironmentalSiphonFilterType.Pressure, false, 1, ScrubberEnvironmentalSiphonComparisonDirection.Above, ScrubberEnvironmentalSiphonComparisonAction.Siphon, 110f),
            new(ScrubberEnvironmentalSiphonFilterType.Temperature, false, 0, ScrubberEnvironmentalSiphonComparisonDirection.Above, ScrubberEnvironmentalSiphonComparisonAction.Siphon, 283.15f),
            new(ScrubberEnvironmentalSiphonFilterType.Temperature, false, 1, ScrubberEnvironmentalSiphonComparisonDirection.Below, ScrubberEnvironmentalSiphonComparisonAction.Siphon, 253.15f),
        ];

        public static ScrubberEnvironmentalSiphonData[] DefaultEnvironmentalSiphonData => (ScrubberEnvironmentalSiphonData[])_defaultEnvironmentalSiphonData.Clone();

        // ScrubberEnvironmentalSiphonFilterType filterType,
        // short index,
        // bool enabled,
        //     ScrubberEnvironmentalSiphonComparisonDirection comparisonDirection,
        // ScrubberEnvironmentalSiphonComparisonAction action,
        // float threshold)
        // End Persistence

        // Presets for 'dumb' air alarm modes

        public static GasVentScrubberData FilterModePreset = new GasVentScrubberData
        {
            Enabled = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = false
        };

        public static GasVentScrubberData WideFilterModePreset = new GasVentScrubberData
        {
            Enabled = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = true
        };

        public static GasVentScrubberData FillModePreset = new GasVentScrubberData
        {
            Enabled = false,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = false
        };

        public static GasVentScrubberData PanicModePreset = new GasVentScrubberData
        {
            Enabled = true,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Siphoning,
            VolumeRate = 200f,
            WideNet = true
        };

        public static GasVentScrubberData ReplaceModePreset = new GasVentScrubberData
        {
            Enabled = true,
            IgnoreAlarms = true,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Siphoning,
            VolumeRate = 200f,
            WideNet = false
        };
    }

    [Serializable, NetSerializable]
    public enum ScrubberPumpDirection : sbyte
    {
        Siphoning = 0,
        Scrubbing = 1,
    }

    public enum ScrubberEnvironmentalSiphonFilterType : sbyte
    {
        Pressure,
        Temperature
    }

    [Serializable, NetSerializable]
    public enum ScrubberEnvironmentalSiphonComparisonDirection : byte
    {
        Above,
        Below
    }

    public enum ScrubberEnvironmentalSiphonComparisonAction : byte
    {
        Siphon,
        Prevent
    }

    [Serializable, NetSerializable]
    public enum ScrubberEnvironmentalSiphonSlot : sbyte
    {
        PressureOne = 0,
        PressureTwo = 1,
        TempOne = 2,
        TempTwo = 3,

        Pressure = PressureOne | PressureTwo,
        Temp = TempOne | TempTwo
    }

    [DataDefinition, Serializable, NetSerializable]
    public partial struct ScrubberEnvironmentalSiphonData(
        ScrubberEnvironmentalSiphonFilterType filterType,
        bool enabled,
        byte index,
        ScrubberEnvironmentalSiphonComparisonDirection comparisonDirection,
        ScrubberEnvironmentalSiphonComparisonAction action,
        float threshold)
    {
        public ScrubberEnvironmentalSiphonFilterType FilterType = filterType;
        public bool Enabled = enabled;
        public byte Index = index;
        public ScrubberEnvironmentalSiphonComparisonDirection ComparisonDirection = comparisonDirection;
        public ScrubberEnvironmentalSiphonComparisonAction Action = action;
        public float Threshold = threshold;

        public bool ConditionsMet(float reading)
        {
            return ComparisonDirection == ScrubberEnvironmentalSiphonComparisonDirection.Above
                ? reading > Threshold
                : reading < Threshold;
        }
    }

    // [DataRecord, Serializable, NetSerializable]
    // public partial record ScrubberEnvironmentalSiphonData(ScrubberEnvironmentalSiphonType Type, bool Enabled, float Value)
    // {
    //     public ScrubberEnvironmentalSiphonType Type;
    //     public bool Enabled;
    //     public float Value;
    // }

}
