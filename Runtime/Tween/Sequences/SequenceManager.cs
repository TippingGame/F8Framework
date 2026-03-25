namespace F8Framework.Core
{
	public static class SequenceManager
	{
		public static Sequence GetSequence()
		{
			return Tween.Instance?.GetSequence();
		}

		public static void KillSequence(this Sequence sequence)
		{
			Tween.Instance?.KillSequence(sequence);
		}
		
		public static void KillAllSequences()
		{
			Tween.Instance?.KillAllSequences();
		}
	}
}
