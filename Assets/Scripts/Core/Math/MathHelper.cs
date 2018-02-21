namespace pdxpartyparrot.Core.Math
{
    public static class MathHelper
    {
        public static float WrapAngle(float angle)
        {
            if(angle > 360.0f) {
                return 360.0f - angle;
            }

            if(angle < -360.0f) {
                return 360.0f + angle;
            }

            return angle;
        }
    }
}