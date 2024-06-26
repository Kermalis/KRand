﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

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

	private KRandState _s;

	public KRand(in KRandState state)
	{
		SetState(state);
	}
	public KRand(ulong? seed = null)
	{
		ulong pickedSeed = seed ?? (ulong)DateTime.UtcNow.Ticks;
		ulong s = pickedSeed;
		_s.S0 = SplitMix64(ref s);
		_s.S1 = SplitMix64(ref s);
		_s.S2 = SplitMix64(ref s);
		_s.S3 = SplitMix64(ref s);

		if (_s.S0 == 0 && _s.S1 == 0 && _s.S2 == 0 && _s.S3 == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(seed), pickedSeed, "Invalid seed supplied");
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

	public KRandState GetState()
	{
		return _s;
	}
	public void SetState(in KRandState state)
	{
		if (state.S0 == 0 && state.S1 == 0 && state.S2 == 0 && state.S3 == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid state supplied");
		}

		_s = state;
	}

	public bool NextBoolean()
	{
		if (_s.Counter_Bool == 0)
		{
			_s.State_Bool = NextUInt64();
			_s.Counter_Bool = 64;
		}

		bool result = (_s.State_Bool & 1) == 0;
		_s.Counter_Bool--;
		_s.State_Bool >>= 1;
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
	[MethodImpl(FAST_INLINE)]
	public bool NextBoolean(ulong chanceNumerator, ulong chanceDenominator)
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
		return NextUInt64(0, chanceDenominator - 1) < chanceNumerator;
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
		if (_s.Counter_8 == 0)
		{
			_s.State_8 = NextUInt64();
			_s.Counter_8 = 8;
		}

		byte result = (byte)_s.State_8;
		_s.Counter_8--;
		_s.State_8 >>= 8;
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
		if (_s.Counter_16 == 0)
		{
			_s.State_16 = NextUInt64();
			_s.Counter_16 = 4;
		}

		ushort result = (ushort)_s.State_16;
		_s.Counter_16--;
		_s.State_16 >>= 16;
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
		ArgumentOutOfRangeException.ThrowIfLessThan(max, min, nameof(max));

		uint range = (uint)((long)max - min + 1);
		return (int)(LemireBoundsUInt32(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="uint.MaxValue"/>]</summary>
	[MethodImpl(FAST_INLINE)]
	public uint NextUInt32()
	{
		if (_s.Next32State is null)
		{
			ulong n = NextUInt64();
			_s.Next32State = (uint)(n >> 32);
			return (uint)n;
		}

		uint result = _s.Next32State.Value;
		_s.Next32State = null;
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
		ArgumentOutOfRangeException.ThrowIfLessThan(max, min, nameof(max));

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
		ArgumentOutOfRangeException.ThrowIfLessThan(max, min, nameof(max));

		ulong range = (ulong)((Int128)max - min + 1);
		return (long)((Int128)LemireBoundsUInt64(range) + min);
	}
	/// <summary>Returns a value in the range [0, <see cref="ulong.MaxValue"/>]</summary>
	public ulong NextUInt64()
	{
		ulong result = BitOperations.RotateLeft(_s.S1 * 5, 7) * 9;
		ulong t = _s.S1 << 17;

		_s.S2 ^= _s.S0;
		_s.S3 ^= _s.S1;
		_s.S1 ^= _s.S2;
		_s.S0 ^= _s.S3;

		_s.S2 ^= t;

		_s.S3 = BitOperations.RotateLeft(_s.S3, 45);

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
		ArgumentOutOfRangeException.ThrowIfLessThan(max, min, nameof(max));

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
	public ref readonly T RandomElement<T>(ReadOnlySpan<T> source)
	{
		if (source.Length == 1)
		{
			return ref source[0];
		}
		ArgumentOutOfRangeException.ThrowIfLessThan(source.Length, 1, nameof(source));
		return ref source[GetRandomIndex(0, source.Length - 1)];
	}
	[MethodImpl(FAST_INLINE)]
	public ref T RandomElement<T>(Span<T> source)
	{
		if (source.Length == 1)
		{
			return ref source[0];
		}
		ArgumentOutOfRangeException.ThrowIfLessThan(source.Length, 1, nameof(source));
		return ref source[GetRandomIndex(0, source.Length - 1)];
	}
	[MethodImpl(FAST_INLINE)]
	public ref T RandomElement<T>(T[] source)
	{
		return ref RandomElement(source.AsSpan());
	}
	[MethodImpl(FAST_INLINE)]
	public T RandomElement<T>(IReadOnlyList<T> source)
	{
		int count = source.Count - 1;
		if (count == 0)
		{
			return source[0];
		}
		ArgumentOutOfRangeException.ThrowIfLessThan(count, 0, nameof(source));
		return source[GetRandomIndex(0, count)];
	}
	/// <summary>Fisher-Yates Shuffle</summary>
	public void Shuffle<T>(Span<T> source)
	{
		int count = source.Length - 1;
		for (int a = count - 1; a >= 0; --a)
		{
			int b = GetRandomIndex(a, count);
			(source[b], source[a]) = (source[a], source[b]);
		}
	}
	/// <inheritdoc cref="Shuffle{T}(Span{T})"/>
	[MethodImpl(FAST_INLINE)]
	public void Shuffle<T>(T[] source)
	{
		Shuffle(source.AsSpan());
	}
	/// <inheritdoc cref="Shuffle{T}(Span{T})"/>
	public void Shuffle<T>(IList<T> source)
	{
		int count = source.Count - 1;
		for (int a = count - 1; a >= 0; --a)
		{
			int b = GetRandomIndex(a, count);
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

	/// <summary>Gets an index for an array or list while consuming the least amount of bytes necessary</summary>
	private int GetRandomIndex(int min, int max)
	{
		int delta = max - min;
		if (delta <= byte.MaxValue)
		{
			// For example: [2, 257]
			// delta = 255
			// rand(0, 255) = 5 => 7

			// For example: [1000, 1025]
			// delta = 25
			// rand(0, 25) = 22 => 1022
			return NextByte(0, (byte)delta) + min;
		}
		if (delta <= ushort.MaxValue)
		{
			// For example: [2, 65537]
			// delta = 65535
			// rand(0, 65535) = 5 => 7

			// For example: [1000, 10025]
			// delta = 9025
			// rand(0, 9025) = 200 => 1200
			return NextUInt16(0, (ushort)delta) + min;
		}
		return NextInt32(min, max);
	}
}