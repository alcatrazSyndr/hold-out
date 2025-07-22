namespace HoldOut
{
    public class MenuManager : Manager<MenuManager>
    {
        #region Setup

        protected override void Setup()
        {
            base.Setup();

            _isSetup = true;
        }

        #endregion
    }
}
