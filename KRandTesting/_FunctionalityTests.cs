using System;
using Xunit;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	private void FakeUsage(KRand rand)
	{
		_ = rand.NextBoolean();
		_ = rand.NextBoolean();
		_ = rand.NextBoolean();
		_ = rand.NextBoolean();
		_ = rand.NextBoolean();
		_ = rand.NextBoolean();
		_ = rand.NextUInt16();
		_ = rand.NextInt16();
		_ = rand.NextInt32();
		_ = rand.NextSingle();
		_ = rand.NextUInt32();
		_ = rand.NextInt64();
		_ = rand.NextDouble();

		Span<uint> items = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];

		rand.Shuffle(items);
		_ = rand.RandomElement((ReadOnlySpan<uint>)items);
	}

	[Fact]
	public void TestStateModification()
	{
		var rand = new KRand();
		KRandState initialState = rand.GetState();
		ulong firstValue = rand.NextUInt64();
		KRandState nextState = rand.GetState();

		Assert.NotEqual(initialState, nextState);

		FakeUsage(rand);
		KRandState finalState = rand.GetState();

		rand.SetState(initialState);
		Assert.Equal(initialState, rand.GetState());
		Assert.Equal(firstValue, rand.NextUInt64());
		Assert.Equal(nextState, rand.GetState());

		FakeUsage(rand);
		Assert.Equal(finalState, rand.GetState());
	}
}