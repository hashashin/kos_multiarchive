// -------------------------------------------------------------------------------------------------
// kos_multiarchive 0.2
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
using LibGit2Sharp;
using UnityEngine;

namespace kos_multiarchive
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public partial class kos_multiarchive : MonoBehaviour
    {
        private Rect _windowRect;

        private string _keybind;

        private bool _visible;

        private IButton _button;
        private const string Tooltipoff = "Show KOS-MA";
        private const string BtextureOff = "kos_ma/Textures/icon_off";

        //applauncher
        private Texture2D _buttonTexture = new Texture2D(38, 38, TextureFormat.ARGB32, false);
        private ApplicationLauncherButton _appbutton;


        private string _version;
        private string _versionlastrun;

        private readonly string _scriptdir = @KSPUtil.ApplicationRootPath.Replace("\\", "/") + "Ships/Script";

        private Repository _repo;
        private List<Branch> _branchList;
        private List<string> _branchNameList;
        private Vector2 _scrollViewVector = Vector2.zero;
        private int _selectionGridInt;
        private bool _isorig;
        private string _inusearch;

        void Awake()
        {
            LoadVersion();
            VersionCheck();
            LoadSettings();
            if (ToolbarManager.ToolbarAvailable) return;
            ApplicationLauncherReady();
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(Unreadifying);
        }

        void Start()
        {
            if (_branchNameList == null)
            {
                GetBranches();
            }
            if (Directory.Exists($"{_scriptdir}/Script_orig") && _inusearch == String.Empty)
            {
                var now = DateTime.Now;
                _isorig = false;
                _inusearch = $"arch_recovered_{now.Hour}{now.Minute}{now.Second}";
            }
            if (ToolbarManager.ToolbarAvailable)
            {
                _button = ToolbarManager.Instance.add("kos_ma", "toggle");
                _button.TexturePath = BtextureOff;
                _button.ToolTip = Tooltipoff;
                _button.OnClick += (e =>
                {
                    Toggle();
                });
            }
        }

        void OnGUI()
        {
            if (_visible)
            {
                _windowRect = GUI.Window(GUIUtility.GetControlID(0, FocusType.Passive), _windowRect, ListWindow, _inusearch);
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(_keybind))
            {
                Toggle();
            }
        }

        void OnDestroy()
        {
            SaveSettings();
            _button?.Destroy();
            if (_appbutton == null) return;
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(Unreadifying);
            AppButtonRemove();
        }
    }
}
