using Xunit;

using FluentAssertions;

using System;
using System.Linq;

namespace Kermalis.KRand.Tests;

public sealed class StateTests
{
	[Fact]
	public void GetState_ShouldReturnValue()
	{
		var rand = new KRand();

		// Ensure state is returned
		var state = rand.GetState();
		state.Should().NotBeNullOrWhiteSpace();

		// Ensure state has requisite number of parts
		var parts = state.Split(':');
		parts.Should().HaveCount(13);
	}

	[Fact]
	public void Constructor_NullStateShouldThrowException()
	{
		Action constructor = () => new KRand(null as string);
		constructor.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Constructor_EmptyStateShouldThrowException()
	{
		Action constructor = () => new KRand(" ");
		constructor.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Constructor_TamperedStateShouldThrowException()
	{
		var rand = new KRand();
		var state = rand.GetState();
		state = state[..^1];

		Action constructor = () => new KRand(state);
		constructor.Should().Throw<ArgumentException>().WithMessage("State is corrupt.");
	}

	[Fact]
	public void RestoringStateShouldAllowSequenceReplication()
	{
		// Create a rand and generate a 100 numbers
		var original = new KRand();
		_ = Enumerable.Range(0, 100).Select(_ => original.NextInt32(-100, 100)).ToArray();

		// Persist the state and generate another 100 numbers
		var state = original.GetState();
		var expected = Enumerable.Range(0, 100).Select(_ => original.NextInt32(-100, 100)).ToArray();

		// Restore the previous state and generate another 100 numbers
		var restored = new KRand(state);
		var actual = Enumerable.Range(0, 100).Select(_ => restored.NextInt32(-100, 100)).ToArray();

		// The numbers generated after persisting the state
		// And the numbers generated after restoring the state
		// should be identical.
		actual.Should().BeEquivalentTo(expected);
	}
}