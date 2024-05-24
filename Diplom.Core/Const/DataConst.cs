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
        }

        public static class ReleationShipMessageValidation
        {
            public static readonly string IdMessage = "Id must not be empty or null";
            public static readonly string EndPointMessage = "EndPoint must not be empty or null";
        }

        //переверь методы (добавь в связи новые + (переделай старые)
        //добавиь интерполяцию везде (переверь везде)
        //займись алгоритмами 
        //дедлайн конец дня 13.05.24 - 14.05.24


    }
}
