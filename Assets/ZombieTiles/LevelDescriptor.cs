public class LevelDescriptor
{

    public enum Type
    {
        GENERATED,
        MANUAL
    }

    public LevelDescriptor(int id, Type type)
    {
        this.id = id;
        this.type = type;
    }

    public int id { get; set; }
    public Type type { get; set; }
}