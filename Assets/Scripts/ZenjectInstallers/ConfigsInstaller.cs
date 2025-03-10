using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "ScriptableObjects/Create ConfigsInstaller asset")]
public class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
{
    [SerializeField] private ConfigForStarsSpawner _configForStartSpawner;
    [SerializeField] private ConfigOfLevels _configOfLevels;
    [SerializeField] private ConfigOfRewards _configOfRewards;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private VisualPresets _visualPresets;

    public override void InstallBindings()
    {
        Container.BindInstance(_configForStartSpawner).AsSingle().NonLazy();
        Container.BindInstance(_configOfLevels).AsSingle().NonLazy();
        Container.BindInstance(_configOfRewards).AsSingle().NonLazy();
        Container.BindInstance(_gameConfig).AsSingle().NonLazy();
        Container.BindInstance(_visualPresets).AsSingle().NonLazy();
    }
}
