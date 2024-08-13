using Godot;
using System;

public partial class GlobalConfig : Node
{
	private const string ConfigFilePath = "user://settings.cfg"; 
	private const string VolumeKey = "audio/master_volume"; 

	public override void _Ready()
	{
		double savedVolume = LoadVolume();
		float volumeDb = Mathf.LinearToDb((float)(savedVolume / 100.0));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), volumeDb);
	}

	private double LoadVolume()
	{
		ConfigFile configFile = new ConfigFile();
		Error err = configFile.Load(ConfigFilePath);

		if (err == Error.Ok)
		{
			if (configFile.HasSectionKey("Audio", VolumeKey))
			{
				return (double)configFile.GetValue("Audio", VolumeKey);
			}
		}

		return Mathf.DbToLinear((float)AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"))) * 100.0;
	}
}
