using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace StackOverflow.Performance.Delegates
{
	sealed class Program
	{
		static void Main()
		{
			BenchmarkRunner.Run<Delegates>();
		}
	}

	public struct DelegatePair<TFrom, TTo>
	{
		public DelegatePair(Func<TFrom, TTo> declared, Expression<Func<TFrom, TTo>> expression) : this(declared, expression.Compile()) {}

		DelegatePair(Func<TFrom, TTo> declared, Func<TFrom, TTo> compiled)
		{
			Declared = declared;
			Compiled = compiled;
		}

		public Func<TFrom, TTo> Declared { get; }

		public Func<TFrom, TTo> Compiled { get; }
	}

	public class MultipleJits : ManualConfig
	{
		public MultipleJits()
		{
			Add(Job.ShortRun.With(new MonoRuntime(name: "Mono x86", customPath: @"C:\Program Files (x86)\Mono\bin\mono.exe")).With(Platform.X86));
			Add(Job.ShortRun.With(new MonoRuntime(name: "Mono x64", customPath: @"C:\Program Files\Mono\bin\mono.exe")).With(Platform.X64));

			Add(Job.ShortRun.With(Jit.LegacyJit).With(Platform.X86).With(Runtime.Clr));
			Add(Job.ShortRun.With(Jit.LegacyJit).With(Platform.X64).With(Runtime.Clr));

			Add(Job.ShortRun.With(Jit.RyuJit).With(Platform.X64).With(Runtime.Clr));

			// RyuJit for .NET Core 1.1
			Add(Job.ShortRun.With(Jit.RyuJit).With(Platform.X64).With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp11));

			// RyuJit for .NET Core 2.0
			Add(Job.ShortRun.With(Jit.RyuJit).With(Platform.X64).With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp20));

			//Add(DisassemblyDiagnoser.Create(new DisassemblyDiagnoserConfig(printAsm: true, printPrologAndEpilog: true, recursiveDepth: 3)));
		}
	}

	[CoreJob, ClrJob]
	// [DisassemblyDiagnoser(true, printSource: true)]
	public class Delegates
	{
		readonly DelegatePair<string, string> _empty;
		readonly DelegatePair<string, int> _expression;
		readonly string _message;

		public Delegates() : this(new DelegatePair<string, string>(_ => default, _ => default), new DelegatePair<string, int>(x => x.Length, x => x.Length)) {}

		public Delegates(DelegatePair<string, string> empty, DelegatePair<string, int> expression, string message = "Hello World!")
		{
			_empty = empty;
			_expression = expression;
			_message = message;
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
