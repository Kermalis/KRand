using Xunit;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

public sealed partial class RandomTests
{
	public sealed class TestDistribution_Boolean
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Boolean(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(1_000_000_000)]
		public void Test(uint numRuns)
		{
			var rand = new KRand();
			uint[] buckets = new uint[2];

			for (uint i = 0; i < numRuns; ++i)
			{
				int idx = rand.NextBoolean() ? 1 : 0;
				buckets[idx]++;
			}

			decimal r = numRuns;
			for (int i = 0; i < buckets.Length; ++i)
			{
				uint num = buckets[i];

				_output.WriteLine(string.Format("Bucket {0} | {1:N0} ({2:P2})",
					i != 0, num, num / r));
			}
		}
	}
	public sealed class TestDistribution_Boolean_Chance
	{
		private readonly ITestOutputHelper _output;
		public TestDistribution_Boolean_Chance(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData(0_500_000_000, 7, 10)]
		public void Test(uint numRuns, int chanceNumerator, int chanceDenominator)
		{
			var rand = new KRand();
			uint[] buckets = new uint[2];

			for (uint i = 0; i < numRuns; ++i)
			{
				int idx = rand.NextBoolean(chanceNumerator, chanceDenominator) ? 1 : 0;
				buckets[idx]++;
			}

			decimal r = numRuns;
			for (int i = 0; i < buckets.Length; ++i)
			{
				uint num = buckets[i];

				_output.WriteLine(string.Format("Bucket {0} | {1:N0} ({2:P2})",
					i != 0, num, num / r));
			}
		}
	}
}
