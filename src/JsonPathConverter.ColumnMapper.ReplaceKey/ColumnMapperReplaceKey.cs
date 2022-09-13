﻿using JsonPathConverter.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace JsonPathConverter.ColumnMapper.ReplaceKey
{
    public class ColumnMapperReplaceKey : IJsonColumnMapper
    {
        public JsonMapResult<TData> Map<TData>(string jsonSourceStr, JsonPathRoot jsonPathRoot)
        {
            return MapToStr<TData>(jsonSourceStr, jsonPathRoot);
        }

        public JsonMapResult<IEnumerable<IDictionary<string, object?>>> MapToCollection(string jsonSourceStr, JsonPathRoot jsonPathRoot)
        {
            return MapToStr<IEnumerable<IDictionary<string, object?>>>(jsonSourceStr, jsonPathRoot);
        }

        public JsonMapResult<IDictionary<string, object?>> MapToDic(string jsonSourceStr, JsonPathRoot jsonPathRoot)
        {
            return MapToStr<IDictionary<string, object?>>(jsonSourceStr, jsonPathRoot);
        }

        public TData? CaptureObject<TData>(string jsonSourceStr, string path)
        {
            var jToken = JToken.Parse(jsonSourceStr);
            if (jToken == null)
            {
                return default;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    jToken = jToken.SelectToken(path);
                }
                catch
                {
                    throw new Exception("json来源的数组项配置不正确");
                }
            }
            else
            {
                throw new ArgumentException("路径不能为空");
            }

            string jTokenStr = jToken?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(jTokenStr))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TData>(jTokenStr);
        }

        private JsonMapResult<TData> MapToStr<TData>(string jsonSourceStr, JsonPathRoot jsonPathRoot)
        {
            var result = new JsonMapResult<TData>((str) => JsonConvert.DeserializeObject<TData>(str));

            var relations = jsonPathRoot.JsonPathMapperRelations?.Where(s => s.IsValidate()).OrderByDescending(s => s.GetFileds().Length).ToList();
            if (relations == null || relations.Count == 0)
            {
                return result;
            }
            var jToken = JToken.Parse(jsonSourceStr);
            if (jToken == null)
            {
                return result;
            }

            if (!string.IsNullOrEmpty(jsonPathRoot.RootPath))
            {
                try
                {
                    jToken = jToken.SelectToken(jsonPathRoot.RootPath);
                    if (jToken != null && jToken.Type != JTokenType.Array
                        && typeof(TData).IsGenericType
                        && typeof(TData).IsAssignableTo(typeof(IEnumerable)))
                    {
                        result = new JsonMapResult<TData>((str) =>
                        {
                            var newStr = $"[{str}]";

                            return JsonConvert.DeserializeObject<TData>(newStr);
                        });
                    }
                }
                catch
                {
                    throw new Exception("json来源的数组项配置不正确");
                }
            }

            if (jToken == null)
            {
                return result;
            }

            JsonPathAdapter jsonPathAdapter = new JsonPathAdapter();

            foreach (var relation in relations)
            {
                string jsonPath = relation.SourceJsonPath ?? string.Empty;

                string jsonPathAdapterResult = jsonPathAdapter.Adapter(jsonPath, jToken);

                var matchJsonToekns = jToken.SelectTokens(jsonPathAdapterResult).ToList();
                if (matchJsonToekns == null || matchJsonToekns.Count() == 0)
                {
                    result.PropertyInfos.Add(new JsonMapInfo
                    {
                        SourcePath = relation.SourceJsonPath ?? string.Empty,
                        DestinationFiled = relation.DestinationJsonColumnCode ?? string.Empty,
                        ErrorMessage = "can't find transfer JTokens in json"
                    });
                    continue;
                }
                foreach (var token in matchJsonToekns)
                {
                    var jProperty = token.Parent;
                    if (jProperty == null)
                    {
                        continue;
                    }
                    if (jProperty.Type != JTokenType.Property)
                    {
                        result.PropertyInfos.Add(new JsonMapInfo
                        {
                            SourcePath = token.Path,
                            DestinationFiled = relation.DestinationJsonColumnCode ?? string.Empty,
                            ErrorMessage = "resolve type error"
                        });
                        continue;
                    }
                    var newProperty = new JProperty(relation.DestinationJsonColumnCode ?? string.Empty, token);
                    jProperty.Replace(newProperty);
                }
            }
            result.MapJsonStr = jToken.ToString();
            return result;
        }
    }
}
