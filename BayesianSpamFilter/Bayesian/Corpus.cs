using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Bayesian
{

	public class Corpus
	{

		public const string TokenPattern = @"([a-zA-Zà-ÿÀ-ß]\w+)\W*";

		private SortedDictionary<string, int> _tokens = new SortedDictionary<string, int>();



		public SortedDictionary<string, int> Tokens
		{
			get { return _tokens; }
		}


		public Corpus()
		{
		}


		public Corpus(TextReader reader)
		{
			LoadFromReader(reader);
		}


		public Corpus(string filepath)
		{
			LoadFromFile(filepath);
		}


		public void LoadFromFile(string filepath)
		{
            try
            {
                using (StreamReader reader = new StreamReader(filepath, System.Text.Encoding.Default))
                {
                    LoadFromReader(reader);
                    reader.Close();
                }
            }
            catch
            {
                MessageBox.Show("Error.");
            }
		}


		public void LoadFromReader(TextReader reader)
		{
			Regex re = new Regex(TokenPattern, RegexOptions.Compiled);
			string line;
			while (null != (line = reader.ReadLine()))
			{
				Match m = re.Match(line);
				while (m.Success)
				{
					string token = m.Groups[1].Value;
					AddToken(token);
					m = m.NextMatch();
				}
			}
		}


		public void AddToken(string rawPhrase)
		{
			if (!_tokens.ContainsKey(rawPhrase))
			{
				_tokens.Add(rawPhrase, 1);
			}
			else
			{
				_tokens[rawPhrase]++;
			}
		}


	}

}
