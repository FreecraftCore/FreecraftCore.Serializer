using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class CompilerConstants
	{
		/// <summary>
		/// Consistently used input buffer name.
		/// </summary>
		public const string INPUT_BUFFER_NAME = "source";

		/// <summary>
		/// Consistently used name for the input offset.
		/// </summary>
		public const string INPUT_OFFSET_NAME = "offset";

		/// <summary>
		/// Consistently used name for the output buffer.
		/// </summary>
		public const string OUTPUT_BUFFER_NAME = "destination";

		/// <summary>
		/// Consistently used name for the output offset.
		/// </summary>
		public const string OUTPUT_OFFSET_NAME = INPUT_OFFSET_NAME;

		public const string SERIALZIABLE_OBJECT_REFERENCE_NAME = "value";
	}
}
