using System.Reflection;
using Xunit.Sdk;

namespace Multiformats.Base.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CsvDataAttribute(string fileName) : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var pars = testMethod.GetParameters();
        var parameterTypes = pars.Select(par => par.ParameterType).ToArray();
        foreach (var line in File.ReadLines(fileName).Skip(1))
        {
            //csvFile.ReadLine();// Delimiter Row: "sep=,". Comment out if not used
            var row = line.Split(',').Select(c => c.Trim('"', ' ')).ToArray();
            yield return ConvertParameters(row, parameterTypes);
        }
    }

    private static object ConvertParameter(object parameter, Type parameterType) =>
        parameterType == typeof(int) ? Convert.ToInt32(parameter) : parameter;

    private static object[] ConvertParameters(IReadOnlyList<object> values, IReadOnlyList<Type> parameterTypes)
    {
        var result = new object[parameterTypes.Count];
        for (var idx = 0; idx < parameterTypes.Count; idx++)
        {
            result[idx] = ConvertParameter(values[idx], parameterTypes[idx]);
        }

        return result;
    }
}
