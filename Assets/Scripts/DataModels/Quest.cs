using System;

[Serializable]
public class Quest
{
  public int quest_id;
  public string quest_name;
  public string quest_description;
  public string quest_type;
  public int required_level;
  public int experience_reward;
}
