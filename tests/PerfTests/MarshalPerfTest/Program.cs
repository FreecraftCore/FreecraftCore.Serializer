using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MarshalPerfTest
{
	class Program
	{
		public unsafe static void Main(string[] args)
		{
			Stopwatch strucToPointerWatch = new Stopwatch();
			Stopwatch strucToPointerWatchGeneric = new Stopwatch();
			Stopwatch marshalCopy = new Stopwatch();
			
			long intTestVar = 5;
			byte[] testByteArray = new byte[sizeof(long)];
			
			int testIterations = 100000000;
			
			strucToPointerWatch.Start();
			
			for(int i = 0; i < testIterations; i++)
			{			
				fixed(byte* bytePtr = &testByteArray[0])
				{
					Marshal.StructureToPtr(i, new IntPtr(bytePtr), false);
				}
			}
			
			strucToPointerWatch.Stop();
			
			Console.WriteLine("StructureToPtr Non-Generic time: " + strucToPointerWatch.ElapsedMilliseconds);
			
			//explict collect after test
			GC.Collect();
			
			strucToPointerWatchGeneric.Start();
			
			for(int i = 0; i < testIterations; i++)
			{		
				fixed(byte* bytePtr = &testByteArray[0])
				{
					Marshal.StructureToPtr<long>(i, new IntPtr(bytePtr), false);
				}
			}
			
			strucToPointerWatchGeneric.Stop();
			
			Console.WriteLine("StructureToPtr Generic time: " + strucToPointerWatchGeneric.ElapsedMilliseconds);
			
			GC.Collect();
			
			marshalCopy.Start();
			
			for(int i = 0; i < testIterations; i++)
			{			
				Marshal.Copy(new IntPtr(&intTestVar), testByteArray, 0, sizeof(long));
			}
			
			marshalCopy.Stop();
			
			Console.WriteLine("Marshal Copy time: " + marshalCopy.ElapsedMilliseconds);
			
			
			Console.ReadKey();
		}
	}
}