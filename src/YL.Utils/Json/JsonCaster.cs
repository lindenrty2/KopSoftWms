using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Utils.Json
{
    public static class JsonCaster
    {

        public static T CastTo<T>(this object source)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

    }
}
