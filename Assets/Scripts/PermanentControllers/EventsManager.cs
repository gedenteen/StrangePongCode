using UnityEngine.Events;

public static class EventsManager
{
    public static UnityEvent<int> levelLoaded = new UnityEvent<int>(); //int for number of a level
    public static UnityEvent levelStart = new UnityEvent();
    public static UnityEvent levelEnd = new UnityEvent();
    public static UnityEvent<StatusOfLevelEnd> levelEndWithStatus = new UnityEvent<StatusOfLevelEnd>();
    public static UnityEvent disableSoundForBalls = new UnityEvent();
    public static UnityEvent<int> resetSpeedSummandForBallAccelerator = new UnityEvent<int>();

    // StartTriggers
    public static UnityEvent changeBallDirection = new UnityEvent();
    public static UnityEvent changeMusicTrack = new UnityEvent();
    public static UnityEvent changeBallBehavior = new UnityEvent();
    public static UnityEvent createGhostBall = new UnityEvent();
}
