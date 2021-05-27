using System;
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
			arrey[i] = 1;
			
		}
		public char Symbol { get; set; }
		public BigInteger Si { get; set; }
		public BigInteger Qi { get; set; }
		public BigInteger Pi { get; set; }
		public BigInteger Cbi { get; set; }
		public BigInteger Zi { get; set; }
		public BigInteger Cei { get; set; }
	}

	static class Program
	{


		public static List<Node> nodes = new();
		public static Dictionary<char, long> keyAndFrequencies = new();


		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.Unicode;
			Console.InputEncoding = System.Text.Encoding.Unicode;

			Console.WriteLine("Enter message: ");
			string word = Console.ReadLine().ToLower();
			string temp = '.' + string.Concat(word.Distinct().OrderBy(c => c));
			
			word += '.';


			foreach (var c in word)
			nodes.Add(new Node(temp) { Symbol = c });
		
			Console.WriteLine($"Word = {word}");
			Console.WriteLine();
			nodes[0].Si = temp.Length;

			for (int step = 1; step < nodes.Count; step++)
			{
				BigInteger[] temparray = new BigInteger[temp.Length];
				for (int i = 0; i < nodes[step - 1].arrey.Length; i++)				
					temparray[i] = nodes[step - 1].arrey[i];
				
				nodes[step].arrey = temparray;
				nodes[step].arrey[temp.IndexOf(nodes[step - 1].Symbol)] += 1;

				for (int i = 0; i < nodes[step].arrey.Length; i++)		
					nodes[step].Si += nodes[step].arrey[i];		
			}
			
			BigInteger sum = 0;
			for (int i = 0; i < nodes.Count; i++)
			{
				for (int j = 0; j < temp.Length; j++)
				{
					if (temp[j] == nodes[i].Symbol)
					{
						nodes[i].Qi = sum;
						nodes[i].Pi = sum + nodes[i].arrey[j];
						break;
					}
					sum += nodes[i].arrey[j];
				}
				sum = 0;
			}
			for (int i = 0; i < nodes.Count; i++)
			if (i == 0)
			{
				nodes[i].Cbi = nodes[i].Qi;
				nodes[i].Cei = nodes[i].Pi;
				nodes[i].Zi = nodes[i].Si;
			}
			else
			{
				nodes[i].Cbi = nodes[i - 1].Cbi * nodes[i].Si + nodes[i].Qi * (nodes[i - 1].Cei - nodes[i - 1].Cbi);
				nodes[i].Cei = nodes[i - 1].Cbi * nodes[i].Si + nodes[i].Pi * (nodes[i - 1].Cei - nodes[i - 1].Cbi);
				nodes[i].Zi = nodes[i - 1].Zi * nodes[i].Si;
			}
			BigInteger number = nodes[^1].Cbi + nodes[^1].Cei;
			BigInteger test = MultiGCD(nodes[^1].Cbi, nodes[^1].Cei, nodes[^1].Zi);
			BigInteger z1 = nodes[^1].Zi / test;

			BigInteger power = 0;

			while (Power(2, power) < z1)			
				power++;

			BigInteger ccode = nodes[^1].Cbi * Power(2, power) / nodes[^1].Zi + 1;
			string encode = "0.";

			encode += ToBinaryString(ccode);
			Console.WriteLine($"Зашифровано {encode}");
			string decode = Decode(temp, encode);
			Console.WriteLine($"Декодування  {decode}");
		}




		public static string Decode(string temp,string binaryEncode)
		{
			double decode = Convertbinaryfloat(binaryEncode) ;
			Node DecodeNode = new(temp);
			string result = "";
			while (true)
			{
				double interval = 0;
				double[,] arreyPr = new double[temp.Length, 2];

				for (int i = 0; i < DecodeNode.arrey.Length; i++)
					DecodeNode.Si += DecodeNode.arrey[i];

				for (int i = 0; i < DecodeNode.arrey.Length; i++)
				{
					arreyPr[i, 0] = interval;
					interval += (double)DecodeNode.arrey[i] / (double)DecodeNode.Si;
					arreyPr[i, 1] = interval;
				}
				for (int i = 0; i < temp.Length; i++)
				if (decode > arreyPr[i, 0] && decode <= arreyPr[i, 1])
				{
					result += temp[i];
					DecodeNode.arrey[i] += 1;
					DecodeNode.Si = 0;
					decode = (decode - arreyPr[i, 0]) / (arreyPr[i, 1] - arreyPr[i, 0]);
					break;
				}
				if (result[^1] == '.')
					return result;
			}
		}


		public static double Convertbinaryfloat(string binary)
		{
			binary = binary.Replace(".", "");
			double result = 0;
			for (int i = 0; i < binary.Length; i++)
			{
				if (binary[i] == '0')
					result += 0 * OtrPower(2, i * -1);
				if (binary[i] == '1')			
					result += 1 * OtrPower(2, i * -1);
				
			}
			return result;
		}

		public static double OtrPower(double a, double b)
		{
			b *= -1;
			double result = 1 / (Math.Pow(a, b));
			return result;
		}






		static BigInteger ClassicGCD(BigInteger a, BigInteger b)
		{
			while (b != 0)
			{
				BigInteger temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}
		static BigInteger MultiGCD(params BigInteger[] n)
		{
			if (n.Length == 0) return 0;
			BigInteger i, gcd = n[0];
			for (i = 0; i < n.Length - 1; i++)
				gcd = ClassicGCD(gcd, n[(int)i + 1]);
			return gcd;
		}

		static BigInteger Power(BigInteger x, BigInteger n)
		{
			if (n == 0)
				return 1;
			if ((n & 1) == 0)
			{

				var p = Power(x, n >> 1);
				return p * p;
			}
			else
				return x * Power(x, n - 1);

		}

		public static string ToBinaryString(this BigInteger bigint)
		{
			var bytes = bigint.ToByteArray();
			var idx = bytes.Length - 1;
			var base2 = new StringBuilder(bytes.Length * 8);
			var binary = Convert.ToString(bytes[idx], 2);
			if (binary[0] != '0' && bigint.Sign == 1)
				base2.Append('0');
			base2.Append(binary);
			for (idx--; idx >= 0; idx--)
				base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
			return base2.ToString();
		}

	}

}