namespace HoldOut
{
    public class AudioManager : Manager<AudioManager>
    {
        protected override void Setup()
        {
            base.Setup();
            
            _isSetup = true;
        }
    }
}
