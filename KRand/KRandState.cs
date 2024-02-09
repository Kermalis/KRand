using System;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.KRand;

public struct KRandState : IEquatable<KRandState>
{
	public ulong S0;
	public ulong S1;
	public ulong S2;
	public ulong S3;

	public byte Counter_Bool;
	public ulong State_Bool;

	public byte Counter_8;
	public ulong State_8;

	public byte Counter_16;
	public ulong State_16;

	public uint? Next32State;

	public static bool operator ==(KRandState left, KRandState right)
	{
		return left.Equals(right);
	}
	public static bool operator !=(KRandState left, KRandState right)
	{
		return !(left == right);
	}

	public override readonly int GetHashCode()
	{
		int a = HashCode.Combine(S0, S1, S2, S3);
		int b = HashCode.Combine(Counter_Bool, Counter_8, Counter_16);
		int c = HashCode.Combine(State_Bool, State_8, State_16, Next32State);
		return HashCode.Combine(a, b, c);
	}
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is KRandState other && Equals(other);
	}
	public readonly bool Equals(KRandState other)
	{
		return S0 == other.S0 && S1 == other.S1 && S2 == other.S2 && S3 == other.S3
			&& Counter_Bool == other.Counter_Bool && Counter_8 == other.Counter_8 && Counter_16 == other.Counter_16
			&& State_Bool == other.State_Bool && State_8 == other.State_8 && State_16 == other.State_16 && Next32State == other.Next32State;
	}
}