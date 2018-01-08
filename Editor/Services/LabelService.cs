using System;
using BeeGraph.Data.Entities;

namespace BeeGraph.Editor
{
    public static class LabelService
    {
        public static string GetLabel(NodeEntity node) => GetLabel(node.Id, node.Body);
        public static string GetLabel(EdgeEntity edge) => GetLabel(edge.Id, edge.Key);

        private static string GetLabel(object id, string body) => $"[{id}] - {body}";

        public static int GetIdentifier(string str)
        {
            var indexOfLastDigit = str.IndexOf(']') - 1;
            var subString = str.Substring(1, indexOfLastDigit);
            return Int32.Parse(subString);
        }
    }
}