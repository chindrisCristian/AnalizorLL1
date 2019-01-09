using System.Collections.Generic;

namespace MiniInterpreterCore
{
	public class RegulaProductie
	{
		#region Public properties

		public string LeftSide { get; set; }

		public List<string> RightSide { get; set; } = new List<string>();

		#endregion

		#region Public Methods
		public override string ToString()
		{
			string result = string.Format($"{LeftSide} -> ");
			if (RightSide.Count == 0)
				result += "ε";
			foreach (var tmp in RightSide)
				result += tmp;
			return result;
		}

		public string PrintRule()
		{
			string result = string.Format($"{LeftSide} -> ");
			if (RightSide.Count == 0)
				result += "ε";
			foreach (var tmp in RightSide)
				result += string.Format($"{tmp} ");
			return result;
		}

		public RegulaProductie Clone() => new RegulaProductie
		{
			LeftSide = (string)LeftSide.Clone(),
			RightSide = new List<string>(RightSide)
		};
		#endregion
	}
}
