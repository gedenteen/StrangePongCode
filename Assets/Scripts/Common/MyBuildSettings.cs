using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildPlatform
{
    PC,
    WebGL,
    Yandex
}

public static class MyBuildSettings
{
    public static BuildPlatform currentBuildPlatform = BuildPlatform.PC;
}
