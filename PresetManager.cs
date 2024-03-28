﻿using System;
using System.IO;
using System.Windows.Forms;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.Registry;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Desktoptale
{
    public class PresetManager
    {
        public const string PRESET_FILE_EXTENSION = "dt";

        private ISerializer serializer;
        private IRegistry<CharacterType, string> characterRegistry;

        public PresetManager(IRegistry<CharacterType, string> characterRegistry)
        {
            this.characterRegistry = characterRegistry;
            
            serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            
            MessageBus.Subscribe<SavePresetRequestedMessage>(OnSavePresetRequestedMessage);
            MessageBus.Subscribe<SetPresetFileAssociationRequestedMessage>(OnSetPresetFileAssociationRequestedMessage);
        }

        public CharacterProperties LoadPreset(string path)
        {
            if (path == null) return null;
            
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
                
                string serialized = File.ReadAllText(path);

                if (!serialized.StartsWith($"Version: {Preset.FILE_FORMAT_VERSION}"))
                {
                    throw new Exception("Unknown file format. You might be trying to open a preset created using a newer version of Desktoptale in an older version.");
                }
                
                Preset preset = deserializer.Deserialize<Preset>(serialized);
                return preset.ToCharacterProperties(id => characterRegistry.Get(id), windowName =>
                {
                    if (!string.IsNullOrWhiteSpace(windowName))
                    {
                        WindowInfo target = WindowsUtils.GetWindowByName(windowName);
                        if (target != null)
                        {
                            return target;
                        }
                    }
                    
                    return null;
                });
            }
            catch (Exception e)
            {
                WindowsUtils.ShowMessageBox($"Failed to load preset: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // public void LoadPreset(Settings settings)
        // {
        //     if (settings.Preset == null) return;
        //     
        //     try
        //     {
        //         var deserializer = new DeserializerBuilder()
        //             .WithNamingConvention(PascalCaseNamingConvention.Instance)
        //             .Build();
        //         
        //         string serialized = File.ReadAllText(settings.Preset);
        //
        //         if (!serialized.StartsWith($"Version: {Preset.FILE_FORMAT_VERSION}"))
        //         {
        //             throw new Exception("Unknown file format. You might be trying to open a preset created using a newer version of Desktoptale in an older version.");
        //         }
        //         
        //         Preset preset = deserializer.Deserialize<Preset>(serialized);
        //         preset.Apply(settings);
        //     }
        //     catch (Exception e)
        //     {
        //         WindowsUtils.ShowMessageBox($"Failed to load preset: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //     }
        // }

        private void OnSavePresetRequestedMessage(SavePresetRequestedMessage message)
        {
            WindowsUtils.SaveDialogResult saveDialogResult =
                WindowsUtils.OpenSaveDialog($"Preset files (*.{PRESET_FILE_EXTENSION})|*.{PRESET_FILE_EXTENSION}|All files (*.*)|*.*");

            if (saveDialogResult.Result == WindowsUtils.SaveDialogResult.DialogState.Succeeded)
            {
                string serialized = serializer.Serialize(new Preset(message.Target.Properties, type => characterRegistry.GetId(type)));

                using (StreamWriter writer = new StreamWriter(saveDialogResult.OutputStream))
                {
                    writer.Write(serialized);
                }
            }
            else if (saveDialogResult.Result == WindowsUtils.SaveDialogResult.DialogState.Failed)
            {
                WindowsUtils.ShowMessageBox("Failed to save preset.", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // private void OnSavePresetRequestedMessage(SavePresetRequestedMessage message)
        // {
        //     WindowsUtils.SaveDialogResult saveDialogResult =
        //         WindowsUtils.OpenSaveDialog($"Preset files (*.{PRESET_FILE_EXTENSION})|*.{PRESET_FILE_EXTENSION}|All files (*.*)|*.*");
        //
        //     if (saveDialogResult.Result == WindowsUtils.SaveDialogResult.DialogState.Succeeded)
        //     {
        //         string serialized = serializer.Serialize(new Preset(settings));
        //
        //         using (StreamWriter writer = new StreamWriter(saveDialogResult.OutputStream))
        //         {
        //             writer.Write(serialized);
        //         }
        //     }
        //     else if (saveDialogResult.Result == WindowsUtils.SaveDialogResult.DialogState.Failed)
        //     {
        //         WindowsUtils.ShowMessageBox("Failed to save preset.", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //     }
        // }

        private void OnSetPresetFileAssociationRequestedMessage(SetPresetFileAssociationRequestedMessage message)
        {
            WindowsUtils.RegisterForFileExtension(PRESET_FILE_EXTENSION);
        }
    }
}