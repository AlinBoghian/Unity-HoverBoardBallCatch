namespace DefaultNamespace.Scenes
{
    public class ControlledState:IHoverBoardState
    {
        public void ComputeMovement(Hoverboard hoverboard)
        {
            hoverboard.ApplyPhysicsFromInput();
        }
    }
}