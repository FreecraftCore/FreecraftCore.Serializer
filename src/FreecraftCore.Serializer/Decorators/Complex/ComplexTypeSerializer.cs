using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public abstract class ComplexTypeSerializer<TType> : SimpleTypeSerializerStrategy<TType>
	{	
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		//We use an array now for more performant iteration
		/// <summary>
		/// Ordered pairs of known serializer references and the memberinfos for wiremembers.
		/// </summary>
		[NotNull]
		protected IMemberSerializationMediator<TType>[] orderedMemberInfos { get; }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		[NotNull]
		protected IGeneralSerializerProvider serializerProviderService { get; }

		protected ComplexTypeSerializer([NotNull] IEnumerable<IMemberSerializationMediator<TType>> serializationDirections, [NotNull] IGeneralSerializerProvider serializerProvider)
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(serializerProvider)} service was null.");

			//TODO: Extract enumerable into its own class an test there.
			try
			{
				//Enumerating the medaitors can throw exceptions.
				orderedMemberInfos = serializationDirections.ToArray();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to create serializer {GetType().FullName} for Type: {typeof(TType).FullName}.", e);
			}
			
			serializerProviderService = serializerProvider;
		}

		protected void SetMembersFromReaderData(TType obj, [NotNull] IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//replaced for perf
			/*foreach (IMemberSerializationMediator<TType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.SetMember(obj, source);
			}*/
			for(int i = 0; i < orderedMemberInfos.Length; i++)
				orderedMemberInfos[i].SetMember(obj, source);
		}

		protected void WriteMemberData(TType obj, [NotNull] IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//replaced for perf
			/*foreach (IMemberSerializationMediator<TType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.WriteMember(obj, dest);
			}*/
			for (int i = 0; i < orderedMemberInfos.Length; i++)
				orderedMemberInfos[i].WriteMember(obj, dest);
		}

		/// <inheritdoc />
		public override TType ReadIntoObject(TType obj, IWireStreamReaderStrategy source)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Basically if someone calls this method they want us to set the members from the reader
			SetMembersFromReaderData(obj, source);

			return obj;
		}

		public override void ObjectIntoWriter(TType obj, IWireStreamWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//This method is only responsible for writing the members
			//Even if we're suppose to write type data for this type we don't
			//Just members
			WriteMemberData(obj, dest);
		}

		//TODO: Implement once we have write and read async
		/// <inheritdoc />
		public override async Task ObjectIntoWriterAsync(TType obj, IWireStreamWriterStrategyAsync dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//This method is only responsible for writing the members
			//Even if we're suppose to write type data for this type we don't
			//Just members
			await WriteMemberDataAsync(obj, dest);
		}

		/// <inheritdoc />
		public override async Task<TType> ReadIntoObjectAsync(TType obj, IWireStreamReaderStrategyAsync source)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Basically if someone calls this method they want us to set the members from the reader
			await SetMembersFromReaderDataAsync(obj, source);

			return obj;
		}

		protected async Task SetMembersFromReaderDataAsync(TType obj, [NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			// Tested, does not yield better perf
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < orderedMemberInfos.Length; i++)
				await orderedMemberInfos[i].SetMemberAsync(obj, source);
		}

		protected async Task WriteMemberDataAsync(TType obj, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			// Tested, does not yield better perf
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < orderedMemberInfos.Length; i++)
				await orderedMemberInfos[i].WriteMemberAsync(obj, dest);
		}
	}
}
