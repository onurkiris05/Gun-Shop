using Cinemachine;
using UnityEngine;

namespace Game.Managers
{
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("Components")]
        [SerializeField] private CinemachineVirtualCamera runningCam;
        [SerializeField] private CinemachineVirtualCamera minigameCam;
        [SerializeField] private CinemachineVirtualCamera closeLookUpCam;
        [SerializeField] private CinemachineVirtualCamera upgradeCam;

        public void SetCamera(CameraType state)
        {
            runningCam.Priority = state == CameraType.Running ? 10 : 0;
            minigameCam.Priority = state == CameraType.Minigame ? 10 : 0;
            closeLookUpCam.Priority = state == CameraType.CloseLookUp ? 10 : 0;
            upgradeCam.Priority = state == CameraType.Upgrade ? 10 : 0;
        }
    }

    public enum CameraType
    {
        Running,
        Minigame,
        CloseLookUp,
        Upgrade
    }
}