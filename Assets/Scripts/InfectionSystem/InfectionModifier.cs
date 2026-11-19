namespace Root
{
    public enum TipoModificador
    {
        Flat, 
        Percentage
    }

    [System.Serializable]
    public class InfectionModifier
    {
        public TipoModificador type;
        public float value;

        public InfectionModifier(TipoModificador type, float value)
        {
            this.type = type;
            this.value = value;
        }
        
    }
}
