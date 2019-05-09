using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct CommonData
  {
    // Identifiers
    private string _name;
    private string sidebar_name;

    public string Name { get { return _name; } }
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }
    public int ID { get; private set; }
    public int dataID { get { return ID % Globals.ActorLimit; } }
    public string Key { get { return _name + " " + ID; } }

    // Ownership
    public int PrevID;
    public int NextID;
    public int ParentID;
    public bool AttachToParent;
    public int PrevSiblingID;
    public int NextSiblingID;
    public int FirstChildID;
    public int LastChildID;
    public int NumberOfChildren;

    public void Init(ActorCreationInfo acreate)
    {
      _name = acreate.Name;
      sidebar_name = "";
      PrevID = -1;
      NextID = -1;
      ParentID = -1;
      AttachToParent = false;
      PrevSiblingID = -1;
      NextSiblingID = -1;
      FirstChildID = -1;
      LastChildID = -1;
      NumberOfChildren = 0;
    }
    
    public void Reset()
    {
      _name = "New Actor";
      sidebar_name = "";
      PrevID = -1;
      NextID = -1;
      ParentID = -1;
      AttachToParent = false;
      PrevSiblingID = -1;
      NextSiblingID = -1;
      FirstChildID = -1;
      LastChildID = -1;
      NumberOfChildren = 0;
    }
  }
}
