using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ByteBrewSettingsHandler
{
    private static ByteBrewSettings _settingsInstance = null;
    public static ByteBrewSettings SettingsInstance 
    {
        get {
            if (_settingsInstance == null) {
                _settingsInstance = GetOrCreateByteBrewSettings();
            }
            return _settingsInstance;
        }
    }

    private static ByteBrewSettings GetOrCreateByteBrewSettings() 
    {
        string bytebrewSettingsDirPath = Path.Combine("Assets", "ByteBrewSDK", "Resources");
        string bytebrewSettingsPath = Path.Combine(bytebrewSettingsDirPath, "ByteBrewSettings.asset");

        AssetDatabase.Refresh();

        int loops = 0;
        // Wait for the AssetDatabase to refresh
        while (AssetDatabase.IsAssetImportWorkerRunning() || EditorApplication.isCompiling || EditorApplication.isUpdating) {
            Debug.Log("Waiting for AssetDatabase to refresh...");
            System.Threading.Thread.Sleep(100);

            loops++;
            if (loops > 100) {
                Debug.LogError("AssetDatabase refresh timed out.");
                return null;
            }
        }
        
        ByteBrewSettings settings = AssetDatabase.LoadAssetAtPath<ByteBrewSettings>(bytebrewSettingsPath);
        if (settings != null) {
            return settings;
        }

        if (!Directory.Exists(bytebrewSettingsDirPath)) {
            Directory.CreateDirectory(bytebrewSettingsDirPath);
            AssetDatabase.Refresh();
        }

        try {
            settings = ScriptableObject.CreateInstance<ByteBrewSettings>();

            AssetDatabase.CreateAsset(settings, bytebrewSettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("ByteBrewSettings.asset created successfully");
        } catch (System.Exception e) {
            Debug.LogError("Error creating ByteBrewSettings.asset: " + e.Message);
        }

        return settings;
    }
}
