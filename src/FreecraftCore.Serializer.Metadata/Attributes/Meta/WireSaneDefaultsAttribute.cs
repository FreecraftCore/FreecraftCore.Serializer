using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta attribute that indicates to the serializer it should use SANE default serialization
	/// behavior rather than the default World of Warcraft serialization/original FreecraftCore serialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class WireSaneDefaultsAttribute : Attribute
	{

	}
}
