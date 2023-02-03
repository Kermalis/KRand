using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

public sealed partial class RandomTests
{
	public sealed class TestDistribution_Int64
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Int64(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 16)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, decimal Avg)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				decimal v = rand.NextInt64();
				int idx = GetBucket(v, long.MinValue, long.MaxValue, numBuckets);
				UpdateAverage(v, ref buckets[idx].Num, ref buckets[idx].Avg);
			}

			decimal increment = CalcIncrement(long.MinValue, long.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Avg(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Avg, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_Int64_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Int64_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10, -5_000_000_1_000_000_000, 8_123_172_036_454_775_807)]
		public void Test(uint numRuns, int numBuckets, long min, long max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, decimal Avg)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				decimal v = rand.NextInt64(min, max);
				int idx = GetBucket(v, min, max, numBuckets);
				UpdateAverage(v, ref buckets[idx].Num, ref buckets[idx].Avg);
			}

			decimal increment = CalcIncrement(min, max, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Avg(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Avg, numRuns, _output);
			}
		}
	}
}
