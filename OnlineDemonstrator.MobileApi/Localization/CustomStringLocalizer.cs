using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace OnlineDemonstrator.MobileApi.Localization
{
    public class CustomStringLocalizer : IStringLocalizer
    {
        Dictionary<string, Dictionary<string, string>> resources;
        const string NewDemonstrationPush = "New demonstration";
        const string NewPosterPush = "New poster";
 
        public CustomStringLocalizer()
        {
            Dictionary<string, string> enDict = new Dictionary<string, string>
            {
                {NewDemonstrationPush, "New demonstration" },
                {NewPosterPush, "New poster" }
            };
            Dictionary<string, string> ruDict = new Dictionary<string, string>
            {
                {NewDemonstrationPush, "New demonstration" },
                {NewPosterPush, "New poster" }
            };
            Dictionary<string, string> deDict = new Dictionary<string, string>
            {
                {NewDemonstrationPush, "Neue Kundgebung" },
                {NewPosterPush, "Neues Plakat" }
            };
            Dictionary<string, string> esDict = new Dictionary<string, string>
            {
                {NewDemonstrationPush, "Nueva reunión" },
                {NewPosterPush, "Una pancarta nueva en tu reunión" }
            };
            Dictionary<string, string> jaDict = new Dictionary<string, string>
            {
                {NewDemonstrationPush, "新しいデモンストレーションを見てください" },
                {NewPosterPush, "デモの新しいポスター" }
            };
            // создаем словарь ресурсов
            resources = new Dictionary<string, Dictionary<string, string>>
            {
                {"en", enDict },
                {"ru", ruDict },
                {"de", deDict }
            };
        }
        public LocalizedString this[string name]
        {
            get
            {
                var currentCulture = new CultureInfo("en", false);
                string val = "";
                if (resources.ContainsKey(currentCulture.Name))
                {
                    if (resources[currentCulture.Name].ContainsKey(name))
                    {
                        val = resources[currentCulture.Name][name];
                    }
                }
                return new LocalizedString(name, val);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
 
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }
 
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}