using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Collection that provides <see cref="IMemberSerializationMediator{TContainingType}"/>s based on the generic type parameter
	/// built from the provided dependencies.
	/// </summary>
	/// <typeparam name="TTargetType">The target the mediator should be using.</typeparam>
	public class MemberSerializationMediatorCollection<TTargetType> : IEnumerable<IMemberSerializationMediator<TTargetType>>
	{
		/// <summary>
		/// Mediator factory service.
		/// </summary>
		[NotNull]
		private IMemberSerializationMediatorFactory mediatorFactory { get; }

		/// <summary>
		/// Lazy-initialized collection of the strongly genericly typed mediator objects.
		/// </summary>
		[NotNull]
		private Lazy<IEnumerable<IMemberSerializationMediator<TTargetType>>> lazyLoadedMediatorCollection { get; }

		public MemberSerializationMediatorCollection([NotNull] IMemberSerializationMediatorFactory mediatorFactory)
		{
			if (mediatorFactory == null) throw new ArgumentNullException(nameof(mediatorFactory));

			this.mediatorFactory = mediatorFactory;
			lazyLoadedMediatorCollection = new Lazy<IEnumerable<IMemberSerializationMediator<TTargetType>>>(CreateMediatorCollection, true);
		}

		public IEnumerator<IMemberSerializationMediator<TTargetType>> GetEnumerator()
		{
			return lazyLoadedMediatorCollection.Value.GetEnumerator();
		}

		protected IEnumerable<IMemberSerializationMediator<TTargetType>> CreateMediatorCollection()
		{
			//TODO: Cleaner/better way to provide instuctions
			//Build the instructions for serializion with mediators
			return typeof(TTargetType).MembersWith<WireMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyDeclaredOnly)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder)
				.Select(x => mediatorFactory.Create<TTargetType>(x))
				.ToArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
