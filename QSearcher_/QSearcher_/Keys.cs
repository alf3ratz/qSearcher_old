using System;
using System.Collections.Generic;
using System.Text;

namespace QSearcher_
{
    class Keys
    {
        public Keys()
        {
        }
        //  URL of Azure Cosmos DB endpoint
        public static readonly string URL = @"https://qsearcher-app.documents.azure.com:443/";
        // The read/write authentication key of Azure Cosmos DB endpoint
        public static readonly string AuthKey = @"здесь должен быть ваш ключ :)";
    }
}
