using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public static class ISymbolExtensions
	{
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

		public static string GetFriendlyName(this ITypeSymbol type)
		{
			if (type is IArrayTypeSymbol arrayTypeSymbol)
				return $"{arrayTypeSymbol.ElementType.GetFriendlyName()}[]";

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

		//See: https://stackoverflow.com/questions/37327056/retrieve-all-types-with-roslyn-within-a-solution
		public static IEnumerable<INamedTypeSymbol> GetAllTypes(this Compilation compilation) =>
			GetAllTypes(compilation.Assembly.TypeNames, compilation);

		public static IEnumerable<INamedTypeSymbol> GetAllTypes(this IEnumerable<string> types, Compilation compilation)
		{
			return GetAllTypes(compilation.Assembly.GlobalNamespace)
				.Where(t => types.Any(ts => t.Name.Contains(ts)));
		}

		public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol @namespace)
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
