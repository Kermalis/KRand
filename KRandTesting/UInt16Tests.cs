using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	public sealed class TestDistribution_UInt16
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt16(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 16)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, ulong Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				ushort v = rand.NextUInt16();
				int idx = GetBucket(v, ushort.MinValue, ushort.MaxValue, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(ushort.MinValue, ushort.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_UInt16_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt16_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10, 14_519, 31_972)]
		public void Test(uint numRuns, int numBuckets, ushort min, ushort max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, ulong Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				ushort v = rand.NextUInt16(min, max);
				int idx = GetBucket(v, min, max, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(min, max, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
}