using System;

namespace CommonLibrary.Application
{
	public enum ReleaseMode
	{
		None,
		Alpha,
		Beta,
		RC,
		Stable
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyStatusAttribute : System.Attribute
	{
		private ReleaseMode _releaseMode;
		public AssemblyStatusAttribute(ReleaseMode releaseMode)
		{
			_releaseMode = releaseMode;
		}
		public ReleaseMode Status
		{
			get { return _releaseMode; }
		}
	}
}
