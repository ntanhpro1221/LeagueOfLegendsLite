#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NGDtuanh.Utils.Editor;
using UnityEditor;

namespace NGDtuanh.Strum {
	[InitializeOnLoad]
	public class StrumGenerator {
		private static readonly string GeneratedDir = Path.Combine(
			AssetHelper.GetScriptPathWithoutFileName(nameof(StrumGenerator))
		  , "Generated");

		private static void GenerateEnum(Request request) {
			var (strumName, enumName, fields) = request;
			bool haveField = fields.Count != 0;
			if (!Directory.Exists(GeneratedDir)) Directory.CreateDirectory(GeneratedDir);
			AssetHelper.SafeWriteToFile(
				Path.Combine(GeneratedDir, $"{strumName}.g.cs")
			  , $@"/* GENERATED CODE */
public static partial class Strum {{ 
	public static partial class {strumName} {{
		public partial struct Fields<T> {{
			{(haveField ? $"public T {string.Join(";\n\t\t\tpublic T ", fields)};" : string.Empty)}

			public T this[{enumName} index] {{
				get => this.ValueRO(index);
				set => this.ValueRW(index) = value;
			}}
		}}

		public struct Info {{
			public const {enumName} First = {(haveField ? $"{enumName}.{fields.First()}" : string.Empty)};
			public const {enumName} Last  = {(haveField ? $"{enumName}.{fields.Last()}" : string.Empty)};

			public const int Count = {fields.Count};
			
			public static readonly Info Indexes = default;

			public Enumerator GetEnumerator() => Enumerator.New;    

			public struct Enumerator {{
				public static readonly Enumerator New = new() {{ current = First - 1 }};

				private {enumName} current;

				public bool MoveNext() => ++current <= Last;

				public {enumName} Current => current;
			}}
		}}
	}}
}}

public static partial class StrumExtensions {{
    public static ref readonly T ValueRO<T>(this in Strum.{strumName}.Fields<T> strum, {enumName} index) {{
        switch (index) {{
			{(haveField ? $"case {string.Join(";\n\t\t\tcase ", fields.Select(field => $"{enumName}.{field}: return ref strum.{field}"))};" : string.Empty)}

            default: throw new System.ArgumentOutOfRangeException(nameof(index), index, null);
        }}
    }}

    public static ref T ValueRW<T>(this ref Strum.{strumName}.Fields<T> strum, {enumName} index) {{
        switch (index) {{
			{(haveField ? $"case {string.Join(";\n\t\t\tcase ", fields.Select(field => $"{enumName}.{field}: return ref strum.{field}"))};" : string.Empty)}

            default: throw new System.ArgumentOutOfRangeException(nameof(index), index, null);
        }}
    }}
}}");
		}

		static StrumGenerator() => AppDomain.CurrentDomain
			.GetAssemblies().First(asm => asm.GetName().Name == "Assembly-CSharp").GetTypes()
			.Where(static item => Attribute.IsDefined(item, typeof(StrumAttribute)))
			.Select(static item => new Request(
				((StrumAttribute)Attribute.GetCustomAttribute(item, typeof(StrumAttribute))).StrumName
			  , GetFullName(item)
			  , Enum.GetNames(item).ToList()))
			.ToList().ForEach(GenerateEnum);

		private static string GetFullName(Type type) {
			string result = type.Name;

			var declaringType = type;
			while ((declaringType = declaringType.DeclaringType) is not null)
				result = $"{declaringType.Name}.{result}";

			return $"global::{(type.Namespace is null ? string.Empty : type.Namespace + '.')}{result}";
		}

		private record Request(string StrumName, string EnumName, List<string> Fields) {
			public string       StrumName { get; } = StrumName;
			public string       EnumName  { get; } = EnumName;
			public List<string> Fields    { get; } = Fields;
		}
	}
}

#endif