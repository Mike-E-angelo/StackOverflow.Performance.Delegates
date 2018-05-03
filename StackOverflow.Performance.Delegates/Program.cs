using BenchmarkDotNet.Running;

namespace StackOverflow.Performance.Delegates
{
	sealed class Program
	{
		static void Main()
		{
			BenchmarkRunner.Run<Delegates>();
		}
	}
}