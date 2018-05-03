using System;
using System.Linq.Expressions;

namespace StackOverflow.Performance.Delegates
{
	public struct DelegatePair<TFrom, TTo>
	{
		DelegatePair(Func<TFrom, TTo> declared, Func<TFrom, TTo> compiled)
		{
			Declared = declared;
			Compiled = compiled;
		}

		public DelegatePair(Func<TFrom, TTo> declared, Expression<Func<TFrom, TTo>> expression) :
			this(declared, expression.Compile()) {}

		public Func<TFrom, TTo> Declared { get; }

		public Func<TFrom, TTo> Compiled { get; }
	}
}