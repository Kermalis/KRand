using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	public sealed class TestDistribution_Single
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Single(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_050_000_000, 10)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, decimal Avg)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				decimal v = (decimal)rand.NextSingle();
				int idx = GetBucket(v, 0, 1, numBuckets);
				UpdateAverage(v, ref buckets[idx].Num, ref buckets[idx].Avg);
			}

			decimal increment = CalcIncrement(0, 1, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Avg(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Avg, numRuns, _output);
			}
		}
	}
}