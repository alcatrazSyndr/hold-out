namespace HoldOut
{
    public class CameraManager : Manager<CameraManager>
    {
        protected override void Setup()
        {
            base.Setup();

            _isSetup = true;
        }
    }
}
