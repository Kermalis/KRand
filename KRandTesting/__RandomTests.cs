using System;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Kermalis.KRand.Tests;

public sealed partial class RandomTests
{
	private const MethodImplOptions FAST_INLINE = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

	/// <summary>Maps a value in the range [a1, a2] to [b1, b2]. Divide by zero occurs if a1 and a2 are equal</summary>
	[MethodImpl(FAST_INLINE)]
	public static decimal Lerp(in decimal value, in decimal a1, in decimal a2, in decimal b1, in decimal b2)
	{
		return b1 + ((value - a1) / (a2 - a1) * (b2 - b1));
	}
	[MethodImpl(FAST_INLINE)]
	private static int GetBucket(in decimal v, in decimal min, in decimal max, int numBuckets)
	{
		return Math.Min(numBuckets - 1, (int)Lerp(v, min, max, 0, numBuckets));
	}
	private static decimal CalcIncrement(in decimal min, in decimal max, int numBuckets, out decimal rangeBegin)
	{
		rangeBegin = min;
		return (max - min) / numBuckets;
	}
	private static void ToName(ref decimal rangeBegin, in decimal increment, int i, int numBuckets, out string range, out decimal expectedAvg)
	{
		decimal rangeEnd = rangeBegin + increment;
		range = string.Format("[{0:F5}, {1:F5}{2}", rangeBegin, rangeEnd, i == numBuckets - 1 ? ']' : ')');
		expectedAvg = Lerp(0.5m, 0, 1, rangeBegin, rangeEnd);
		rangeBegin = rangeEnd;
	}

	#region 8bit & 16bit & 32bit

	// Max/min value of 8/16/32 bits can still fit into 64 bits when multiplied by uint.MaxValue, so the sum can be a ulong, and a ulong fits into a decimal
	private static void WriteResult_Sum(ref decimal rangeBegin, in decimal increment, int i, int numBuckets, uint num, in decimal sum, in decimal numRuns, ITestOutputHelper o)
	{
		ToName(ref rangeBegin, increment, i, numBuckets, out string range, out decimal expectedAvg);

		o.WriteLine(string.Format("Bucket {0} | {1} | R AVG = {2:F5} ||| {3:N0} ({4:P2}) | SUM = {5:N0} | AVG = {6:F5}",
			i, range, expectedAvg, num, num / numRuns, sum, sum / num));
	}

	#endregion

	#region 64bit & Floats

	// These tests are limited by the amount of bits or precision
	// The decimal type is a saving grace when combined with this average calculation: https://stackoverflow.com/a/1934266
	// All integers 2^96 fit within a decimal, and all [0,1] instances of float or double also fit into a decimal
	[MethodImpl(FAST_INLINE)]
	private static void UpdateAverage(in decimal v, ref uint num, ref decimal avg)
	{
		avg += (v - avg) / (num + 1m);
		num++;
	}
	private static void WriteResult_Avg(ref decimal rangeBegin, in decimal increment, int i, int numBuckets, uint num, in decimal avg, in decimal numRuns, ITestOutputHelper o)
	{
		ToName(ref rangeBegin, increment, i, numBuckets, out string range, out decimal expectedAvg);

		o.WriteLine(string.Format("Bucket {0} | {1} | R AVG = {2:F5} ||| {3:N0} ({4:P2}) | AVG = {5:F5}",
			i, range, expectedAvg, num, num / numRuns, avg));
	}

	#endregion
}
