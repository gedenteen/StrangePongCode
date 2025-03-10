using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private KeyBindingsController _keyBindingsController;
    [SerializeField] private GameObject _prefabSteamServices;
    [SerializeField] private GameConfig _gameConfig;

    public override void InstallBindings()
    {
        // BUG: if instaiate SteamService via this code, then player can't quit from the game via 
        // Steam. I added SteamServices as child object for PermanentManagers on Boot scene.
        // if (_gameConfig.AddSteamSevices)
        // {
        //     Container.InstantiatePrefab(_prefabSteamServices);
        // }

        Container.Bind<AudioManager>().FromInstance(_audioManager);
        Container.Bind<KeyBindingsController>().FromInstance(_keyBindingsController);

    }
}
