using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

public sealed partial class RandomTests
{
	public sealed class TestDistribution_SByte
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_SByte(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_100_000_000, 16)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, long Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				sbyte v = rand.NextSByte();
				int idx = GetBucket(v, sbyte.MinValue, sbyte.MaxValue, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(sbyte.MinValue, sbyte.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_SByte_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_SByte_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_100_000_000, 10, -115, 115)]
		public void Test(uint numRuns, int numBuckets, sbyte min, sbyte max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, long Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				sbyte v = rand.NextSByte(min, max);
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
