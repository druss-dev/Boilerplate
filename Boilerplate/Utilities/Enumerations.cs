using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Boilerplate.Utilities;

public class Enumerations
{
    //[JsonConverter(typeof (StringEnumConverter))]
    public enum TheThingEnum
    {
        ThingOne = 1,
        ThingTwo = 2
    }
}