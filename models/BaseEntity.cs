namespace DebugOmgDispClient.models
{
    public abstract record BaseEntity
    {
        public virtual int Id 
        { 
            get; protected set; 
        }
    }
}
