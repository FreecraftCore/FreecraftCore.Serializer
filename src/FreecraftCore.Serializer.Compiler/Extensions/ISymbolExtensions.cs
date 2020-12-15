using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public static class ISymbolExtensions
	{
		public static bool IsTypeExact<T>(this ITypeSymbol symbol)
		{
			if (symbol.ContainingNamespace != null)
				return $"{symbol.ContainingNamespace.FullNamespaceString()}.{symbol.Name}" == typeof(T).FullName;
			else
				return $"{symbol.Name}" == typeof(T).FullName;
		}

		public static bool IsTypeLike<T>(this ITypeSymbol symbol)
		{
			string symbolName = symbol.ContainingNamespace != null 
				? $"{symbol.ContainingNamespace.FullNamespaceString()}.{symbol.Name}"
				: $"{symbol.Name}";

			if (symbolName == typeof(T).FullName)
				return true;

			if (typeof(T).IsInterface)
			{
				foreach(var interfaceType in symbol.AllInterfaces)
					if(interfaceType.IsTypeExact<T>())
						return true;
			}
			else
			{
				//Walking base types is annoying and dumb
				if (symbol.BaseType != null)
					do
					{
						if (symbol.IsTypeExact<T>())
							return true;

						symbol = symbol.BaseType;
					} 
					while (symbol.BaseType != null);
			}

			return false;
		}

		public static bool HasAttributeExact<T>([NotNull] this ISymbol symbol, bool inherit = false)
			where T : Attribute
		{
			if (symbol == null) throw new ArgumentNullException(nameof(symbol));

			if (!inherit)
			{
				return symbol
					.GetAttributes()
					.Any(a => a.AttributeClass.IsTypeExact<T>());
			}
			else
			{
				//Check current type first.
				if(symbol.HasAttributeExact<T>())
					return true;

				if(symbol is ITypeSymbol typeSymbol)
					foreach(var baseTypeSymbol in typeSymbol.GetAllBaseTypes())
					{
						if(baseTypeSymbol.HasAttributeExact<T>())
							return true;
					}
			}

			return false;
		}

		public static AttributeData GetAttributeExact<T>([NotNull] this ISymbol symbol, bool inherit = false)
			where T : Attribute
		{
			if(symbol == null) throw new ArgumentNullException(nameof(symbol));

			if (!inherit)
			{
				return symbol
					.GetAttributes()
					.First(a => a.AttributeClass.IsTypeExact<T>());
			}
			else
			{
				//Check current type first.
				if (symbol.HasAttributeExact<T>())
					return symbol.GetAttributeExact<T>();

				if(symbol is ITypeSymbol typeSymbol)
					foreach(var baseTypeSymbol in typeSymbol.GetAllBaseTypes())
					{
						if (baseTypeSymbol.HasAttributeExact<T>())
							return baseTypeSymbol.GetAttributeExact<T>();
					}
			}

			return null;
		}

		public static AttributeData GetAttributeLike<T>([NotNull] this ISymbol symbol, bool inherit = false)
			where T : Attribute
		{
			if(symbol == null) throw new ArgumentNullException(nameof(symbol));

			if(!inherit)
			{
				return symbol
					.GetAttributes()
					.First(a => a.AttributeClass.IsTypeLike<T>());
			}
			else
			{
				//Check current type first.
				if(symbol.HasAttributeLike<T>())
					return symbol.GetAttributeLike<T>();

				if(symbol is ITypeSymbol typeSymbol)
					foreach(var baseTypeSymbol in typeSymbol.GetAllBaseTypes())
					{
						if(baseTypeSymbol.HasAttributeLike<T>())
							return baseTypeSymbol.GetAttributeLike<T>();
					}
			}

			return null;
		}

		public static IEnumerable<AttributeData> GetAttributesExact<T>([NotNull] this ISymbol symbol)
			where T : Attribute
		{
			if(symbol == null) throw new ArgumentNullException(nameof(symbol));

			return symbol
				.GetAttributes()
				.Where(a => a.AttributeClass.IsTypeExact<T>());
		}

		public static IEnumerable<AttributeData> GetAttributesLike<T>([NotNull] this ISymbol symbol)
			where T : Attribute
		{
			if(symbol == null) throw new ArgumentNullException(nameof(symbol));

			return symbol
				.GetAttributes()
				.Where(a => a.AttributeClass.IsTypeLike<T>());
		}

		public static bool HasAttributeLike<T>([NotNull] this ISymbol symbol, bool inherit = false)
			where T : Attribute
		{
			if (!inherit)
			{
				return symbol
					.GetAttributes()
					.Any(a => a.AttributeClass.IsTypeLike<T>());
			}
			else
			{
				//Check current type first.
				if (symbol.HasAttributeLike<T>())
					return true;

				if (symbol is ITypeSymbol typeSymbol)
					foreach (var baseTypeSymbol in typeSymbol.GetAllBaseTypes())
					{
						if (baseTypeSymbol.HasAttributeLike<T>())
							return true;
					}
			}

			return false;
		}

		public static IEnumerable<ITypeSymbol> GetAllBaseTypes(this ITypeSymbol symbol)
		{
			if (symbol.BaseType == null)
				yield break;

			do
			{
				yield return symbol.BaseType;
				symbol = symbol.BaseType;
			} while (symbol.BaseType != null);
		}

		public static bool ImplementsInterface<T>(this ITypeSymbol symbol, bool inherit = false)
		{
			if (!typeof(T).IsInterface)
				throw new InvalidOperationException($"Type: {typeof(T).Name} is not an inteface type.");

			if (!inherit)
				return symbol.Interfaces
					.Any(i => i.IsTypeExact<T>());
			else
				return symbol.AllInterfaces
					.Any(i => i.IsTypeExact<T>());
		}


		/// <summary>
		/// Indicates if the provided Type is a wire message type or SHOULD be a wire message type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns></returns>
		public static bool IsWireMessageType([NotNull] this ITypeSymbol type)
		{
			if(type == null) throw new ArgumentNullException(nameof(type));

			//Generic types cannot be marked as wire messages because they lack
			//the ability to implement proper dispatching to their closed generic serializer.
			if (type is INamedTypeSymbol nts)
				if (nts.IsGenericType)
					return false;

			if (type.IsTypeExact<System.Object>())
				return false;

			if(type.HasAttributeLike<WireMessageTypeAttribute>())
				return true;

			if(type.HasAttributeLike<WireDataContractAttribute>())
			{
				if(DefinesPolymorphicContract(type))
					return true; //all polymorphic serializers need Wire Message implementation
			}

			//all polymorphic serializers need Wire Message implementation
			if (type.HasAttributeLike<WireDataContractBaseLinkAttribute>(false))
			{
				if (type.AllInterfaces.Any(i => i.IsTypeExact<ISelfSerializable>()))
					return true;
			}

			return false;
		}

		private static bool DefinesPolymorphicContract(ITypeSymbol type)
		{
			if (!type.HasAttributeExact<WireDataContractAttribute>(false))
				return false;
			
			AttributeData attri = type.GetAttributeExact<WireDataContractAttribute>(false);

			//TODO: This breaks if WireDataContractAttribute has more than 1 constructor
			if (WireDataContractAttribute.IsDefiningPolymorphicDefinition(attri.ConstructorArguments.Count()))
				return true;
			else
			{
				if(type.BaseType != null && type.BaseType.HasAttributeExact<WireDataContractAttribute>(false))
				{
					AttributeData baseAttri = type.BaseType.GetAttributeExact<WireDataContractAttribute>(false);

					//TODO: This breaks if WireDataContractAttribute has more than 1 constructor
					if(WireDataContractAttribute.IsDefiningPolymorphicDefinition(baseAttri.ConstructorArguments.Count()))
						return true;
				}
			}

			return false;
		}

		public static bool IsPrimitive(this ITypeSymbol symbol)
		{
			switch (symbol.SpecialType)
			{
				case SpecialType.None:
				case SpecialType.System_Object:
				case SpecialType.System_Enum:
				case SpecialType.System_MulticastDelegate:
				case SpecialType.System_Delegate:
				case SpecialType.System_ValueType:
				case SpecialType.System_Void:
					return false;
				case SpecialType.System_Boolean:
				case SpecialType.System_Char:
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return true;
				case SpecialType.System_String:
				case SpecialType.System_IntPtr:
				case SpecialType.System_UIntPtr:
				case SpecialType.System_Array:
				case SpecialType.System_Collections_IEnumerable:
				case SpecialType.System_Collections_Generic_IEnumerable_T:
				case SpecialType.System_Collections_Generic_IList_T:
				case SpecialType.System_Collections_Generic_ICollection_T:
				case SpecialType.System_Collections_IEnumerator:
				case SpecialType.System_Collections_Generic_IEnumerator_T:
				case SpecialType.System_Collections_Generic_IReadOnlyList_T:
				case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
				case SpecialType.System_Nullable_T:
				case SpecialType.System_DateTime:
				case SpecialType.System_Runtime_CompilerServices_IsVolatile:
				case SpecialType.System_IDisposable:
				case SpecialType.System_TypedReference:
				case SpecialType.System_ArgIterator:
				case SpecialType.System_RuntimeArgumentHandle:
				case SpecialType.System_RuntimeFieldHandle:
				case SpecialType.System_RuntimeMethodHandle:
				case SpecialType.System_RuntimeTypeHandle:
				case SpecialType.System_IAsyncResult:
				case SpecialType.System_AsyncCallback:
				case SpecialType.System_Runtime_CompilerServices_RuntimeFeature:
				case SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute:
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string GetFriendlyName(this ITypeSymbol type)
		{
			string friendlyName = type.Name;
			if(type is INamedTypeSymbol namedTypeSymbolRef && namedTypeSymbolRef.IsGenericType && !namedTypeSymbolRef.IsUnboundGenericType)
			{
				int iBacktick = friendlyName.IndexOf('`');
				if(iBacktick > 0)
				{
					friendlyName = friendlyName.Remove(iBacktick);
				}
				friendlyName += "<";
				ImmutableArray<ITypeSymbol> typeParameters = namedTypeSymbolRef.TypeArguments;
				friendlyName = ConcatArgs(typeParameters, friendlyName);
				friendlyName += ">";
			}

			return friendlyName;
		}

		private static string ConcatArgs(ImmutableArray<ITypeSymbol> typeParameters, string friendlyName)
		{
			for(int i = 0; i < typeParameters.Length; ++i)
			{
				string typeParamName = typeParameters[i] is INamedTypeSymbol ? GetFriendlyName((INamedTypeSymbol)typeParameters[i]) : typeParameters[i].Name;
				friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
			}

			return friendlyName;
		}

		//From: https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Symbols/TypeSymbolExtensions.cs#L183
		public static INamedTypeSymbol GetEnumUnderlyingType(this ITypeSymbol type)
		{
			return (type is INamedTypeSymbol namedType) ? namedType.EnumUnderlyingType : null;
		}

		public static string ToFullName([NotNull] this ITypeSymbol type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			if (type.ContainingNamespace != null)
				if (type.ContainingType == null)
					return $"{type.ContainingNamespace.FullNamespaceString()}.{type.GetFriendlyName()}";
				else
					return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
			else
			{
				if(type.ContainingType == null)
					return type.GetFriendlyName();
				else
					return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
			}
		}

		public static string FullNamespaceString([NotNull] this INamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol == null) throw new ArgumentNullException(nameof(namespaceSymbol));

			return namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
		}

		public static bool IsEnumType(this ITypeSymbol type)
		{
			return type.GetEnumUnderlyingType() != null;
		}

		//See: https://stackoverflow.com/questions/37327056/retrieve-all-types-with-roslyn-within-a-solution
		public static IEnumerable<INamedTypeSymbol> GetAllTypes(this Compilation compilation) =>
			GetAllTypes(compilation.Assembly.TypeNames, compilation);

		static IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<string> types, Compilation compilation)
		{
			return GetAllTypes(compilation.Assembly.GlobalNamespace)
				.Where(t => types.Any(ts => t.Name.Contains(ts)));
		}

		static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
		{
			foreach(INamedTypeSymbol type in @namespace.GetTypeMembers())
			foreach(INamedTypeSymbol nestedType in GetNestedTypes(type))
				yield return nestedType;

			foreach(INamespaceSymbol nestedNamespace in @namespace.GetNamespaceMembers())
			foreach(INamedTypeSymbol type in GetAllTypes(nestedNamespace))
				yield return type;
		}

		static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
		{
			yield return type;
			foreach(INamedTypeSymbol nestedType in type.GetTypeMembers()
				.SelectMany(GetNestedTypes))
				yield return nestedType;
		}
	}
}
