public class EnergyChunk{

    public EnergyType EnergyType {get; set;}
    public float Amount {get; set;}
}

public enum EnergyType{
    ELECTRIC,
    CHEMICAL,
    HEAT
}