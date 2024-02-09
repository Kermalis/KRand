using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	public sealed class TestDistribution_UInt64
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt64(ITestOutputHelper output)
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
				decimal v = rand.NextUInt64();
				int idx = GetBucket(v, ulong.MinValue, ulong.MaxValue, numBuckets);
				UpdateAverage(v, ref buckets[idx].Num, ref buckets[idx].Avg);
			}

			decimal increment = CalcIncrement(ulong.MinValue, ulong.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Avg(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Avg, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_UInt64_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_UInt64_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10, 1_000_000_000, 18_446_244_013_709_451_615)]
		public void Test(uint numRuns, int numBuckets, ulong min, ulong max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, decimal Avg)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				decimal v = rand.NextUInt64(min, max);
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