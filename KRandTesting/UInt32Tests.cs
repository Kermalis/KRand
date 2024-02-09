using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	public sealed class TestDistribution_UInt32
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt32(ITestOutputHelper output)
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
				uint v = rand.NextUInt32();
				int idx = GetBucket(v, uint.MinValue, uint.MaxValue, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(uint.MinValue, uint.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_UInt32_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt32_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10, 2_651_879_544, 3_951_220_587)]
		public void Test(uint numRuns, int numBuckets, uint min, uint max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, ulong Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				uint v = rand.NextUInt32(min, max);
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