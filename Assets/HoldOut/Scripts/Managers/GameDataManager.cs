namespace HoldOut
{
    public class GameDataManager : Manager<GameDataManager>
    {
        protected override void Setup()
        {
            base.Setup();

            _isSetup = true;
        }
    }
}
