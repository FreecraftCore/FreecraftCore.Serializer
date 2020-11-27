using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public sealed class ExternalContentCollectorSerializationSourceOutputStrategy : ISerializationSourceOutputStrategy
	{
		public Dictionary<string, string> Content { get; } = new Dictionary<string, string>();

		public void Output(string name, string content)
		{
			Content.Add(name, content);
		}
	}
}
