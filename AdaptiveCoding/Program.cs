using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using PeterO;
using PeterO.Numbers;

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

	static class Program
	{
		public static List<Node> nodes = new List<Node>();
		public static Dictionary<char, long> keyAndFrequencies = new Dictionary<char, long>();

		
		static void Main(string[] args)
		{
			Console.WriteLine("Enter message: ");
			string word = Console.ReadLine().ToLower();
			string temp = '.' + string.Concat(word.Distinct().OrderBy(c => c));
			word += '.';
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
			BigInteger test = multiGCD(nodes[nodes.Count - 1].cbi, nodes[nodes.Count - 1].cei, nodes[nodes.Count - 1].zi);


			BigInteger ch1 = nodes[nodes.Count - 1].cbi / test;

			BigInteger ch2 = nodes[nodes.Count - 1].cei / test;

			BigInteger z1 = nodes[nodes.Count - 1].zi / test;

			BigInteger power = new BigInteger(0);

			while (Power(2, power) < z1)
			{
				power++;
			}

			BigInteger resultStep = Power(2, power);
			BigInteger ccode = 0;
			ccode = nodes[nodes.Count - 1].cbi * resultStep / nodes[nodes.Count - 1].zi;
			string encode = "0.";
			encode += ToBinaryString(ccode);
			//decode
			nodes.Clear();
			EFloat decode = (EFloat)(double)ccode / (double)Power(2, power);
			
			foreach (var c in word)
			{
				nodes.Add(new Node(temp) { Symbol = c });
			}
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
			
			Node DecodeNode = new Node(temp);
			string result = "";
			while (true)
			{
				for (int i = 0; i < DecodeNode.arrey.Length; i++)
				{
					DecodeNode.si += DecodeNode.arrey[i];
				}
				EFloat interval = 0;
				EFloat[,] arreyPr = new EFloat[temp.Length, 2];
				for (int i = 0; i < DecodeNode.arrey.Length; i++)
				{
					arreyPr[i, 0] = interval;
					interval += (double)DecodeNode.arrey[i] / (double)DecodeNode.si;
					
					arreyPr[i, 1] = interval;
					
				}
				int count = 0;
				for (int i = 0; i < temp.Length; i++)
				{
					EFloat g =  EFloat.Max(decode, arreyPr[i, 0]);

					EFloat g1 = EFloat.Max(decode, arreyPr[i, 1]);

					if (decode > arreyPr[i, 0] && decode < arreyPr[i, 1])
					{
								result += temp[i];
								DecodeNode.arrey[i] += 1;
								DecodeNode.si = 0;
								decode = (decode - arreyPr[i, 0]) / (arreyPr[i, 1] - arreyPr[i, 0]);
								break;
					}
						
					
				}
				if (result[result.Length - 1] == '.') { Console.WriteLine(result); break; }
			}

		}

		public static double FromBinary(string encode)
		{
			double ten = 0;
			for (int i = 2; i < encode.Length; i++)
			{
				if (encode[i] == '1') ten += OtrPower(2, i * -1) * 2;
			}
			return ten;
		}
		public static float OtrPower(float a, float b)
		{
			b = b * -1;
			float result = 1 / ((float)Power((BigInteger)a, (BigInteger)b));
			return result;
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