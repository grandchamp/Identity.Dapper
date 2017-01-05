using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Identity.Dapper
{
    public static class SqlQueryParametersExtensions
    {
        public static List<string> InsertQueryValuesFragment(this List<string> valuesArray, string parameterNotation, IEnumerable<string> propertyNames)
        {
            foreach (var property in propertyNames)
                valuesArray.Add($"{parameterNotation}{property.Replace("\"", "")}");

            return valuesArray;
        }

        public static string UpdateQuerySetFragment(this IEnumerable<string> propertyNames, string parameterNotation)
        {
            var setBuilder = new StringBuilder();

            var propertyNamesArray = propertyNames.ToArray();
            for (int i = 0; i < propertyNamesArray.Length; i++)
            {
                var propertyName = propertyNamesArray[i];

                if (i == 0)
                    setBuilder.Append($"SET {propertyName} = {parameterNotation}{propertyName.Replace("\"", "")}");
                else
                    setBuilder.Append($", {propertyName} = {parameterNotation}{propertyName.Replace("\"", "")}");
            }

            return setBuilder.ToString();
        }

        public static string SelectFilterWithTableName(this IEnumerable<string> propertyNames, string tableName)
        {
            var propertyNamesArray = propertyNames.ToArray();
            var filterBuilderArray = new List<string>(propertyNamesArray.Length);

            for (int i = 0; i < propertyNamesArray.Length; i++)
                filterBuilderArray.Add($"\"{tableName}\".\"{propertyNamesArray[i]}\"");

            return string.Join(", ", filterBuilderArray);
        }
    }
}
