using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

partial class RandomTests
{
	public sealed class TestDistribution_Byte
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Byte(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_100_000_000, 16)]
		public void Test(uint numRuns, int numBuckets)
		{
			var rand = new KRand();
			var buckets = new (uint Num, ulong Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				byte v = rand.NextByte();
				int idx = GetBucket(v, byte.MinValue, byte.MaxValue, numBuckets);
				buckets[idx].Num++;
				buckets[idx].Sum += v;
			}

			decimal increment = CalcIncrement(byte.MinValue, byte.MaxValue, numBuckets, out decimal rangeBegin);
			for (int i = 0; i < buckets.Length; ++i)
			{
				WriteResult_Sum(ref rangeBegin, increment, i, numBuckets, buckets[i].Num, buckets[i].Sum, numRuns, _output);
			}
		}
	}
	public sealed class TestDistribution_Byte_Range
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Byte_Range(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_100_000_000, 10, 15, 245)]
		public void Test(uint numRuns, int numBuckets, byte min, byte max)
		{
			var rand = new KRand();
			var buckets = new (uint Num, ulong Sum)[numBuckets];

			for (uint i = 0; i < numRuns; ++i)
			{
				byte v = rand.NextByte(min, max);
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