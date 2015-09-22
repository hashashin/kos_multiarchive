// -------------------------------------------------------------------------------------------------
// kos_multiarchive 0.0.1
//
// Simple KSP plugin to make kos have multiple archives (poor man style).
// Copyright (C) 2014 Iván Atienza
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// 
// Email: mecagoenbush at gmail dot com
// Freenode: hashashin
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace kos_multiarchive
{
    partial class kos_multiarchive 
    {
        private void LoadSettings()
        {
            KSPLog.print("[kos_multiarchive.dll] Loading Config...");
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<kos_multiarchive>();
            configfile.load();

            _visible = configfile.GetValue<bool>("visible", false);
            _inusearch = configfile.GetValue<string>("actualarch", String.Empty);
            _isorig = configfile.GetValue<bool>("isoriginal", true);

            _windowRect = configfile.GetValue<Rect>("windowpos", new Rect(50f, 25f, 200f, 260f));
            _keybind = configfile.GetValue<string>("keybind", "y");
            _versionlastrun = configfile.GetValue<string>("version");
            KSPLog.print("[kos_multiarchive.dll] Config Loaded Successfully");
        }

        private void SaveSettings()
        {
            KSPLog.print("[kos_multiarchive.dll] Saving Config...");
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<kos_multiarchive>();

            configfile.SetValue("visible", _visible);
            configfile.SetValue("actualarch", _inusearch);
            configfile.SetValue("isoriginal", _isorig);

            configfile.SetValue("windowpos", _windowRect);
            configfile.SetValue("keybind", _keybind);
            configfile.SetValue("version", _version);

            configfile.save();
            KSPLog.print("[kos_multiarchive.dll] Config Saved ");
        }

        public void Toggle()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                const string tooltipon = "Hide KOS-MA";
                const string btextureOn = "kos_ma/Textures/icon_on";
                if (_visible)
                {
                    _visible = false;
                    _button.TexturePath = BtextureOff;
                    _button.ToolTip = Tooltipoff;
                }
                else
                {
                    _visible = true;
                    _button.TexturePath = btextureOn;
                    _button.ToolTip = tooltipon;
                }
            }
            else
            {
                _visible = !_visible;
            }
        }

        private void VersionCheck()
        {
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            KSPLog.print("kos_multiarchive.dll version: " + _version);
            if ((_version != _versionlastrun) && (KSP.IO.File.Exists<kos_multiarchive>("config.xml")))
            {
                KSP.IO.File.Delete<kos_multiarchive>("config.xml");
            }
#if DEBUG
            KSP.IO.File.Delete<kos_multiarchive>("config.xml");
#endif
        }

        private void LoadVersion()
        {
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<kos_multiarchive>();
            configfile.load();
            _versionlastrun = configfile.GetValue<string>("version");
        }

        private void NewArch()
        {
            for (int i = 0; i <= 100; i++)
            {
                if (!Directory.Exists($"{_shipsdir}/arch_{i + 1}") && ("arch_" + (i + 1) != _inusearch) && (!Directory.Exists($"{_shipsdir}/arch_{i + 2}")))
                {
                    Directory.CreateDirectory($"{_shipsdir}/arch_{i + 1}");
                    GetDirs();
                    break;
                }
            }
        }

        private void RestoreOrig()
        {
            Directory.Move($"{_shipsdir}/Script", $"{_shipsdir}/{_inusearch}");
            Directory.Move($"{_shipsdir}/Script_orig", $"{_shipsdir}/Script");
            _inusearch = String.Empty;
            _isorig = true;
            GetDirs();
        }

        private void ChangeArch()
        {
            var newarch = _dirList[_selectionGridInt];
            if (_isorig)
            {
                _inusearch = newarch;
                Directory.Move($"{_shipsdir}/Script", $"{_shipsdir}/Script_orig");
                Directory.Move($"{_shipsdir}/{newarch}", $"{_shipsdir}/Script");
                _isorig = false;
                GetDirs();
            }
            else if (_inusearch != newarch && _inusearch != String.Empty)
            {
                Directory.Move($"{_shipsdir}/Script", $"{_shipsdir}/{_inusearch}");
                Directory.Move($"{_shipsdir}/{newarch}", $"{_shipsdir}/Script");
                _inusearch = newarch;
                GetDirs();
            }
        }

        private void GetDirs()
        {
            _dirList = new List<string>(Directory.GetDirectories(_shipsdir, "arch_*"));

            for (int i = 0; i < _dirList.Count; i++)
            {
                _dirList[i] = new DirectoryInfo(_dirList[i]).Name;
            }
        }

        private static void LoadTexture(ref Texture2D tex, string file, string folder)
        {
            //File Exists check
            if (File.Exists($"{folder}/{file}"))
            {
                tex.LoadImage(File.ReadAllBytes($"{folder}/{file}"));
            }
        }

        private void ApplicationLauncherReady()
        {
            if ((ApplicationLauncher.Ready) && (_appbutton == null))
            {
                LoadTexture(ref _buttonTexture, "kos_ma_icon.png",
                    KSPUtil.ApplicationRootPath.Replace("\\", "/") + "GameData/kos_ma/Textures/");
                _appbutton = ApplicationLauncher.Instance.AddModApplication(Toggle, Toggle, null, null, null,
                    null, ApplicationLauncher.AppScenes.MAINMENU| ApplicationLauncher.AppScenes.FLIGHT|ApplicationLauncher.AppScenes.SPACECENTER|
                    ApplicationLauncher.AppScenes.SPH|ApplicationLauncher.AppScenes.VAB, (Texture) _buttonTexture);
            }
        }

        private void AppButtonRemove()
        {
            if (_appbutton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_appbutton);
                _appbutton = null;
            }
        }

        private void Unreadifying(GameScenes scene)
        {
            AppButtonRemove();
        }
    }
}
