using System;

public class Quest
{
	public enum QuestType { Beat5Enemies, PickFirstItem, BeatBoss }
	public QuestType Type;
	public string Name;
	public string Description;
	public bool Cleared;

	public Quest(QuestType type, string name, string description)
	{
		Type = type;
		Name = name;
		Description = description;
		Cleared = false;
	}

	//status for Quests
	public string Status => Cleared ? "Cleared" : "In-Progress";
}
