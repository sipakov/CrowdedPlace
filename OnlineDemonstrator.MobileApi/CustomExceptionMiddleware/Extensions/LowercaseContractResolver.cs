using Newtonsoft.Json.Serialization;

namespace OnlineDemonstrator.MobileApi.CustomExceptionMiddleware.Extensions
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}