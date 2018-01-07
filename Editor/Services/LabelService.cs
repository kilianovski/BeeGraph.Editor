using System;
using BeeGraph.Data.Entities;

namespace BeeGraph.Editor
{
    public static class LabelService
    {
        public static string GetLabel(NodeEntity node) => $"[{node.Id}] - {node.Body}";

        public static int GetIdentifier(string str)
        {
            var indexOfLastDigit = str.IndexOf(']') - 1;
            var subString = str.Substring(1, indexOfLastDigit);
            return Int32.Parse(subString);
        }
    }
}