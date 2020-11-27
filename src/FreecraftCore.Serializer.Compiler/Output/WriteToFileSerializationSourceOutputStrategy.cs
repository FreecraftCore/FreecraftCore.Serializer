using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public sealed class WriteToFileSerializationSourceOutputStrategy : ISerializationSourceOutputStrategy
	{
		/// <summary>
		/// The output path for serializers.
		/// </summary>
		public string OutputPath { get; }

		public WriteToFileSerializationSourceOutputStrategy([NotNull] string outputPath)
		{
			if (outputPath == null) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputPath));
			OutputPath = outputPath;
		}

		public void Output([NotNull] string name, [NotNull] string content)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(content));

			string rootPath = Path.Combine(OutputPath, $"Strategy");
			if(!Directory.Exists(rootPath))
				Directory.CreateDirectory(rootPath);

			//Write the packet file out
			File.WriteAllText(Path.Combine(rootPath, $"{name}.cs"), content);
		}
	}
}
