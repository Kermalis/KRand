using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

public sealed partial class RandomTests
{
	public sealed class TestDistribution_Int32
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Int32(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 16)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, long Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				int v = rand.NextInt32();
				int idx = GetBucket(v, int.MinValue, int.MaxValue, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(int.MinValue, int.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_Int32_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Int32_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10, -0_347_155_864, 1_357_821_457)]
		public void Test(uint numRuns, int numBuckets, int min, int max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, long Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				int v = rand.NextInt32(min, max);
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
