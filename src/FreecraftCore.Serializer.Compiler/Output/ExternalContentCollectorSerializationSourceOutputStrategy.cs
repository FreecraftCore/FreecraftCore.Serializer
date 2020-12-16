using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public sealed class ExternalContentCollectorSerializationSourceOutputStrategy : ISerializationSourceOutputStrategy
	{
		private readonly Dictionary<string, string> _content = new Dictionary<string, string>();

		public IReadOnlyDictionary<string, string> Content => _content;

		public void Output(string name, string content)
		{
			_content.Add(name, content);
		}
	}
}
