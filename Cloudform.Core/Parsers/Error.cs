﻿using System;
namespace Cloudform.Core.Parsers
{
    public class Error
    {
        public const string UnknownSyntax = "Unknown syntax";
        public const string UnknownSqlSyntax = "Unknown SQL syntax";
        public const string UnknownType = "Unknown type";
        public const string MissingOpenBrace = "Missing {";
        public const string MissingCloseBrace = "Missing }";
        public const string UnknownComponent = "Unknown component";
        public const string TriggerExpected = "Input expected";
        public const string InvalidTimerPeriod = "Invalid timer period - Should be a number represnting seconds.";
        public const string InvalidComponentName = "Component name should be all lower case and only include dashes in the middle";
        public const string UknownQueue = "Uknown queue";
        public const string ParameterTypeMissing = "Parameter type missing";
        public const string InvalidReturnType = "Invalid return type - Acceptable types are array, entity, or scalar";

        public Error(string message, string what)
        {
            Message = message;
            What = what;
        }

        public Error(string message, int lineNumber = 0, string what = null)
        {
            LineNumber = lineNumber;
            Message = message;
            What = what; 
        }

        public int LineNumber { get; set; }
        public string Message { get; set; }
        public string What { get; set; }
    }
}
