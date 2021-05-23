﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arithmetic_coding
{
	public class Node
	{
		public BigInteger[] arrey;

		public Node(string message)
		{
			arrey = new BigInteger[message.Length];
			for (int i = 0; i < message.Length; i++)
			{
				arrey[i] = 1;
			}
		}

		public char Symbol { get; set; }
		public BigInteger si { get; set; }
		public BigInteger qi { get; set; }
		public BigInteger pi { get; set; }
		public BigInteger cbi { get; set; }
		public BigInteger zi { get; set; }
		public BigInteger cei { get; set; }
	}

	static class  Program
	{
		public static List<Node> nodes = new List<Node>();
		public static Dictionary<char, long> keyAndFrequencies = new Dictionary<char, long>();


		static void Main(string[] args)
		{
			string word = "sszzzuyzuuo".ToLower();
			string temp = string.Concat(word.Distinct().OrderBy(c => c));

			Console.WriteLine($"Word = {word}");
			Console.WriteLine();
			foreach (var c in word)
			{
				nodes.Add(new Node(temp) { Symbol = c });
			}
			nodes[0].si = temp.Length;

			for (int step = 1; step < nodes.Count; step++)
			{

				BigInteger[] temparray = new BigInteger[temp.Length];
				for (int i = 0; i < nodes[step - 1].arrey.Length; i++)
				{
					temparray[i] = nodes[step - 1].arrey[i];

				}
				nodes[step].arrey = temparray;
				nodes[step].arrey[temp.IndexOf(nodes[step - 1].Symbol)] += 1;

				for (int i = 0; i < nodes[step].arrey.Length; i++)
				{
					nodes[step].si += nodes[step].arrey[i];
				}
			}

			BigInteger sum = 0;
			for (int i = 0; i < nodes.Count; i++)
			{
				for (int j = 0; j < temp.Length; j++)
				{
					if (temp[j] == nodes[i].Symbol)
					{

						nodes[i].qi = sum;
						nodes[i].pi = sum + nodes[i].arrey[j];
						break;

					}
					sum += nodes[i].arrey[j];
				}
				sum = 0;
			}

			for (int i = 0; i < nodes.Count; i++)
			{
				if (i == 0)
				{
					nodes[i].cbi = nodes[i].qi;
					nodes[i].cei = nodes[i].pi;
					nodes[i].zi = nodes[i].si;
				}
				else
				{
					nodes[i].cbi = nodes[i - 1].cbi * nodes[i].si + nodes[i].qi * (nodes[i - 1].cei - nodes[i - 1].cbi);
					nodes[i].cei = nodes[i - 1].cbi * nodes[i].si + nodes[i].pi * (nodes[i - 1].cei - nodes[i - 1].cbi);
					nodes[i].zi = nodes[i - 1].zi * nodes[i].si;
				}
			}
			BigInteger number = nodes[nodes.Count - 1].cbi + nodes[nodes.Count - 1].cei;

			

			BigInteger test = multiGCD(nodes[nodes.Count-1].cbi, nodes[nodes.Count - 1].cei, nodes[nodes.Count - 1].zi);
			string encode = ToBinaryString(test);




			Console.WriteLine(encode);

		}
		static BigInteger classicGCD(BigInteger a, BigInteger b)
		{
			while (b != 0)
			{
				BigInteger temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}
		static BigInteger multiGCD(params BigInteger[] n)
		{
			if (n.Length == 0) return 0;
			BigInteger i, gcd = n[0];
			for (i = 0; i < n.Length - 1; i++)
				gcd = classicGCD(gcd, n[(int)i + 1]);
			return gcd;
		}




		static BigInteger Power(BigInteger x, BigInteger n)
		{
			if (n == 0)
			{
				return 1;
			}
			//у кратного числа останній біт рівний 0
			if ((n & 1) == 0)
			{
				//зміщення на один біт вправо рівносильно діленню на два	
				var p = Power(x, n >> 1);
				return p * p;
			}
			else
			{
				return x * Power(x, n - 1);
			}
		}

		public static string ToBinaryString(this BigInteger bigint)
		{
			var bytes = bigint.ToByteArray();
			var idx = bytes.Length - 1;


			var base2 = new StringBuilder(bytes.Length * 8);


			var binary = Convert.ToString(bytes[idx], 2);

			if (binary[0] != '0' && bigint.Sign == 1)
			{
				base2.Append('0');
			}

			base2.Append(binary);

			for (idx--; idx >= 0; idx--)
			{
				base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
			}

			return base2.ToString();
		}

	}

}