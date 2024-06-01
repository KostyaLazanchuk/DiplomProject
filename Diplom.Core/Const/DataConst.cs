using System.ComponentModel.DataAnnotations;

namespace Diplom.Core.Const
{
    public static class DataConst
    {
        public static class ConnectionData
        {
            public static readonly string url = "bolt://localhost:7687";
            public static readonly string user = "neo4j";
            public static readonly string password = "Kostya2032";
        }

        public static class NodeMessageValidation
        {
            public static readonly string NameMessage = "Name must not be empty or null";
            public static readonly string IdMessage = "Id must not be empty or null";
            public static readonly string PostionMessage = "Position must not be empty or null";
            public static readonly string InvalidColor = "Invalid color value.";
        }

        public static class ReleationShipMessageValidation
        {
            public static readonly string IdMessage = "Id must not be empty or null";
            public static readonly string EndPointMessage = "EndPoint must not be empty or null";
        }

        public static class Color
        {
            public static readonly string Red = "Red";
            public static readonly string Orange = "Orange";
            public static readonly string Yellow = "Yellow";
            public static readonly string Green = "Green";
            public static readonly string Blue = "Blue";
            public static readonly string Purple = "Purple";
            public static readonly string White = "White";
        }
    }
}
