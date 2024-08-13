using Godot;
using System;

public partial class Setting : Control
{
	private HSlider volumeSlider;
	private Label volumeLabel;

	private const string ConfigFilePath = "user://settings.cfg";
	private const string VolumeKey = "audio/master_volume";

	public override void _Ready()
	{
		volumeSlider = GetNode<HSlider>("VBoxContainer/VolumeSlider");
		volumeLabel = GetNode<Label>("VBoxContainer/VolumeLabel");

		double initialVolume = LoadVolume();
		volumeSlider.Value = initialVolume;

		float volumeDb = Mathf.LinearToDb((float)(initialVolume / 100.0));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), volumeDb);

		volumeLabel.Text = $"Master Volume: {(int)volumeSlider.Value}%";

		volumeSlider.ValueChanged += _on_volume_slider_value_changed;
	}

	private void _on_volume_slider_value_changed(double value)
	{
		float volumeDb = Mathf.LinearToDb((float)(value / 100.0));

		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), volumeDb);

		volumeLabel.Text = $"Master Volume: {(int)value}%";

		SaveVolume(value);
	}

	private void SaveVolume(double value)
	{
		ConfigFile configFile = new ConfigFile();
		configFile.Load(ConfigFilePath);
		configFile.SetValue("Audio", VolumeKey, value);
		configFile.Save(ConfigFilePath);
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
