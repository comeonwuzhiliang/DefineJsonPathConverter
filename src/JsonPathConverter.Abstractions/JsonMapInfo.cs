﻿namespace JsonPathConverter.Abstractions
{
    public class JsonMapInfo
    {
        public string SourcePath { get; set; } = string.Empty;

        public string DestinationFiled { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
