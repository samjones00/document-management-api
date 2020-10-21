using Newtonsoft.Json;
using Xunit;

namespace DocumentManager.Core.Tests.Helpers
{
    public static class TestHelper
    {
        public static bool HaveEqualPropertiesAndValues(this object object1, object object2)
        {
            var object1Json = JsonConvert.SerializeObject(object1);
            var object2Json = JsonConvert.SerializeObject(object2);

            return object1Json.Equals(object2Json);
        }
    }
}
