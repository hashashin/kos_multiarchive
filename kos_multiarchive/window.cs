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

using UnityEngine;

namespace kos_multiarchive
{
    partial class kos_multiarchive
    {
        private void ListWindow(int windowId)
        {
            if (_dirList != null)
            {
                _scrollViewVector = GUILayout.BeginScrollView(_scrollViewVector);
                var options = new[] { GUILayout.Width(160f), GUILayout.ExpandWidth(false) };
                _selectionGridInt = GUILayout.SelectionGrid(_selectionGridInt, _dirList.ToArray(), 1, options);
                GUILayout.EndScrollView();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Change Archive"))
            {
                ChangeArch();
            }
            if (GUILayout.Button("New"))
            {
                NewArch();
            }
            GUILayout.EndHorizontal();
            if (!_isorig)
            {
                if (GUILayout.Button("Return to Original"))
                {
                    RestoreOrig();
                }
            }
            GUI.contentColor = Color.green;
            // Refresh images list.
            if (GUILayout.Button("Refresh list"))
            {
                GetDirs();
            }
            GUI.contentColor = Color.white;
            // Close the list window.
            if (GUI.Button(new Rect(2f, 2f, 13f, 13f), "X"))
            {
                _visible = !_visible;
            }
            // Makes the window dragable.
            GUI.DragWindow();
        }
    }
}
