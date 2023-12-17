using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Globals
{
    public static BeatManager BeatManager = null;
    public static DropManager DropManager = null;
    public static SpawnManager SpawnManager = null;
    public static PriestCalculator PriestCalculator = null;
    public static ExitPoints ExitPoints = null;
    public static CameraProfileManager CameraProfileManager = null;
    public static PlayerInputsAssigner PlayerInputsAssigner = null;
    public static TutorialManager TutorialManager = null;
    public static GameManager GameManager = null;
    public static WinMenu WinMenu = null;
    public static MenuMusicPlayer MenuMusicPlayer = null;
    public static DatabaseActionSprites DatabaseActionSprites = null;
    public static DataLoader DataLoader = null;
    public static CSVLoader DataControllerType = null;
}