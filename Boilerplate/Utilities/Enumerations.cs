using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Boilerplate.Utilities;

public class Enumerations
{
    [JsonConverter(typeof (StringEnumConverter))]
    public enum ServiceLineSegmentation
    {
        Heart = 1,
        Orthopedic = 2,
        Lung = 3,
        Digestive = 4,
    }
    
    [JsonConverter(typeof (StringEnumConverter))]
    public enum TheThingEnum
    {
        ThingOne = 1,
        ThingTwo = 2
    }
}