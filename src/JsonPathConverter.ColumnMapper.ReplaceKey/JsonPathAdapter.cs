﻿using Newtonsoft.Json.Linq;

namespace JsonPathConverter.ColumnMapper.ReplaceKey
{
    public class JsonPathAdapter
    {
        private Dictionary<string, string> jsonPathAdapters = new Dictionary<string, string>();

        public string Adapter(string jsonSourcePath, JToken jToken)
        {
            string adapterResult = string.Empty;
            string jsonSourcePathSelectionSplicing = string.Empty;

            var jsonSourcePathSelects = jsonSourcePath.Split('.');

            for (int i = 0; i < jsonSourcePathSelects.Length; i++)
            {
                var jsonSourcePathSelectionItem = jsonSourcePathSelects[i];

                if (jsonSourcePathSelectionSplicing != string.Empty)
                {
                    jsonSourcePathSelectionSplicing = jsonSourcePathSelectionSplicing + ".";
                }

                jsonSourcePathSelectionSplicing = jsonSourcePathSelectionSplicing + jsonSourcePathSelectionItem;

                if (jsonPathAdapters.ContainsKey(jsonSourcePathSelectionSplicing))
                {
                    adapterResult = jsonPathAdapters[jsonSourcePathSelectionSplicing];
                    continue;
                }

                if (adapterResult != string.Empty)
                {
                    adapterResult = adapterResult + ".";
                }

                try
                {
                    var temporaryAdapterResult = adapterResult + jsonSourcePathSelectionItem;
                    if (i == jsonSourcePathSelects.Length - 1)
                    {
                        adapterResult = temporaryAdapterResult;
                        break;
                    }

                    var token = jToken.SelectToken(temporaryAdapterResult);
                    if (token?.Type == JTokenType.Array)
                    {
                        temporaryAdapterResult = adapterResult + jsonSourcePathSelectionItem + "[*]";
                    }
                    adapterResult = temporaryAdapterResult;
                }
                catch
                {
                    try
                    {

                        if (jsonSourcePathSelectionItem.EndsWith("]") && jsonSourcePathSelectionItem.Contains("["))
                        {
                            adapterResult = adapterResult + jsonSourcePathSelectionItem;
                        }
                        else
                        {
                            //var temporaryAdapterResult = adapterResult + jsonSourcePathSelectionItem + "[*]";
                            //jToken.SelectTokens(temporaryAdapterResult);
                            //adapterResult = temporaryAdapterResult;

                            adapterResult = adapterResult + jsonSourcePathSelectionItem + "[*]";
                        }

                        jToken.SelectTokens(adapterResult);
                    }
                    catch
                    {
                        return "";
                    }
                }

                jsonPathAdapters[jsonSourcePathSelectionSplicing] = adapterResult;
            }

            return adapterResult;

        }
    }
}
