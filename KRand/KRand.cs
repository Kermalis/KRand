using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kermalis.KRand;

// Xoshiro256**
// https://prng.di.unimi.it/
// https://prng.di.unimi.it/xoshiro256starstar.c
// https://source.dot.net/#System.Private.CoreLib/Random.Xoshiro256StarStarImpl.cs,bb77e610694e64ca
// Lemire's bounds method
// https://lemire.me/blog/2019/06/06/nearly-divisionless-random-integer-generation-on-various-systems/
// https://www.pcg-random.org/posts/bounded-rands.html
// https://shufflesharding.com/posts/dissecting-lemire

/// <summary>An inclusive-ranged Xoshiro256** randomizer with Lemire's bounds method</summary>
public class KRand
{
	protected const MethodImplOptions FAST_INLINE = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

	private ulong _s0;
	private ulong _s1;
	private ulong _s2;
	private ulong _s3;

	private byte _boolStateCounter;
	private ulong _boolState;

	private byte _8StateCounter;
	private ulong _8State;

	private byte _16StateCounter;
	private ulong _16State;

	private uint? _next32State;

	public KRand(ulong? seed = null)
	{
		ulong s = seed ?? (ulong)DateTime.UtcNow.Ticks;
		_s0 = SplitMix64(ref s);
		_s1 = SplitMix64(ref s);
		_s2 = SplitMix64(ref s);
		_s3 = SplitMix64(ref s);

		if (_s0 == 0 && _s1 == 0 && _s2 == 0 && _s3 == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(seed), $"Invalid seed supplied: {s}");
		}
	}

	/// <summary>
	/// Initializes a new <see cref="KRand"/> instance using a previously
	/// captured state.
	/// </summary>
	/// <param name="state">Encoded state.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="state"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="state"/> is a whitespace-only string.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="state"/> is invalid.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="state"/> is corrupt.</exception>
	public KRand(string state)
	{
		SetState(state);
	}

	/// <summary>
	/// Gets the current state.
	/// </summary>
	/// <returns>Encoded state.</returns>
	public string GetState()
	{
		var builder = new StringBuilder("v1:");
		builder.Append($"{_s0}:");
		builder.Append($"{_s1}:");
		builder.Append($"{_s2}:");
		builder.Append($"{_s3}:");
		builder.Append($"{_boolStateCounter}:");
		builder.Append($"{_boolState}:");
		builder.Append($"{_8StateCounter}:");
		builder.Append($"{_8State}:");
		builder.Append($"{_16StateCounter}:");
		builder.Append($"{_16State}:");
		builder.Append($"{_next32State}:");
		builder.Append(NextUInt64());

		return builder.ToString();
	}

	private void SetState(string state)
	{
		ArgumentException.ThrowIfNullOrEmpty(state);

		var parts = state.Split(':');
		if (parts.Length != 13)
			throw new ArgumentException($"{nameof(state)} is invalid.");

		try
		{
			_s0 = ulong.Parse(parts[1]);
			_s1 = ulong.Parse(parts[2]);
			_s2 = ulong.Parse(parts[3]);
			_s3 = ulong.Parse(parts[4]);
			_boolStateCounter = byte.Parse(parts[5]);
			_boolState = ulong.Parse(parts[6]);
			_8StateCounter = byte.Parse(parts[7]);
			_8State = ulong.Parse(parts[8]);
			_16StateCounter = byte.Parse(parts[9]);
			_16State = ulong.Parse(parts[10]);
			_next32State = uint.TryParse(parts[11], out var next32State)
				? next32State
				: null;
		}
		catch(Exception ex)
		{
			throw new ArgumentException($"{nameof(state)} is invalid.", ex);
		}

		var expectedNext = ulong.Parse(parts[12]);
		if (expectedNext != NextUInt64())
		{
			throw new ArgumentException("State is corrupt.");
		}
	}
	
	[MethodImpl(FAST_INLINE)]
	private static ulong SplitMix64(ref ulong x)
	{
		ulong z = x += 0x9E3779B97F4A7C15;
		z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
		z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
		return z ^ (z >> 31);
	}

	public bool NextBoolean()
	{
		if (_boolStateCounter == 0)
		{
			_boolState = NextUInt64();
			_boolStateCounter = 64;
		}

		bool result = (_boolState & 1) == 0;
		_boolStateCounter--;
		_boolState >>= 1;
		return result;
	}
	[MethodImpl(FAST_INLINE)]
	public bool NextBoolean(int chanceNumerator, int chanceDenominator)
	{
		if (chanceNumerator >= chanceDenominator)
		{
			return true;
		}
		if (chanceNumerator <= 0)
		{
			return false;
		}
		if (chanceNumerator == chanceDenominator / 2)
		{
			return NextBoolean();
		}
		return NextInt32(0, chanceDenominator - 1) < chanceNumerator;
	}
	[MethodImpl(FAST_INLINE)]
	public bool NextBoolean(uint chanceNumerator, uint chanceDenominator)
	{
		if (chanceNumerator >= chanceDenominator)
		{
			return true;
		}
		if (chanceNumerator == 0)
		{
			return false;
		}
		if (chanceNumerator == chanceDenominator / 2)
		{
			return NextBoolean();
		}
		return NextUInt32(0, chanceDenominator - 1) < chanceNumerator;
	}

	/// <summary>Returns a value in the range [<see cref="sbyte.MinValue"/>, <see cref="sbyte.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public sbyte NextSByte()
	{
		return (sbyte)NextByte();
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public sbyte NextSByte(sbyte min, sbyte max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == sbyte.MinValue && max == sbyte.MaxValue)
		{
			return NextSByte();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		byte range = (byte)(max - min + 1);
		return (sbyte)(LemireBoundsByte(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="byte.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public byte NextByte()
	{
		if (_8StateCounter == 0)
		{
			_8State = NextUInt64();
			_8StateCounter = 8;
		}

		byte result = (byte)_8State;
		_8StateCounter--;
		_8State >>= 8;
		return result;
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public byte NextByte(byte min, byte max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == byte.MinValue && max == byte.MaxValue)
		{
			return NextByte();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		byte range = (byte)(max - min + 1);
		return (byte)(LemireBoundsByte(range) + min);
	}

	/// <summary>Returns a value in the range [<see cref="short.MinValue"/>, <see cref="short.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public short NextInt16()
	{
		return (short)NextUInt16();
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public short NextInt16(short min, short max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == short.MinValue && max == short.MaxValue)
		{
			return NextInt16();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		ushort range = (ushort)(max - min + 1);
		return (short)(LemireBoundsUInt16(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="ushort.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public ushort NextUInt16()
	{
		if (_16StateCounter == 0)
		{
			_16State = NextUInt64();
			_16StateCounter = 4;
		}

		ushort result = (ushort)_16State;
		_16StateCounter--;
		_16State >>= 16;
		return result;
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public ushort NextUInt16(ushort min, ushort max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == ushort.MinValue && max == ushort.MaxValue)
		{
			return NextUInt16();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		ushort range = (ushort)(max - min + 1);
		return (ushort)(LemireBoundsUInt16(range) + min);
	}

	/// <summary>Returns a value in the range [<see cref="int.MinValue"/>, <see cref="int.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public int NextInt32()
	{
		return (int)NextUInt32();
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public int NextInt32(int min, int max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == int.MinValue && max == int.MaxValue)
		{
			return NextInt32();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		uint range = (uint)((long)max - min + 1);
		return (int)(LemireBoundsUInt32(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="uint.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public uint NextUInt32()
	{
		if (_next32State is null)
		{
			ulong n = NextUInt64();
			_next32State = (uint)(n & 0xFFFFFFFF);
			return (uint)(n >> 32);
		}

		uint result = _next32State.Value;
		_next32State = null;
		return result;
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public uint NextUInt32(uint min, uint max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == uint.MinValue && max == uint.MaxValue)
		{
			return NextUInt32();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		uint range = max - min + 1;
		return LemireBoundsUInt32(range) + min;
	}

	/// <summary>Returns a value in the range [<see cref="long.MinValue"/>, <see cref="long.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public long NextInt64()
	{
		return (long)NextUInt64();
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public long NextInt64(long min, long max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == long.MinValue && max == long.MaxValue)
		{
			return NextInt64();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}
		ulong range = (ulong)((Int128)max - min + 1);
		return (long)((Int128)LemireBoundsUInt64(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="ulong.MaxValue"/>]</summary>
	public ulong NextUInt64()
	{
		ulong result = BitOperations.RotateLeft(_s1 * 5, 7) * 9;
		ulong t = _s1 << 17;

		_s2 ^= _s0;
		_s3 ^= _s1;
		_s1 ^= _s2;
		_s0 ^= _s3;

		_s2 ^= t;

		_s3 = BitOperations.RotateLeft(_s3, 45);

		return result;
	}
	/// <summary>Returns a value in the range [<paramref name="min"/>, <paramref name="max"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public ulong NextUInt64(ulong min, ulong max)
	{
		if (min == max)
		{
			return min;
		}
		if (min == ulong.MinValue && max == ulong.MaxValue)
		{
			return NextUInt64();
		}
		if (max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		ulong range = max - min + 1;
		return LemireBoundsUInt64(range) + min;
	}

	/// <summary>Returns a value in the range [0, 1]</summary>
	[MethodImpl(FAST_INLINE)]
	public float NextSingle()
	{
		//return (NextUInt64() >> 40) * (1.0f / (1u << 24));
		return (NextUInt32() >> 8) * (1.0f / (1u << 24));
	}
	/// <summary>Returns a value in the range [0, 1]</summary>
	[MethodImpl(FAST_INLINE)]
	public double NextDouble()
	{
		return (NextUInt64() >> 11) * (1.0 / (1ul << 53));
	}

	/// <summary>Returns a vector with each value in the range [0, 1]</summary>
	[MethodImpl(FAST_INLINE)]
	public Vector3 NextVector3()
	{
		return new Vector3(NextSingle(), NextSingle(), NextSingle());
	}

	[MethodImpl(FAST_INLINE)]
	public T RandomElement<T>(ReadOnlySpan<T> source)
	{
		if (source.Length == 1)
		{
			return source[0];
		}
		if (source.Length == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
		}
		return source[NextInt32(0, source.Length - 1)];
	}
	[MethodImpl(FAST_INLINE)]
	public T RandomElement<T>(IReadOnlyList<T> source)
	{
		int count = source.Count - 1;
		if (count == 0)
		{
			return source[0];
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
		}
		return source[NextInt32(0, count)];
	}
	/// <summary>Fisher-Yates Shuffle</summary>
	public void Shuffle<T>(Span<T> source)
	{
		int count = source.Length - 1;
		for (int a = count - 1; a >= 0; --a)
		{
			int b = NextInt32(a, count);
			(source[b], source[a]) = (source[a], source[b]);
		}
	}
	/// <summary>Fisher-Yates Shuffle</summary>
	public void Shuffle<T>(IList<T> source)
	{
		int count = source.Count - 1;
		for (int a = count - 1; a >= 0; --a)
		{
			int b = NextInt32(a, count);
			(source[b], source[a]) = (source[a], source[b]);
		}
	}

	/// <summary>Returns a value in the range [0, <paramref name="range"/>). <paramref name="range"/> is "max - min + 1"</summary>
	private byte LemireBoundsByte(byte range)
	{
		byte x = NextByte();
		ushort m = (ushort)(x * range);
		byte l = (byte)m;
		if (l < range)
		{
			byte t = (byte)(0 - range);
			if (t >= range)
			{
				t -= range;
				if (t >= range)
				{
					t %= range;
				}
			}
			while (l < t)
			{
				x = NextByte();
				m = (ushort)(x * range);
				l = (byte)m;
			}
		}
		return (byte)(m >> 8);
	}
	/// <summary>Returns a value in the range [0, <paramref name="range"/>). <paramref name="range"/> is "max - min + 1"</summary>
	private ushort LemireBoundsUInt16(ushort range)
	{
		ushort x = NextUInt16();
		uint m = (uint)x * range;
		ushort l = (ushort)m;
		if (l < range)
		{
			ushort t = (ushort)(0 - range);
			if (t >= range)
			{
				t -= range;
				if (t >= range)
				{
					t %= range;
				}
			}
			while (l < t)
			{
				x = NextUInt16();
				m = (uint)x * range;
				l = (ushort)m;
			}
		}
		return (ushort)(m >> 16);
	}
	/// <summary>Returns a value in the range [0, <paramref name="range"/>). <paramref name="range"/> is "max - min + 1"</summary>
	private uint LemireBoundsUInt32(uint range)
	{
		uint x = NextUInt32();
		ulong m = (ulong)x * range;
		uint l = (uint)m;
		if (l < range)
		{
			uint t = 0u - range;
			if (t >= range)
			{
				t -= range;
				if (t >= range)
				{
					t %= range;
				}
			}
			while (l < t)
			{
				x = NextUInt32();
				m = (ulong)x * range;
				l = (uint)m;
			}
		}
		return (uint)(m >> 32);
	}
	/// <summary>Returns a value in the range [0, <paramref name="range"/>). <paramref name="range"/> is "max - min + 1"</summary>
	private ulong LemireBoundsUInt64(ulong range)
	{
		ulong x = NextUInt64();
		UInt128 m = (UInt128)x * range;
		ulong l = (ulong)m;
		if (l < range)
		{
			ulong t = 0uL - range;
			if (t >= range)
			{
				t -= range;
				if (t >= range)
				{
					t %= range;
				}
			}
			while (l < t)
			{
				x = NextUInt64();
				m = (UInt128)x * range;
				l = (ulong)m;
			}
		}
		return (ulong)(m >> 64);
	}
}
