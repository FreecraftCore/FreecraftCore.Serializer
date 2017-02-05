using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;

namespace FreecraftCore.Serializer
{
	public class LambdabasedDeserializationPrototyeFactory : IDeserializationPrototypeFactory
	{
		private static IDictionary<Type, Func<object>> generatorMap { get; } = new Dictionary<Type, Func<object>>();

		private ReaderWriterLockSlim lockObj { get; } = new ReaderWriterLockSlim();

		public TType Create<TType>()
		{
			lockObj.EnterReadLock();
			try
			{
				if (generatorMap.ContainsKey(typeof(TType)))
					return (TType)generatorMap[typeof(TType)]();

				//else it's not in the collection
			}
			finally
			{
				lockObj.ExitReadLock();
			}

			//need to add it to the collection; must use write lock
			lockObj.EnterWriteLock();
			try
			{
				//Basically double check locking
				if (generatorMap.ContainsKey(typeof(TType)))
					return (TType)generatorMap[typeof(TType)]();

				//At this point we know we are write locked AND it's not in the collection.
				generatorMap[typeof(TType)] =
					Expression.Lambda<Func<TType>>(Expression.New(typeof(TType))).Compile()
						as Func<object>;
			}
			finally
			{
				lockObj.ExitWriteLock();
			}

			//it's in the collection now; recur
			return Create<TType>();
		}
	}

	public class LambdabasedDeserializationPrototyeFactory<TType> : LambdabasedDeserializationPrototyeFactory, IDeserializationPrototypeFactory<TType>
	{
		//Mono doesn't like creating lambdas for interface types. .NET will do it, but will probably throw if you invoke it.
		//However, Mono will throw if you try to compile the lambda
#if __MonoCS__
		//New constaint is gone; this provided an efficient way to create new instances over Activator.CreateInstance.
		//Search compiled lambda and new constaint operator on google to see dicussions about it
		private static Func<TType> instanceGeneratorDelegate { get; }

		static LambdabasedDeserializationPrototyeFactory()
		{
			if (!typeof(TType).IsInterface)
				instanceGeneratorDelegate = Expression.Lambda<Func<TType>>(Expression.New(typeof(TType))).Compile();
			else
				instanceGeneratorDelegate = null;
		}

		public TType Create()
		{
			if(instanceGeneratorDelegate != null)
				return instanceGeneratorDelegate();
			else
				throw new InvalidOperationException($"Tried to create an instance of Type: {typeof(TType)} but Type was an interface. Cannot create an interface type.");
		}
#else
		//New constaint is gone; this provided an efficient way to create new instances over Activator.CreateInstance.
		//Search compiled lambda and new constaint operator on google to see dicussions about it
		private static Func<TType> instanceGeneratorDelegate { get; } = Expression.Lambda<Func<TType>>(Expression.New(typeof(TType))).Compile();

		public TType Create()
		{
			return instanceGeneratorDelegate();
		}
#endif
	}
}
