using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace StackOverflow.Performance.Delegates
{
	[CoreJob, ClrJob, DisassemblyDiagnoser(true, printSource: true)]
	public class Delegates
	{
		readonly DelegatePair<string, string> _empty;
		readonly DelegatePair<string, int>    _expression;
		readonly string                       _message;

		public Delegates() : this(new DelegatePair<string, string>(_ => default, _ => default),
		                          new DelegatePair<string, int>(x => x.Length, x => x.Length)) {}

		public Delegates(DelegatePair<string, string> empty, DelegatePair<string, int> expression,
		                 string message = "Hello World!")
		{
			_empty      = empty;
			_expression = expression;
			_message    = message;
			EmptyDeclared();
			EmptyCompiled();
			ExpressionDeclared();
			ExpressionCompiled();
		}

		[Benchmark]
		public void EmptyDeclared() => _empty.Declared(default);

		[Benchmark]
		public void EmptyCompiled() => _empty.Compiled(default);

		[Benchmark]
		public void ExpressionDeclared() => _expression.Declared(_message);

		[Benchmark]
		public void ExpressionCompiled() => _expression.Compiled(_message);
	}
}