using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tuples
{
    public static class ParameterInfo_Extensions
    {
        public class TupleArg
        {
            public TupleArg(Type type)
            {
                Type = type; Name = "Arg"; TupleArgs = new TupleArg[0];
            }
            public TupleArg(Type type, string name, TupleArg[] tupleArgs)
            {
                Type = type; Name = name; TupleArgs = tupleArgs;
            }
            public Type Type { get; }
            public string Name { get; }
            public TupleArg[] TupleArgs { get; }
            public override string ToString()
            {
                if (TupleArgs.Any())
                    return $"{Name} {{{string.Join(", ", TupleArgs.Select(ta => ta.ToString()).ToArray())}}}";
                return Type.ToString();
            }
        }

        static TupleArg[] PopulateTupleArgs(Type type, IList<string> tupleNames, ref int strsOffset)
        {
            if (type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
            {
                var tupleArgs = type.GetGenericArguments();
                var thisOffset = strsOffset;
                strsOffset += tupleArgs.Length;
                var ret = new TupleArg[tupleArgs.Length];
                for (int i = 0; i < tupleArgs.Length; ++i)
                {
                    ret[i] = new TupleArg(tupleArgs[i],
                        (tupleNames != null) ? tupleNames[i + thisOffset] : $"Item{i}",
                        PopulateTupleArgs(tupleArgs[i], tupleNames, ref strsOffset));
                }
                return ret;
            }
            return new[] { new TupleArg(type) };
        }

        public static bool TryGetValueTupleTypes(this ParameterInfo pi, out TupleArg[] args)
        {
            if (pi == null)
                ExThrowers.ThrowArgNull(nameof(pi));
            args = null;
            var paramType = pi.ParameterType;
            if (!paramType.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
                return false;
            var attr = pi.GetCustomAttribute<TupleElementNamesAttribute>();
            int offset = 0;
            args = PopulateTupleArgs(paramType, attr?.TransformNames, ref offset);
            return true;
        }

    }
}
